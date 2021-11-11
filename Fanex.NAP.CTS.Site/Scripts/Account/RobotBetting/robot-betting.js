function robot_betting() {
    let firstRobotDate = $('#spanFirstRobotDate');
    let lastRobotDate = $('#spanLastRobotDate');
    let robotBettingTable = $('#tableRobotBetting');

    function RenderGrid(custID) {
        robotBettingTable.DataTable({
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            serverSide: false,
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

                    HandleRobotDetection(json.data.length);                    

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
                    let td = `<td class="robot-betting" colspan="8" data-custid=${obj.CustID} data-periodrangetype=${obj.PeriodRangeType}><span class="icon-chevron-right"><i class="title-robot">${obj.PeriodRangeName}</span></td>`;
                    
                    $(row).empty().html(td);
                }
            }
        });

        $('tbody', robotBettingTable).on('click', 'tr', function () {
            var tr = $(this);
            var row = robotBettingTable.DataTable().row(tr);

            let td = $('.robot-betting', tr);

            if (td.length > 0) {
                LoadDetail(tr, row);  
            }
        });
    }   

    function HandleRobotDetection(dataLength) {
        if (dataLength > 0) {
            $('span.icon-big-robot').removeClass('hide');
        } else {
            $('span.icon-big-robot').addClass('hide');
        }       
    }
    
    function LoadDetail(tr, row) {
        let span = $('span', tr);

        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('shown');
            
            span.removeClass('icon-chevron-down')
                .addClass('icon-chevron-right');
        }
        else {
            span.removeClass('icon-chevron-right')
                .addClass('icon-chevron-down');           

            if (tr.hasClass('detail-loaded')) {               
                row.child.show();                
            } else {
                let td = $('.robot-betting', tr);
                let custID = td.attr('data-custid');
                let periodRangeType = td.attr('data-periodrangetype');

                $.ajax(common.modifyRelativeUrl('Account/GetRobotBettingDetail'), {
                    type: 'POST',
                    data: {
                        custID: custID,
                        periodRangeType: periodRangeType
                    },
                    success: function (response) {
                        let childRow = $(NewChildRows(response));

                        row.child(childRow).show();      

                        tr.addClass('detail-loaded');
                    },
                    beforeSend: function () {
                        td.prop('disabled', true);
                    },
                    complete: function () {
                        td.prop('disabled', false);
                    },
                    error: function (errMsg) {
                        console.log(errMsg);
                        td.prop('disabled', false);
                    }
                });          
            }              
        }        
    }

    function NewChildRows(data) {
        let html = '';

        for (let i = 0; i < data.length; i++) {
            let item = data[i];

            html += `<tr>`;
            html += `<td class="text-center">${item.TransDate}</td>`;
            html += `<td class="text-center">${item.MatchName}</td>`;
            html += `<td class="text-center">${item.TransID}</td>`;
            html += `<td class="text-center">${item.BetType}</td>`;
            html += `<td class="text-center">${item.BetTeam}</td>`;
            html += `<td class="text-center">${item.Stake}</td>`;
            html += `<td class="text-center">${item.Odds}</td>`;
            html += `<td class="text-center">${item.LiveScore}</td>`;
            html += `</tr>`;
        }

        return html;
    }

    this.Initial = function (custID) {
        RenderGrid(custID);
    }
}
