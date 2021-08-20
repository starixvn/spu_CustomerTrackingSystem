function robot_classification() {
    var btnViewHistory = $('#btnViewHistory');
    var robotClassificationTable = $('#lastest-robot-classification-table');

    function RenderGrid(custId) {
        var url = common.modifyRelativeUrl('Account/GetLastestRobotClassification');

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

    this.Initial = function (custId) {
        RenderGrid(custId);

        btnViewHistory.on('click', function () {

            if (dataTable) {
                dataTable.ajax.reload(null, true);
            }
        });
    }
}