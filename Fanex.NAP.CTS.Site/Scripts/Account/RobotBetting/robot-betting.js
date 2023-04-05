function robot_betting() {
    let firstRobotDate = $('#spanFirstRobotDate');
    let lastRobotDate = $('#spanLastRobotDate');   
    let robotSCETable = $('#tableRobotBetting');
    let robotOCRDTable = $('#tblRobotOCRD');
    let robotImpervaTable = $('#tblImperva');

    function RenderGrid(custID) {
        robotSCETable.DataTable({
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            serverSide: false,
            ajax: {
                url: common.modifyRelativeUrl('Account/GetRobotBetting'),
                data: {
                    custID: custID
                },
                type: "POST",
                beforeSend: function () {
                    robotSCETable.find('tbody').addClass('loading');
                },
                dataFilter: function (response) {
                    var json = jQuery.parseJSON(response);

                    firstRobotDate.text("First Robot Date: " + json.firstRobotDate);
                    lastRobotDate.text("Last Robot Date: " + json.lastRobotDate);
                   
                    return response;
                },
                complete: function () {
                    robotSCETable.find('tbody').removeClass('loading');
                },
                error: function (error) {
                    robotSCETable.find('tbody').removeClass('loading');
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
                    let td = `<td class="robot-betting" colspan="8" data-custid=${obj.CustID} data-robotTypeOrigin=${obj.RangeType}>
                                    <span class="icon-chevron-right"><i class="title-robot">${obj.RangeName}</span>
                                  </td>`;
                    $(row).empty().html(td);
                }
            }
        });

        $('tbody', robotSCETable).on('click', 'tr', function () {
            var tr = $(this).closest('tr');
            var row = robotSCETable.DataTable().row(tr);

            let tdImperva = $('.imperva', tr);
            let tdTicket = $('.robot-betting', tr);

            if (tdTicket.length > 0 && tdImperva.length == 0) {
                LoadRobotDetail(tr, row);
            }          
        });
    }
  
    function LoadRobotDetail(tr, row) {
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
                let robotTypeOrigin = $('.robot-betting', tr).attr('data-robotTypeOrigin');

                if (robotTypeOrigin == 100 || robotTypeOrigin == 101) {
                    GenerateSCEReason(tr, row);
                } else {
                    GenerateTicketDetail(tr, row);
                }
            }
        }
    }

    function GenerateTicketDetail(tr, row) {
        let td = $('.robot-betting', tr);
        let custID = td.attr('data-custid');
        let robotTypeOrigin = td.attr('data-robotTypeOrigin');

        $.ajax(common.modifyRelativeUrl('Account/GetRobotBettingTicketDetail'), {
            type: 'POST',
            data: {
                custID: custID,
                robotTypeOrigin: robotTypeOrigin
            },
            success: function (response) {
                let childRow = NewDetailTicketRows(response);
                row.child(childRow).show();
                tr.addClass('detail-loaded');
            },
            beforeSend: function () {
                td.prop('disabled', true);
                robotSCETable.find('tbody').addClass('loading');
            },
            complete: function () {
                td.prop('disabled', false);
                robotSCETable.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                console.log(errMsg);
                td.prop('disabled', false);
                robotSCETable.find('tbody').removeClass('loading');
            }
        });
    }

    function GenerateSCEReason(tr, row) {
        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('shown');
        }
        else {
            if (tr.hasClass('detail-loaded')) {
                row.child.show();
            } else {

                let td = $('.robot-betting', tr);
                let custID = td.attr('data-custid');
                let robotTypeOrigin = td.attr('data-robotTypeOrigin');

                $.ajax(common.modifyRelativeUrl('Account/GetRobotBettingReasonDetail'), {
                    type: 'POST',
                    data: {
                        custID: custID,
                        robotTypeOrigin: robotTypeOrigin
                    },
                    success: function (response) {
                        if (response.reasonGroups.length  > 0) {
                            let childTable = CreateNewChildTable(response);

                            row.child(childTable).show();

                            RenderSCEReasonTable(response);
                            tr.addClass('detail-loaded');
                        }
                    },
                    beforeSend: function () {
                        td.prop('disabled', true);
                        robotSCETable.find('tbody').addClass('loading');
                    },
                    complete: function () {
                        td.prop('disabled', false);
                        robotSCETable.find('tbody').removeClass('loading');
                    },
                    error: function (errMsg) {
                        console.log(errMsg);
                        td.prop('disabled', false);
                        robotSCETable.find('tbody').removeClass('loading');
                    }
                });
            }
        }
    }

    function CreateNewChildTable(response) {
        var childTable = '<table id="childTable' + response.childTableId + '" class="display compact nowrap w-100" width="100%">' +
                         '<thead style="display:none"></thead >' +
                         '</table>';

        return $(childTable);

    }

    function RenderSCEReasonTable(data) {
        $('#childTable' + data.childTableId).DataTable({
            data: data.reasonGroups,
            columnDefs: [
                { targets: "_all", defaultContent: "-", className: "text-center" },
            ],
            columns: [],
            info: false,
            order: false,
            paging: false,
            searching: false,
            scrollX: false,
            scrollCollapse: false,
            language: {
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            createdRow: function (row, obj) {
                if (obj.length != 0) {
                    let td = RenderSCEReasonData(obj);

                    let tdChildTable = $(this).closest('td');

                    if (!tdChildTable.hasClass('nest-table')) {
                        tdChildTable.addClass('nest-table');
                    }

                    $(row).empty().html(td);
                }
            }
        });

        $('#childTable' + data.childTableId).removeClass('no-footer');

        $('tbody', $('#childTable' + data.childTableId)).on('click', 'tr', function () {
            var tr = $(this).closest('tr');
            var row = $('#childTable' + data.childTableId).DataTable().row(tr);
            let tdReason = $('.robot-betting-reason', tr);

            if (tdReason.length > 0) {
                GenerateSCETicketDetail(tr, row);
            }
        });
    }

    function RenderSCEReasonData(item) {
        let html;

        switch (item.RobotTypeDisplay) {
            case 100:
                html = `<td class="robot-betting-reason" colspan="8" data-custid=${item.CustID} data-robotTypeOrigin=${item.RobotTypeOrigin} data-robotTypeDisplay=${item.RobotTypeDisplay} data-clusterId=${item.ClusterID} data-createdTime=${item.CreatedDateToString}>`;
                html += `<span> ${item.FirstTransDateToString} - ${item.LastTransDateToString} (Total ${item.TotalHours} hours): <i class="title-reason">${item.BetCount} </i> Tickets </span>`;
                html += `</td >`;
                break;
            case 101:
                html = `<td class="robot-betting-reason" colspan="8" data-custid=${item.CustID} data-robotTypeOrigin=${item.RobotTypeOrigin}  data-robotTypeDisplay=${item.RobotTypeDisplay} data-clusterId=${item.ClusterID} data-createdTime=${item.CreatedDateToString}>`;
                html += `<span><i class="title-reason">${item.BetCount}/${item.TotalBetCount}</i> High Ugly Stake Tickets From ${item.FirstTransDateToString} To ${item.LastTransDateToString} </span>`;
                html += `</td >`;
                break;
        }

        return html;
    }

    function GenerateSCETicketDetail(tr, row) {
        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('shown');
        }
        else {
            if (tr.hasClass('detail-loaded')) {
                row.child.show();
            } else {
                let td = $('.robot-betting-reason', tr);
                let custID = td.attr('data-custid');
                let robotTypeOrigin = td.attr('data-robotTypeOrigin');
                let robotTypeDisplay = td.attr('data-robotTypeDisplay');
                let createdTime = td.attr('data-createdTime');
                let clusterId = td.attr('data-clusterId');

                $.ajax(common.modifyRelativeUrl('Account/GetRobotBettingTicketDetail'), {
                    type: 'POST',
                    data: {
                        custID: custID,
                        robotTypeOrigin: robotTypeOrigin,
                        robotTypeDisplay: robotTypeDisplay,
                        clusterId: clusterId,
                        createdTime: createdTime
                    },
                    success: function (response) {
                        if (response.length > 0) {
                            let childRow = NewDetailTicketRows(response);
                            row.child(childRow).show();
                            tr.addClass('detail-loaded');
                        }
                    },
                    beforeSend: function () {
                        td.prop('disabled', true);
                        robotSCETable.find('tbody').first().addClass('loading');
                    },
                    complete: function () {
                        td.prop('disabled', false);
                        robotSCETable.find('tbody').first().removeClass('loading');
                    },
                    error: function (errMsg) {
                        console.log(errMsg);
                        td.prop('disabled', false);
                        robotSCETable.find('tbody').first().removeClass('loading');
                    }
                });
            }
        }      
    }

    function NewDetailTicketRows(data) {
        let html = ''; 

        for (let i = 0; i < data.length; i++) {
            let item = data[i];

            html += `<tr class='robotBetting-childRow'>`;
            html += `<td class="td1 text-center">${item.TransDate}</td>`;
            html += `<td class="td2 text-center">${item.MatchName}</td>`;
            html += `<td class="td3 text-center">${item.TransID}</td>`;
            html += `<td class="td4 text-center">${item.BetTypeName}</td>`;
            html += `<td class="td5 text-center">${item.BetTeam}</td>`;
            html += `<td class="td6 text-center">${item.Stake.toLocaleString('en-US', { maximumFractionDigits: 2 })}</td>`;
            html += `<td class="text-center">${item.Odds}</td>`;
            html += `<td class="td7 text-center">${item.LiveScore}</td>`;
            html += `</tr>`;
        }

        return $(html);
    }

    function GenerateGridRobotSCE() {
        let table = robotSCETable.DataTable({
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            serverSide: false,
            language: {
                lengthMenu: "",
                info: "",
                infoEmpty: "",
                zeroRecords: "",
                emptyTable: "There's no data"
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
                    let td = `<td class="robot-betting" colspan="8" data-custid=${obj.CustID} data-robotTypeOrigin=${obj.RangeType}>
                                    <span class="icon-chevron-right"><i class="title-robot">${obj.RangeName}</span>
                                  </td>`;
                    $(row).empty().html(td);
                }
            }
        });

        $('tbody', robotSCETable).on('click', 'tr', function () {
            var tr = $(this).closest('tr');
            var row = robotSCETable.DataTable().row(tr);
          
            let tdTicket = $('.robot-betting', tr);

            if (tdTicket.length > 0) {
                LoadRobotDetail(tr, row);
            }
        });

        return table;
    }

    function GenerateGridRobotOCRD() {
        let table = robotOCRDTable.DataTable({
            language: {
                lengthMenu: "",
                info: "",
                infoEmpty: "",
                zeroRecords: "",
                emptyTable: "There's no data"
            },
            info: false,
            ordering: false,
            searching: false,
            scrollX: false,
            scrollCollapse: true,
            paging: false,
            searching: false,
            columnDefs: [
                { "visible": false, "targets": 0 },
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7], className: "text-center" },
                { targets: [0], width: "5%" },
                { targets: [1], width: "13%" },
                { targets: [2], width: "34%" },
                { targets: [3], width: "10%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "7%" },
                { targets: [6], width: "7%" },
                { targets: [7], width: "9%" }
            ],
            columns: [
                { data: obj => obj.TransDate },                
                { data: obj => obj.TransID },
                { data: obj => obj.IP },
                { data: obj => obj.BetType },
                { data: obj => obj.BetTeam },
                { data: obj => obj.Stake },
                { data: obj => obj.Odds },
                { data: obj => obj.LiveScore }
            ],
            createdRow: function (row, obj) {
                if (obj.Detail.length == 0) {
                    //if (obj.IsRobotImperva) {
                    //    let td = `<td class="robot-betting imperva" colspan="8">`;
                    //    td += `<span>Imperva -`;

                    //    $.each(obj.RobotImpervas, function (index, value) {
                    //        let cssClass = index == 0 ? 'title-robot' : 'title-robot imperva-line';
                    //        td += `<i class="${cssClass}"> CreatedDate: ${value.CreatedDate} - Platform: ${value.PlatformName}</i><br/>`;
                    //    });

                    //    td += `</span></td>`;

                    //    $(row).empty().html(td);
                    //} else {
                    //    let td = `<td class="robot-betting" colspan="8" data-custid=${obj.CustID} data-robotTypeOrigin=${obj.RangeType}>
                    //                <span class="icon-chevron-right"><i class="title-robot">${obj.RangeName}</span>
                    //              </td>`;
                    //    $(row).empty().html(td);
                    //}

                    let td = `<td class="robot-betting robot-ocrd" colspan="8" data-custid=${obj.CustID} data-rangetype=${obj.RangeType}>
                                    <span class="icon-chevron-right"><i class="title-robot">${obj.RangeName}</span>
                                  </td>`;
                        $(row).empty().html(td);
                }
            },
            drawCallback: function (settings) { }
        });

        $('tbody', robotOCRDTable).on('click', 'tr', function () {
            var tr = $(this).closest('tr');
            var row = robotOCRDTable.DataTable().row(tr);
            
            let tdTicket = $('.robot-ocrd', tr);

            if (tdTicket.length > 0) {
                GetRobotOcrdMatch(tr, row);
            }
        });

        return table;
    }

    function GetRobotOcrdMatch(tr, row) {
        let span = $('span', tr);

        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('shown');

            span.removeClass('icon-chevron-down')
                .addClass('icon-chevron-right');
        } else {
            span.removeClass('icon-chevron-right')
                .addClass('icon-chevron-down');

            if (tr.hasClass('detail-loaded')) {
                row.child.show();
            } else {
                let td = $('.robot-betting', tr);
                let custID = td.attr('data-custid');

                $.ajax(common.modifyRelativeUrl('Account/GetRobotOCRDMatch'), {
                    type: 'POST',
                    data: {
                        custID: custID
                    },
                    success: function (response) {
                        let childRow = NewRowOrcdMatch(response.data);
                        row.child(childRow).show();
                        tr.addClass('match-loaded');

                        $('.robot-ocrd-match-td', robotOCRDTable).on('click', function () {
                            console.log(this);
                        });
                    },
                    beforeSend: function () {
                        td.prop('disabled', true);
                        robotOCRDTable.find('tbody').addClass('loading');
                    },
                    complete: function () {
                        td.prop('disabled', false);
                        robotOCRDTable.find('tbody').removeClass('loading');
                    },
                    error: function (errMsg) {
                        console.log(errMsg);
                        td.prop('disabled', false);
                        robotOCRDTable.find('tbody').removeClass('loading');
                    }
                });
            }
        }        
    }

    function GetRobotOcrdMatch_backup(tr, row) {
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

                $.ajax(common.modifyRelativeUrl('Account/GetRobotOCRDMatch'), {
                    type: 'POST',
                    data: {
                        custID: custID                  
                    },
                    success: function (response) {
                        let childRow = NewRowOrcdMatch(response);
                        row.child(childRow).show();
                        tr.addClass('detail-loaded');
                    },
                    beforeSend: function () {
                        td.prop('disabled', true);
                        robotOCRDTable.find('tbody').addClass('loading');
                    },
                    complete: function () {
                        td.prop('disabled', false);
                        robotOCRDTable.find('tbody').removeClass('loading');
                    },
                    error: function (errMsg) {
                        console.log(errMsg);
                        td.prop('disabled', false);
                        robotOCRDTable.find('tbody').removeClass('loading');
                    }
                });
            }
        }
    }

    function NewRowOrcdMatch(data) {
        let html = '';

        for (let i = 0; i < data.length; i++) {
            let item = data[i];

            html += `<tr class='robot-ocrd-match'>`;
            html += `<td class="robot-betting robot-ocrd-match-td" colspan="8" data-custid=${item.CustID} data-matchid=${item.MatchID}>
                        <span class="icon-chevron-right">${item.MatchName} - ${item.EventDate}: ${item.TotalTicket}</span>
                    </td>`;
            html += `</tr>`;
        }            

        return $(html);
    }

    function GenerateRobotImperva(rbImpervas) {
        if (rbImpervas.length > 0) {
            let td = `<td class="robot-betting imperva" colspan="8">`;
            td += `<span>Imperva -`;

            $.each(rbImpervas, function (index, value) {
                let cssClass = index == 0 ? 'title-robot' : 'title-robot imperva-line';
                td += `<i class="${cssClass}"> CreatedDate: ${value.CreatedDate} - Platform: ${value.PlatformName}</i><br/>`;
            })
            td += `</span></td>`;

            let tr = `<tr>${td}</tr>`;
          
            $('tbody', robotImpervaTable).append(tr);  
        }        
    }

    function GetRobotBetting(custID) {
        $.ajax(common.modifyRelativeUrl('Account/GetRobotBetting'), {
            type: "POST",
            data: {
                custID: custID
            },
            beforeSend: function () {
                robotSCETable.find('tbody').addClass('loading');
            },
            success: function (response) {
                if (response.data.FirstRobotDate != '' && response.data.LastRobotDate != '') {
                    firstRobotDate.text("First Robot Date: " + response.data.FirstRobotDate);
                    lastRobotDate.text("Last Robot Date: " + response.data.LastRobotDate);
                }

                robotSCETable.DataTable().clear().rows.add(response.data.RobotSCE).draw();
                robotOCRDTable.DataTable().clear().rows.add(response.data.RobotOCRD).draw();       

                GenerateRobotImperva(response.data.RobotImpervas);
            },
            complete: function () {
                robotSCETable.find('tbody').removeClass('loading');
            },
            error: function (error) {
                robotSCETable.find('tbody').removeClass('loading');
                console.log(error);
            }
        });
    }

    this.Initial = function (custID) {
        GenerateGridRobotSCE();
        GenerateGridRobotOCRD();
        
        GetRobotBetting(custID);
    }
}