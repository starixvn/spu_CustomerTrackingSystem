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
        refresh: $('#refresh'),
        cbbRefresh: $('#cbbRefresh'),
        tblMonitor: $('#tblMonitor'),
        btnGenerate: $('#btnGenerate'),
        btnGenerateDetail: $('#btnGenerate-detail'),
        firsttblMonitor: $('#first-tblMonitor'),
        secondtblMonitor: $('#second-tblMonitor'),

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
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "15%" },
                { targets: [6], width: "15%" },
            ],
            columns: [
                {
                    data: obj => () => {
                        return html = `<a class="click-view-details">${obj.MatchName} </a>`;
                    }, createdCell: (cell, _, row) => {
                        $('.click-view-details', cell).click(() => {
                            alert(row, 'click view');
                        });
                    }
                },
                { data: obj => obj.EventDateToString },
                { data: obj => obj.LeagueName },
                { data: obj => obj.Reason },
                { data: obj => obj.VerifiedBy }, //PIC
                { data: obj => obj.VerifiedDateToString },
                {
                    data: obj => () => {
                        return html = '<label class="container-checkbox" id="containerCheckbox"><input id="overwriteCheckbox" type="checkbox"' + (obj.IsVerified ? ' checked' : '') +' ><span class="checkmark"></span></label>';                         
                    }
                }
            ]
        });
    }

    function FirstDetailGird() {
        var data = [];
        for (var i = 0; i < 500; i++) {
            data.push([i, i, i, i, i, i, i , i,i,i,i,i]);
        }
        return domElements.firsttblMonitor.DataTable({
            serverSide: false,
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
                { targets: [0, 1, 2, 3, 4, 5, 6 , 7 , 8 , 9 , 10 , 11], className: "text-center" },
                { targets: [0], width: "20%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "15%" },
                { targets: [6], width: "15%" },
                { targets: [7], width: "15%" },
                { targets: [8], width: "15%" },
                { targets: [9], width: "15%" },
                { targets: [10], width: "15%" },
                { targets: [11], width: "15%" },

            ],
            data: data,
            columns: [
                {
                    data: obj => () => {
                        return html = '<a class="click-view-details">Click Me</a>';
                    }, createdCell: (cell, _, row) => {
                        $('.click-view-details', cell).click(() => {
                            alert(row, 'click view');
                        });
                    }
                }
            ]
        });
    }
    function SecondDetailGird() {
        var data = [];
        for (var i = 0; i < 20; i++) {
            data.push([i, i, i, i, i, i, i, i, i, i, i, i]);
        }
        return domElements.secondtblMonitor.DataTable({
            serverSide: false,
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
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11], className: "text-center" },
                { targets: [0], width: "20%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "15%" },
                { targets: [6], width: "15%" },
                { targets: [7], width: "15%" },
                { targets: [8], width: "15%" },
                { targets: [9], width: "15%" },
                { targets: [10], width: "15%" },
                { targets: [11], width: "15%" },

            ],
            data: data,
            columns: [
                {
                    data: obj => () => {
                        return html = '<a class="click-view-details">Click Me</a>';
                    }, createdCell: (cell, _, row) => {
                        $('.click-view-details', cell).click(() => {
                            alert(row, 'click view');
                        });
                    }
                }
            ]
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
        domElements.cbbRefresh.find('option').each(function() {
            this.text = "Refresh(" + this.value + ")";
        });

        var selectedVal = parseInt($target.val());
        clearTimeout(timeoutHolder);
        StartCountingDown($target, selectedVal, selectedVal);
    }

    function StartCountingDown($target, originalVal, val) {
        $target.children("option:selected").text("Refresh(" + val + ")");

        if (val !== 0) {
            timeoutHolder = setTimeout(() => StartCountingDown($target, originalVal, val - 1), 1000);
        }
        else {
            domElements.btnGenerate.click();
            StartCountingDown($target, originalVal, originalVal);
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

        //////.mouseout(function () {
        //    if (!domElements.refreshList.is(event.target) && domElements.refreshList.has(event.target).length === 0)
        //    {
        //        domElements.refreshList.toggleClass('block-box');
        //    }
        //})

        ////domElements.refreshList.mouseout(function () {
        ////    if (!domElements.refresh.is(event.target)
        ////        && !domElements.refreshList.is(event.target)
        ////        && domElements.refresh.find("li").has(event.target).length === 0)
        ////    {
        ////        domElements.refreshList.toggleClass('block-box');
        ////    }
        ////})

        domElements.btnGenerate.on('click', function () { GetMatchMonitor(); });
        //domElements.btnGenerateDetail.on('click', function () { FirstDetailGird(); SecondDetailGird(); })

        domElements.cbbRefresh.val(30).change(e => RefreshChange($(e.target))).change();
    }

    $(document).ready(function () {
        Initial();
    });
}

$(MatchMonitor);