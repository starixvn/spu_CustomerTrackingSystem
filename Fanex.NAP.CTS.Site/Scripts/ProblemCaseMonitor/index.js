function ProblemCaseMonitor() {
    let dataTable;
    let domElements = {
        dpkFromDate: $('#dpkFromDate'),
        dpkToDate: $('#dpkToDate'),
        cbbProblemCases: $('#cbbProblemCases'),
        cbbSubscriber: $('#cbbSubscriber'),
        btnGenerate: $('#btnGenerate'),
        tblProblemCasesReport: $('#tblProblemCasesReport'),
        btnExportExcel: $('#btnExportExcel')
    };

    let exportPara = {
        FromDate: null,
        ToDate: null,
        ProblemCaseIds: null,
        SubscriberIds: null
    };

    function GetProblemCaseMonitor() {
        if (ValidateRangeDate()) {
            exportPara.FromDate = domElements.dpkFromDate.val();
            exportPara.ToDate = domElements.dpkToDate.val();
            exportPara.ProblemCaseIds = domElements.cbbProblemCases.val().join(",");
            exportPara.SubscriberIds = domElements.cbbSubscriber.val().join(",");

            dataTable
                ? dataTable.ajax.reload(null, true)
                : dataTable = GenerateGrid();
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
        return domElements.tblProblemCasesReport.DataTable({
            serverSide: false,
            ajax: {
                url: common.modifyRelativeUrl('ProblemCaseMonitor/GetProblemCasesMonitor'),
                data: {
                    FromDate: function () { return domElements.dpkFromDate.val(); },
                    ToDate: function () { return domElements.dpkToDate.val(); },
                    ProblemCaseIds: function () { return domElements.cbbProblemCases.val().join(); },
                    SubscriberIds: function () { return domElements.cbbSubscriber.val().join(); },
                },
                type: "POST",
                beforeSend: function () {
                    domElements.tblProblemCasesReport.find('tbody').addClass('loading');
                    domElements.btnGenerate.prop('disabled', true);
                },
                dataFilter: function (response) {
                      return response;
                },
                complete: function () {
                    domElements.tblProblemCasesReport.find('tbody').removeClass('loading');
                    domElements.btnGenerate.prop('disabled', false);
                },
                error: function (error) {
                    domElements.tblProblemCasesReport.find('tbody').removeClass('loading');
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
                { targets: [0, 1, 2, 3, 4], className: "text-center" },
                { targets: [0], width: "15%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" }
            ],
            columns: [
                {
                    data: obj => () => {
                        let url = common.modifyRelativeUrl('Account/Index/' + obj.CTSCustId);
                        return html = `<a class='click-view-name' href=${url}> ${obj.UserName} </a>`;
                    }
                },
                { data: obj => obj.SubscriberName },
                { data: obj => obj.RobotToString },
                { data: obj => obj.SuspiciousAssociationToString },
                { data: obj => obj.ProPunterToString}
            ]
        });
    }

    var ExportExcel = function () {
          common.downloadFilePostMethod(common.modifyRelativeUrl("ProblemCaseMonitor/ExportProblemCases"), exportPara);
    };

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

        domElements.cbbProblemCases.multiselect({
            menuWidth: 260,
            menuHeight: 260,
            selectedText: "# of # selected",
            noneSelectedText: "Select Problem Case"
        });

        domElements.cbbSubscriber.multiselect({
            menuWidth: 320,
            menuHeight: 320,
            selectedText: "# of # selected",
            noneSelectedText: "Select Subscriber",
            enableClickableOptGroups: true,
            includeSelectAllOption: true,
            groupsCollapsable: true,
            optGroupChecked: true
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
        });

        domElements.btnGenerate.on('click', function () { GetProblemCaseMonitor(); });

        domElements.btnExportExcel.on('click', function () { ExportExcel(); });
    }

    $(document).ready(function () {
        Initial();        
    });
}

$(ProblemCaseMonitor);