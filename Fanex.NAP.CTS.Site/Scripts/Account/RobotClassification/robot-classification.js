function robot_classification() {
    let btnViewHistory = $('#btnViewHistory');
    let robotClassificationTable = $('#lastest-robot-classification-table');
    let robotClassificationHistoryTable = $('#robot-classification-history-table');
    let divRobotClassHistory = $('#divRobotClassificationHistory');
    let dpkFromDate = $('#dpkFromDate');
    let dpkToDate = $('#dpkToDate');

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

    function DisplayViewHistory(custId, fromDate, toDate) {
        let url = common.modifyRelativeUrl('Account/GetRobotClassificationHistory');

        robotClassificationHistoryTable.DataTable({
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            serverSide: true,
            ajax: {
                url: url,
                data: {
                    custId,
                    fromDate,
                    toDate
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
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            info: false,
            ordering: false,
            paging: true,
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
            divRobotClassHistory.dialog({
                modal: true,
                width: '700PX',
                title: 'SCAN HISTORY',
                resizable: true,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            });

            DisplayViewHistory(custId, dpkFromDate.val(), dpkToDate.val());
        });
    }
}