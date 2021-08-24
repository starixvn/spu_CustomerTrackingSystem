function robot_classification() {
    let btnViewHistory = $('#btnViewHistory');
    let btnSearchRobotClassHistory = $('#btnSearchRobotClassHistory');
    let robotClassificationTable = $('#lastest-robot-classification-table');
    let robotClassificationHistoryTable = $('#robot-classification-history-table');
    let divRobotClassHistory = $('#divRobotClassificationHistory');
    let dpkFromDate = $('#dpkFromDate');
    let dpkToDate = $('#dpkToDate');
    let historyDataTable;

    function RenderGrid(custId) {
        let url = common.modifyRelativeUrl('Account/GetLastestRobotClassification');

        robotClassificationTable.DataTable({
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            serverSide: true,
            ajax: {
                url: url,
                data: {
                    custId: custId
                },
                type: "POST",
                beforeSend: function () {
                    robotClassificationTable.find('tbody').addClass('loading');
                },
                complete: function () {
                    robotClassificationTable.find('tbody').removeClass('loading');
                }
            },
            language: {
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            info: false,
            ordering: false,
            paging: false,
            searching: false,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-", className: "text-center" },
            ],
            columns: [
                { data: obj => obj.Sport },
                { data: obj => obj.Bettype },
                { data: obj => obj.LiveIndicator },
                { data: obj => obj.RobotType },
                { data: obj => obj.ScannedDate }
            ]
        });
    }

    function RenderHistoryGrid(custId) {
        let url = common.modifyRelativeUrl('Account/GetRobotClassificationHistory');

        return robotClassificationHistoryTable.DataTable({
            dom: '<"wrapper-info-table"<"wrapper-info"lp>>t',
            serverSide: true,
            ajax: {
                url: url,
                data: {
                    custId: function () {
                        return custId;
                    },
                    fromDate: function () {
                        return dpkFromDate.val();
                    },
                    toDate: function () {
                        return dpkToDate.val();
                    }
                },
                type: "POST",
                beforeSend: function () {
                    robotClassificationHistoryTable.find('tbody').addClass('loading');
                },
                complete: function () {
                    robotClassificationHistoryTable.find('tbody').removeClass('loading');
                }
            },
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
            pagingType: "full_numbers",
            paging: true,
            ordering: false,
            searching: false,
            lengthMenu: [50, 100, 200, 500],
            pageLength: 50,
            scrollX: false,
            scrollCollapse: false,       
            columnDefs: [
                { targets: "_all", defaultContent: "-", className: "text-center" },
            ],
            columns: [
                { data: obj => obj.Sport },
                { data: obj => obj.Bettype },
                { data: obj => obj.LiveIndicator },
                { data: obj => obj.RobotType },
                { data: obj => obj.ScannedDate }
            ]
        });
    }

    this.Initial = function (custId) {
        RenderGrid(custId);

        dpkFromDate.datepicker({
            dateFormat: "M dd, yy",
            changeMonth: true,
            changeYear: true
        }).datepicker("setDate", new Date());

        dpkToDate.datepicker({
            dateFormat: "M dd, yy",
            changeMonth: true,
            changeYear: true
        }).datepicker("setDate", new Date());

        btnViewHistory.click(function () {
            if (historyDataTable == undefined) {
                historyDataTable = RenderHistoryGrid(custId);
            }

            divRobotClassHistory.dialog({
                modal: true,
                width: '700PX',
                title: 'SCAN HISTORY',
                resizable: true,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            });

            btnSearchRobotClassHistory.click(function () {
                if (historyDataTable) {
                    historyDataTable.ajax.reload(null, true);
                }
            });
        });
    }
}