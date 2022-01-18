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
        cbbSport: $('#cbbSport'),
        groupDropDown: $('.group-drop-down'),
        refreshList: $('#refreshList'),
        btnRefresh: $('#btnRefresh'),
        tblMonitor: $('#tblMonitor'),
        btnGenerate: $('#btnGenerate'),
        eventTarget: $('#selectedRefresh')[0],
        txtLeague: $('#txtLeague'),
        cbbMarket: $('#cbbMarket'),
        cbbMatchSection: $('#cbbMatchSection')
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
                    Sports: function () { return domElements.cbbSport.val().join(); },
                    BetTypes: function () { return "[" + domElements.cbbBetType.val().join() + "]" },
                    IsVerified: function () { return domElements.cbbIsVerified.val(); },
                    LeagueName: function () { return domElements.txtLeague.val(); },
                    Market: function () { return domElements.cbbMarket.val(); },
                    MatchSection: function () { return domElements.cbbMatchSection.val().join(); },
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
                { targets: [0], width: "32%" },
                { targets: [1], width: "9%" },
                { targets: [2], width: "9%" },
                { targets: [3], width: "7%" },
                { targets: [4], width: "28%" },
                { targets: [5], width: "15%" },                
                { targets: [6], width: "4%" },
                { targets: [7], visible: false },
            ],
            columns: [
                {
                    data: obj => () => {
                        let paramTemplate = "EventDateToString={0}&MatchID={1}&HomeID={2}&AwayID={3}&BetTypeID={4}&MatchSection={5}&Market={6}&Reason={7}&LeagueID={8}&BetID={9}&SportType={10}";
                        let para = paramTemplate.format(obj.EventDateToString, obj.MatchID, obj.HomeID, obj.AwayID, obj.BettypeID, obj.MatchSection, obj.Market, obj.Reason, obj.LeagueID, obj.BetID, obj.SportType);
                        return '<a class="click-view-details" href="MatchMonitor/Detail?' + para + '" target="_blank">' + obj.MatchName + '</a>';
                    }
                },
                { data: obj => obj.EventDateToString },
                { data: obj => obj.SportName },
                { data: obj => obj.MarketName },
                { data: obj => obj.LeagueName },
                { data: obj => obj.ReasonGroup },
                { data: obj => RenderVerifiedUI(obj.IsVerified) },
                { data: obj => obj.MatchSectionName },
            ],
            drawCallback: function () {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var data = api.rows({ page: 'current' }).data();
                var last = null;

                api.column(7, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        $(rows).eq(i).before(
                            '<tr class="group group group-site"><td colspan="8">' + data[i].MatchSectionName + '</td></tr>'
                        );

                        last = group;
                    }
                });
            }
        });
    }

    function RenderVerifiedUI(isVerified) {
        return '<label class="container-checkbox" id="containerCheckbox"><input id="isVerifiedCheckbox" type="checkbox" disabled' + (isVerified ? ' checked' : '') + '><span class="checkmark"></span></label>';
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

    function LoadBetTypes() {
        domElements.cbbBetType.find('option').remove();

        let sportTypes = domElements.cbbSport.val();
        let existedBetType = [];

        if (sportTypes != null) {
            sportTypes.forEach(c => {
                if (c == 1) {
                    var options1 = [{ text: 'HDP', value: RenderBetTypeHTML(1, 0) },
                    { text: 'O/U', value: RenderBetTypeHTML(3, 0) },
                    { text: '1st HDP', value: RenderBetTypeHTML(7, 0) },
                    { text: '1st O/U', value: RenderBetTypeHTML(8, 0) }
                    ];

                    options1.forEach(i => {
                        if (!existedBetType.includes(i.value)) {
                            domElements.cbbBetType.append(`<option value="${i.value}">${i.text}</option>`);
                            existedBetType.push(i.value);
                        }
                    })
                }
                else if (c == 2) {
                    var options2 = [{ text: 'HDP', value: RenderBetTypeHTML(1, 0) },
                    { text: 'O/U', value: RenderBetTypeHTML(3, 0) },
                    { text: '1st HDP', value: RenderBetTypeHTML(7, 0) },
                    { text: '1st O/U', value: RenderBetTypeHTML(8, 0) },
                    { text: 'Moneyline', value: RenderBetTypeHTML(20, 0) },
                    { text: 'Quarter 1 HDP', value: RenderBetTypeHTML(609, 801) },
                    { text: 'Quarter 2 HDP', value: RenderBetTypeHTML(609, 802) },
                    { text: 'Quarter 3 HDP', value: RenderBetTypeHTML(609, 803) },
                    { text: 'Quarter 4 HDP', value: RenderBetTypeHTML(609, 804) },
                    { text: 'Quarter 1 O/U', value: RenderBetTypeHTML(610, 801) },
                    { text: 'Quarter 2 O/U', value: RenderBetTypeHTML(610, 802) },
                    { text: 'Quarter 3 O/U', value: RenderBetTypeHTML(610, 803) },
                    { text: 'Quarter 4 O/U', value: RenderBetTypeHTML(610, 804) },
                    { text: 'Quarter 1 Moneyline', value: RenderBetTypeHTML(612, 801) },
                    { text: 'Quarter 2 Moneyline', value: RenderBetTypeHTML(612, 802) },
                    { text: 'Quarter 3 Moneyline', value: RenderBetTypeHTML(612, 803) },
                    { text: 'Quarter 4 Moneyline', value: RenderBetTypeHTML(612, 804) }
                    ];

                    options2.forEach(i => {
                        if (!existedBetType.includes(i.value)) {
                            domElements.cbbBetType.append(`<option value="${i.value}">${i.text}</option>`);
                            existedBetType.push(i.value);
                        }
                    })
                }
                else if (c == 3) {
                    var options3 = [{ text: 'HDP', value: RenderBetTypeHTML(1, 0) },
                    { text: 'O/U', value: RenderBetTypeHTML(3, 0) },
                    { text: 'Moneyline', value: RenderBetTypeHTML(20, 0) }
                    ];

                    options3.forEach(i => {
                        if (!existedBetType.includes(i.value)) {
                            domElements.cbbBetType.append(`<option value="${i.value}">${i.text}</option>`);
                            existedBetType.push(i.value);
                        }
                    })
                }
            })
        }

        domElements.cbbBetType.multiselect('refresh');
    }

    function RenderBetTypeHTML(betTypeID, betId) {
        return `{&quot;BettypeID&quot;:` + betTypeID + `, &quot;BetID&quot;:` + betId +`}`;
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

        domElements.cbbSport.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Sport"
        });

        domElements.cbbBetType.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Bet Type"
        });

        domElements.cbbMatchSection.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Match Section"
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

        domElements.cbbSport.on('change', function () { LoadBetTypes(); });
    }

    $(document).ready(function () {
        Initial();
    });
}

$(MatchMonitor);