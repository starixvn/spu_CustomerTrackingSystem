function MatchMonitor() {
    let dataTable;
    let timeoutHolder = null;
    let domElements = {
        isHostPermission: $("#hdIsHostPermission").attr("IsHostPermission"),
        userId: $("#hdCTSCustId").val(),
        dpkFromDate: $('#dpkFromDate'),
        dpkToDate: $('#dpkToDate'),
        cbbIsVerified: $('#cbbIsVerified'),
        cbbBetType: $('#cbbBetType'),
        groupDropDown: $('.group-drop-down'),
        refreshList: $('#refreshList'),
        btnRefresh: $('#btnRefresh'),
        tblMonitor: $('#tblMonitor'),
        btnGenerate: $('#btnGenerate'),
        eventTarget: $('#selectedRefresh')[0]
    };

    function GetMatchMonitor() {
        if (ValidateRangeDate()) {
            dataTable ? dataTable.ajax.reload(null, true) : dataTable = GenerateGrid();
        }
    }

    function ValidateRangeDate() {
        let toDateValue = domElements.dpkToDate.datepicker("getDate");
        let toDateValueAllow = domElements.dpkFromDate.datepicker("getDate");
        toDateValueAllow.setDate(toDateValueAllow.getDate() + 6);

        if (toDateValue > toDateValueAllow) {
            common.showError("Invalid date range, please select not more than 7 days");
            return false;
        }

        return true;
    }

    function GenerateGrid() {
        return domElements.tblMonitor.DataTable({
            serverSide: false,
            ajax: {
                url: common.modifyRelativeUrl('MatchMonitor/GetMatchMonitor'),
                data:
                {
                    FromDate: function () { return domElements.dpkFromDate.val(); },
                    ToDate: function () { return domElements.dpkToDate.val(); },
                    BetTypes: function () { return domElements.cbbBetType.val().join(); },
                    IsVerified: function () { return domElements.cbbIsVerified.val(); }
                },
                type: "POST",
                beforeSend: function () {
                    domElements.tblMonitor.find('tbody').addClass('loading');
                },
                complete: function () {
                    domElements.tblMonitor.find('tbody').removeClass('loading');
                },
                error: function (error) {
                    domElements.tblMonitor.find('tbody').removeClass('loading');
                    console.log(error);
                }
            },
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            pagingType: "full_numbers",
            language: {
                lengthMenu: "Page size _MENU_",
                paginate: {
                    first: "<span class=\"icon-chevron-left\"></span><span class=\"icon-chevron-left\"></span>",
                    last: "<span class=\"icon-chevron-right\"></span><span class=\"icon-chevron-right\"></span>",
                    next: "",
                    previous: ""
                },
                info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            ordering: false,
            searching: false,
            lengthMenu: [20, 50, 100, 200],
            pageLength: 50,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6], className: "text-center" },
                { targets: [0], width: "20%" },
                { targets: [1], width: "10%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "10%" },
                { targets: [6], width: "10%" },                 
                { targets: [7], width: "5%" },
                { targets: [8], visible: false },
            ],
            columns: [
                {
                    data: obj => () => {
                        let paramTemplate = "EventDateToString={0}&MatchID={1}&HomeID={2}&AwayID={3}&BetTypeID={4}&EventGroup={5}&LiveIndicator={6}&IsHedging={7}";
                        let para = paramTemplate.format(obj.EventDateToString, obj.MatchID, obj.HomeID, obj.AwayID, obj.BettypeID, obj.EventGroup, obj.Market == 1, obj.IsHedging);
                        return '<a class="click-view-details" href="MatchMonitor/Detail?' + para + '" target="_blank">' + obj.MatchName + '</a>';

                        //let url = common.modifyRelativeUrl(`MatchMonitor/Detail?eventDataJson=${obj.EventDateToString}&matchID=${obj.MatchID}&matchNameJson=JSON.stringify(${obj.MatchName})&betTypeID=${obj.BettypeID}`);
                        //return html = `<a class="click-view-details" href=${url} target="_blank">${obj.MatchName} </a>`; 
                    }
                },
                { data: obj => obj.EventDateToString },                
                { data: obj => obj.MarketName },
                { data: obj => obj.LeagueName },
                { data: obj => obj.ReasonGroup },
                { data: obj => obj.VerifiedBy }, //PIC
                { data: obj => obj.VerifiedDateToString },
                {
                    data: obj => RenderVerifiedUI(obj.IsVerified), createdCell: (cell, _, row) => {
                        $('#isVerifiedCheckbox', cell).click(() => {
                            common.showConfirmation('<div class="popup - content">Are you sure about this?<br>Once confirmed, match review status will be updated to "Verified".</div>', function () {
                                UpdateVerification(row, cell);
                            });
                        });
                    }
                },
                { data: obj => obj.EventGroupName },
            ],
            drawCallback: function () {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var data = api.rows({ page: 'current' }).data();
                var last = null;

                api.column(8, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        $(rows).eq(i).before(
                            '<tr class="group group group-site"><td colspan="8">' + data[i].EventGroupName + '</td></tr>'
                        );

                        last = group;
                    }
                });
            }
        });
    }

    function RenderVerifiedUI(isVerified) {
        return html = '<label class="container-checkbox" id="containerCheckbox"><input id="isVerifiedCheckbox" type="checkbox"' + (isVerified ? ' checked disabled' : '') + '><span class="checkmark"></span></label>';                         
    }

    function UpdateVerification(row, cell) {
        $.ajax(common.modifyRelativeUrl('MatchMonitor/UpdateVerification'), {
            type: 'POST',
            data: { matchID: row.ID },          beforeSend: function () {
                domElements.tblMonitor.addClass('loading-value');
            },
            success: function (response) {
                GetMatchMonitor(); 
            },
            complete: function () {
                domElements.tblMonitor.removeClass("loading-value");
            },
            error: function (errMsg) {
                console.log(errMsg);
                domElements.tblMonitor.removeClass("loading-value");
            }
        });
    }

    function SetRestrictFromDate() {
        let toDateValue = domElements.dpkToDate.datepicker("getDate");

        domElements.dpkFromDate.datepicker("option", "maxDate", toDateValue);
    }

    function SetRestrictToDate() {
        let fromDateValue = domElements.dpkFromDate.datepicker("getDate");

        domElements.dpkToDate
            .datepicker("option", "minDate", fromDateValue)
            .datepicker("option", "maxDate", moment(fromDateValue).add(1, "year").toDate());
    }

    function RefreshChange($target) {
        domElements.eventTarget = $target
        var selectedVal = parseInt($target.getAttribute("value"));

        clearTimeout(timeoutHolder);
        StartCountingDown(domElements.btnRefresh, selectedVal, selectedVal);
    }

    function StartCountingDown($target, originalVal, val) {
        if (isNaN(originalVal)|| originalVal == -1) {
            domElements.btnRefresh.text("None");
        }
        else {
            domElements.btnRefresh.text("Refresh(" + val + ")");

            if (val !== 0) {
                timeoutHolder = setTimeout(() => StartCountingDown($target, originalVal, val - 1), 1000);
            }
            else {
                domElements.btnGenerate.click();
                StartCountingDown($target, originalVal, originalVal);
            }
        }
    }

    function Initial() {
        domElements.dpkFromDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true
            })
            .datepicker("setDate", new Date())
            .change(SetRestrictToDate);

        domElements.dpkToDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true
            })
            .datepicker("setDate", new Date())
            .change(SetRestrictFromDate);

        domElements.cbbBetType.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Bet Type"
        });

        domElements.groupDropDown.hover(function () {
            domElements.refreshList.toggleClass('block-box');
        })

        domElements.btnGenerate.on('click', function () { GetMatchMonitor(); });
        domElements.btnRefresh.on('click', function () {
            GetMatchMonitor();
            RefreshChange(domElements.eventTarget);
        });

        domElements.refreshList.find("li").on('click', function () { RefreshChange(event.target); })

        RefreshChange(domElements.eventTarget);
    }

    $(document).ready(function () {
        Initial();
    });
}

$(MatchMonitor);