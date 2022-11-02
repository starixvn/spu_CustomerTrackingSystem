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

                    HandleRobotBettingDetection(json.robotTypeClassify);

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

            let tdTicket = $('.robot-betting', tr);
            let tdReason = $('.robot-betting-reason', tr);

            if (tdTicket.length > 0) {
                LoadRobotDetail(tr, row);
            }
            else if (tdReason.length > 0) {
                LoadSCETicketDetail(tr, row);
            }            
        });
    }

    function HandleRobotBettingDetection(robotTypeClassify) {
        switch (robotTypeClassify) {
            case -1:
                $('span.icon-big-robot').removeClass('hide');
                $('span.icon-big-observation-robot').addClass('hide');
                $('span.icon-big-good-robot').addClass('hide');
                $('span.icon-big-bad-robot').addClass('hide');
                break;
            case 0:
                $('span.icon-big-robot').addClass('hide');
                $('span.icon-big-observation-robot').removeClass('hide');
                $('span.icon-big-good-robot').addClass('hide');
                $('span.icon-big-bad-robot').addClass('hide');
                break;
            case 1:
                $('span.icon-big-robot').addClass('hide');
                $('span.icon-big-observation-robot').addClass('hide');
                $('span.icon-big-good-robot').removeClass('hide');
                $('span.icon-big-bad-robot').addClass('hide');
                break;
            case 2:
                $('span.icon-big-robot').addClass('hide');
                $('span.icon-big-observation-robot').addClass('hide');
                $('span.icon-big-good-robot').addClass('hide');
                $('span.icon-big-bad-robot').removeClass('hide');
                break;
        }
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
                let periodrangetype = $('.robot-betting', tr).attr('data-periodrangetype');

                if (periodrangetype == 100 || periodrangetype == 101) {
                    LoadSCEReasonDetail(tr, row);
                } else {
                    LoadRobotTicketDetail(tr, row);
                }
            }
        }
    }

    function LoadRobotTicketDetail(tr, row) {
        let td = $('.robot-betting', tr);
        let custID = td.attr('data-custid');
        let periodrangetype = td.attr('data-periodrangetype');

        $.ajax(common.modifyRelativeUrl('Account/GetRobotBettingTicketDetail'), {
            type: 'POST',
            data: {
                custID: custID,
                periodrangetype: periodrangetype
            },
            success: function (response) {
                let childRow = $(NewDetailTicketRows(response));
                row.child(childRow).show();
                tr.addClass('detail-loaded');
            },
            beforeSend: function () {
                td.prop('disabled', true);
                robotBettingTable.find('tbody').addClass('loading');
            },
            complete: function () {
                td.prop('disabled', false);
                robotBettingTable.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                console.log(errMsg);
                td.prop('disabled', false);
                robotBettingTable.find('tbody').removeClass('loading');
            }
        });
    }

    function LoadSCEReasonDetail(tr, row) {
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
                let periodrangetype = td.attr('data-periodrangetype');

                $.ajax(common.modifyRelativeUrl('Account/GetRobotBettingReasonDetail'), {
                    type: 'POST',
                    data: {
                        custID: custID,
                        periodrangetype: periodrangetype
                    },
                    success: function (response) {
                        let childRow = $(NewDetailReasonRows(response));
                        row.child(childRow).show();

                        tr.addClass('detail-loaded');
                    },
                    beforeSend: function () {
                        td.prop('disabled', true);
                        robotBettingTable.find('tbody').addClass('loading');
                    },
                    complete: function () {
                        td.prop('disabled', false);
                        robotBettingTable.find('tbody').removeClass('loading');
                    },
                    error: function (errMsg) {
                        console.log(errMsg);
                        td.prop('disabled', false);
                        robotBettingTable.find('tbody').removeClass('loading');
                    }
                });
            }
        }
    }

    function LoadSCETicketDetail(tr, row) {
        let td = $('.robot-betting-reason', tr);
        let custID = td.attr('data-custid');
        let periodrangetype = td.attr('data-periodrangetype');
        let createdTime = td.attr('data-createdTime');

        $.ajax(common.modifyRelativeUrl('Account/GetRobotBettingReasonDetail'), {
            type: 'POST',
            data: {
                custID: custID,
                periodrangetype: periodrangetype,
                createdTime: createdTime
            },
            success: function (response) {
                let childRow = $(NewDetailReasonRows2(response));
                row.child(childRow).show();
                row.child(format()).show();

                tr.addClass('detail-loaded');
            },
            beforeSend: function () {
                td.prop('disabled', true);
                robotBettingTable.find('tbody').addClass('loading');
            },
            complete: function () {
                td.prop('disabled', false);
                robotBettingTable.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                console.log(errMsg);
                td.prop('disabled', false);
                robotBettingTable.find('tbody').removeClass('loading');
            }
        });
    }

    function NewDetailTicketRows(data) {
        let html = '';

        for (let i = 0; i < data.length; i++) {
            let item = data[i];

            html += `<tr>`;
            html += `<td class="text-center">${item.TransDate}</td>`;
            html += `<td class="text-center">${item.MatchName}</td>`;
            html += `<td class="text-center">${item.TransID}</td>`;
            html += `<td class="text-center">${item.BetTypeName}</td>`;
            html += `<td class="text-center">${item.BetTeam}</td>`;
            html += `<td class="text-center">${item.Stake.toLocaleString('en-US', { maximumFractionDigits: 2 })}</td>`;
            html += `<td class="text-center">${item.Odds}</td>`;
            html += `<td class="text-center">${item.LiveScore}</td>`;
            html += `</tr>`;
        }

       return html;
    }

    function NewDetailReasonRows(data) {
        let html = '';

        for (let i = 0; i < data.length; i++) {
            let item = data[i];

            switch (item.PeriodRangeType) {
                case 100:
                    html += `<tr>`;
                    html += `<td class="robot-betting-reason" colspan="8" data-custid=${item.CustID} data-periodrangetype=${item.PeriodRangeType} data-createdTime=${item.CreatedDateToString}>`;
                    html += `<span class="d"> ${item.FirstTransDateToString} - ${item.LastTransDateToString} (Total ${item.TotalHours} hours): <i class="title-reason-BetCount">${item.BetCount} </i> Tickets </span>`;
                    html += `</td >`;
                    html += `</tr>`;

                    break;
                case 101:
                    html += `<tr>`;
                    html += `<td class="robot-betting-reason" colspan = "8" data-custid=${item.CustID} data-periodrangetype=${item.PeriodRangeType} data-createdTime=${item.CreatedDateToString}>`;
                    html += `<span class=""><i class="title-reason-BetCount">${item.BetCount}/${item.TotalBetCount}</i> High Ugly Stake Tickets From ${item.FirstTransDateToString} To ${item.LastTransDateToString} </span>`;
                    html += `</td >`;
                    html += `</tr>`;

                    break;
            }
        }

        return html;
    }

    this.Initial = function (custID) {
        RenderGrid(custID);
    }
}