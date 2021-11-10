function robot_betting() {
    let firstRobotDate = $('#spanFirstRobotDate');
    let lastRobotDate = $('#spanLastRobotDate');
    let robotBettingTable = $('#tableRobotBetting');

    function RenderGrid(custID) {
        robotBettingTable.DataTable({
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            serverSide: true,
            ajax: {
                url: common.modifyRelativeUrl('Account/GetRobotBetting'),
                data: {
                    custID: custID
                },
                type: "POST",
                beforeSend: function () {
                    robotBettingTable.find('tbody').addClass('loading');
                },
                dataFilter: function (response) {
                    var json = jQuery.parseJSON(response);

                    firstRobotDate.text("First Robot Date: " + json.firstRobotDate);
                    lastRobotDate.text("Last Robot Date: " + json.lastRobotDate);

                    return response;
                },
                complete: function () {
                    robotBettingTable.find('tbody').removeClass('loading');
                },
                error: function (error) {
                    robotBettingTable.find('tbody').removeClass('loading');
                    console.log(error);
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
                { data: obj => obj.TransDate },
                { data: obj => obj.Match },
                { data: obj => obj.TransID },
                { data: obj => obj.BetType },
                { data: obj => obj.BetTeam },
                { data: obj => obj.Stake },
                { data: obj => obj.Odds },
                { data: obj => obj.LiveScore }
            ],
            createdRow: function (row, obj) {
                if (obj.Detail.length == 0) {
                    let td = `<td class="robot-betting" colspan="8"><span class="icon-chevron-right"><i class="title-robot">${obj.PeriodRangeName}</span></td>`;
                    
                    $(row).empty().html(td);
                }
            }
        });

        $('tbody', robotBettingTable).on('click', 'tr', function () {
            var tr = $(this);
            var row = robotBettingTable.DataTable().row(tr);
           
            if (row.child.isShown()) {
                row.child.hide();
                tr.removeClass('shown');
            }
            else {
                let childRow = $(NewChildRow());

                row.child(childRow).show();
                tr.addClass('shown');
            }
        });
    }

    function NewChildRow() {
        let htmlTr = '<tr>';
        htmlTr += '<td>1</td>';
        htmlTr += '<td>2</td>';
        htmlTr += '<td>3</td>';
        htmlTr += '<td>4</td>';
        htmlTr += '<td>5</td>';
        htmlTr += '<td>6</td>';
        htmlTr += '<td>7</td>';
        htmlTr += '<td>8</td>';
        htmlTr += '</tr>';

        return htmlTr;
    }

    function LoadDetail(row) {
        $.ajax(common.modifyRelativeUrl('Account/GetRobotBettingDetail'), {
            type: 'POST',
            data: {
                custID: 1773789,
                periodRangeType: 5
            },
            success: function (response) {
                console.log(response);
            },
            complete: function () {
               
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    this.Initial = function (custID) {
        RenderGrid(custID);
    }
}
