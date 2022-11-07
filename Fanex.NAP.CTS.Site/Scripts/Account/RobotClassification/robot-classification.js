function robot_classification() {    
    let robotClassificationTable = $('#lastest-robot-classification-table');
   
    function RenderGrid(custId) {
        let url = common.modifyRelativeUrl('Account/GetHistoryRobotClassification');

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
                { data: obj => obj.ScannedDate },
                { data: obj => obj.BettingStatistic },
                { data: obj => obj.RobotTypeName }               
            ]            
        });        
    }
    
    this.Initial = function (custId) {
        RenderGrid(custId);
    }
}