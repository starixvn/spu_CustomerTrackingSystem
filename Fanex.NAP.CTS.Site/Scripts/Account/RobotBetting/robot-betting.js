const RobotPeriodRangeType =
{
    SCEConsecutive: 100,
    SCEUglyStake: 101,
    OCRD: 200,
    TD: 300
}

function robot_betting() {
    let firstRobotDate = $('#spanFirstRobotDate');
    let lastRobotDate = $('#spanLastRobotDate');
    let robotSCETable = $('#tableRobotBetting');
    let robotRDTable = $('#tblRobotRD');
    let robotImpervaTable = $('#tblImperva');
    let robotLPTable = $('#tblBotLoginPattern');

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

                if (robotTypeOrigin == RobotPeriodRangeType.SCEConsecutive || robotTypeOrigin == RobotPeriodRangeType.SCEUglyStake) {
                    GenerateSCEReason(tr, row);
                } else {
                    GenerateTicketDetail(tr, row);
                }
            }
        }
    }

    function GenerateGridRobotLP() {
        let table = robotLPTable.DataTable({
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
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6], className: "text-center" },
            ],
            columns: [
                { data: obj => obj.TransDate },
                { data: obj => obj.Device },
                { data: obj => obj.RobotTracking },
                { data: obj => obj.DeviceType },
                { data: obj => obj.Browser },
                { data: obj => obj.IP },
                { data: obj => obj.Domain }               
            ],
            createdRow: function (row, obj) {
                let td = `<td class="robot-rd robot-lp-td" colspan="7" data-custid=${obj.CustID} data-rangetype=${obj.RangeType}>
                              <span class="icon-chevron-right"><i class="title-robot">${obj.RangeName}</i></span>
                              </td>`;
                $(row).empty().html(td);
            },
        });

        $('tbody', robotLPTable).on('click', 'tr', function () {
            let tr = $(this).closest('tr');
            let td = $('.robot-lp-td', tr);
           
            if (td.length > 0) {               
                GenerateRobotAILPDetail(tr, td);
            }
        });

        return table;
    }

    function GenerateRobotAILPDetail(tr, td) {    
        let span = $('span', tr);
        let custID = td.attr('data-custid');
        let rangeType = td.attr('data-rangetype');

        if (tr.hasClass('detail-loaded')) {            
            let isShow = true;

            if (span.hasClass('icon-chevron-right')) {
                span.removeClass('icon-chevron-right').addClass('icon-chevron-down');
            } else {
                span.removeClass('icon-chevron-down').addClass('icon-chevron-right'); isShow = false;                
            }

            $('tr[data-rangetype="' + rangeType + '"]', robotLPTable.find('tbody')).each(function (idx, ele) {
                if (isShow) {
                    $(ele).removeClass('hide');
                } else {
                    $(ele).addClass('hide');
                }
            })

        } else {                      
            $.ajax(common.modifyRelativeUrl('Account/GetRobotAILPDetail'), {
                type: 'POST',
                data: {
                    custID: custID,
                    rangeType: rangeType
                },
                success: function (response) {
                    let html = '';

                    for (const item of response.data) {                      
                        html += `<tr class='robot-ai-lp-childRow' data-rangetype=${rangeType}>`;
                        html += `<td class="td1 text-center">${item.TransDate}</td>`;
                        html += `<td class="td2 text-center">${item.Device}</td>`;
                        html += `<td class="td3 text-center">${item.RobotTracking}</td>`;
                        html += `<td class="td4 text-center">${item.DeviceType}</td>`;
                        html += `<td class="td5 text-center">${item.Browser}</td>`;
                        html += `<td class="td6 text-center">${item.IP}</td>`;
                        html += `<td class="td7 text-center">${item.Domain}</td>`;
                        html += `</tr>`;
                    }

                    $(tr).after(html);

                    tr.addClass('detail-loaded');
                },
                beforeSend: function () {
                    td.prop('disabled', true);
                    robotLPTable.find('tbody').addClass('loading');
                },
                complete: function () {
                    td.prop('disabled', false);
                    robotLPTable.find('tbody').removeClass('loading');
                },
                error: function (errMsg) {
                    td.prop('disabled', false);
                    robotLPTable.find('tbody').removeClass('loading');
                }
            });

            span.removeClass('icon-chevron-right').addClass('icon-chevron-down');
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
                        if (response.reasonGroups.length > 0) {
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
            case RobotPeriodRangeType.SCEConsecutive:
                html = `<td class="robot-betting-reason" colspan="8" data-custid=${item.CustID} data-robotTypeOrigin=${item.RobotTypeOrigin} data-robotTypeDisplay=${item.RobotTypeDisplay} data-clusterId=${item.ClusterID} data-createdTime=${item.CreatedDateToString}>`;
                html += `<span> ${item.FirstTransDateToString} - ${item.LastTransDateToString} (Total ${item.TotalHours} hours): <i class="title-reason">${item.BetCount} </i> Tickets </span>`;
                html += `</td >`;
                break;
            case RobotPeriodRangeType.SCEUglyStake:
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
                let td = `<td class="robot-betting" colspan="8" data-custid=${obj.CustID} data-robotTypeOrigin=${obj.RangeType}>
                                    <span class="icon-chevron-right"><i class="title-robot">${obj.RangeName}</i></span>
                                  </td>`;
                $(row).empty().html(td);
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

    function GenerateGridRobotRD() {
        let table = robotRDTable.DataTable({
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
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7], className: "text-center" },
            ],
            columns: [
                { data: obj => obj.TransDate },
                { data: obj => obj.TransID },
                { data: obj => obj.IP },
                { data: obj => obj.BetTypeName },
                { data: obj => obj.BetTeam },
                { data: obj => obj.Stake },
                { data: obj => obj.Odds },
                { data: obj => obj.LiveScore }
            ],
            createdRow: function (row, obj) {
                let td = `<td class="robot-rd robot-rd-td" colspan="8" data-custid=${obj.CustID} data-rangetype=${obj.RangeType}>
                              <span class="icon-chevron-right"><i class="title-robot">${obj.RangeName}</i></span>
                              </td>`;
                $(row).empty().html(td);
            },
        });

        $('tbody', robotRDTable).on('click', 'tr', function () {
            var tr = $(this).closest('tr');
            let tdTicket = $('.robot-rd-td', tr);

            if (tdTicket.length > 0) {
                GenerateRobotRDMatch(tr);
            }
        });

        return table;
    }

    function GenerateRobotRDMatch(tr) {
        let span = $('span', tr);
        let td = $('.robot-rd-td', tr);
        let rangeType = td.attr('data-rangetype');

        if (tr.hasClass('match-loaded')) {
            let isShow = true;

            if (span.hasClass('icon-chevron-right')) {
                span.removeClass('icon-chevron-right').addClass('icon-chevron-down');
            } else {
                span.removeClass('icon-chevron-down').addClass('icon-chevron-right');
                isShow = false;
            }

            let isShowChildRow = false;
            $('tr[data-rangetype="' + rangeType + '"]', robotRDTable.find('tbody')).each(function (idx, ele) {
                let spanCurrent = $('span', this);
                if (spanCurrent.hasClass('icon-chevron-down')) {
                    isShowChildRow = true;
                } else if (spanCurrent.hasClass('icon-chevron-right')) {
                    isShowChildRow = false;
                }
                if (isShow) {
                    if (spanCurrent.hasClass('icon-chevron-down') || spanCurrent.hasClass('icon-chevron-right')) {
                        $(ele).removeClass('hide');
                    } else if (isShowChildRow) {
                        $(ele).removeClass('hide');
                    }
                } else {
                    $(ele).addClass('hide');
                }
            })

        } else {
            GetRobotRDMatch(tr, rangeType);

            span.removeClass('icon-chevron-right').addClass('icon-chevron-down');
        }
    }

    function GetRobotRDMatch(tr, rangeType) {
        if (tr && rangeType) {
            let td = $('.robot-rd', tr);
            let custID = td.attr('data-custid');
            $.ajax(common.modifyRelativeUrl('Account/GetRobotRDMatch'), {
                type: 'POST',
                data: {
                    rangeType: rangeType,
                    custID: custID
                },
                success: function (response) {
                    let childRow = NewRowRDMatch(response.data, custID, rangeType);

                    $(tr).after(childRow);

                    tr.addClass('match-loaded');

                    $('tr[data-rangetype="' + rangeType + '"]', robotRDTable.find('tbody')).on('click', function () {
                        let trMatch = $(this).closest('tr');

                        GenerateRobotRDTicketDetail(trMatch);
                    });
                },
                beforeSend: function () {
                    td.prop('disabled', true);
                    robotRDTable.find('tbody').addClass('loading');
                },
                complete: function () {
                    td.prop('disabled', false);
                    robotRDTable.find('tbody').removeClass('loading');
                },
                error: function (errMsg) {

                    td.prop('disabled', false);
                    robotRDTable.find('tbody').removeClass('loading');
                }
            });
        }
    }

    function NewRowRDMatch(data, custID, rangeType) {
        let html = '';

        for (let i = 0; i < data.length; i++) {
            let item = data[i];

            html += `<tr class='robot-rd-match' data-rangetype=${rangeType}>
                        <td class="robot-rd robot-rd-match-td" colspan="8" data-custid=${custID} data-matchid=${item.MatchID} data-matchguid=${CreateGuid()}>
                            <span class="icon-chevron-right level2"><i class="title-robot">${item.MatchName} - ${item.EventDateStr}: total ${item.TotalTicket} tickets</i>
                            </span>
                        </td>
                    </tr>`;
        }

        return $(html);
    }

    function GenerateRobotRDTicketDetail(tr) {
        let span = $('span', tr);

        if (tr.hasClass('match-ticket-loaded')) {
            let td = $('.robot-rd-match-td', tr);
            let matchGuid = td.attr('data-matchguid');
            let isShow = true;

            if (span.hasClass('icon-chevron-right')) {
                span.removeClass('icon-chevron-right').addClass('icon-chevron-down');
            } else {
                span.removeClass('icon-chevron-down').addClass('icon-chevron-right');
                isShow = false;
            }

            $('tr[data-matchguid="' + matchGuid + '"]', robotRDTable.find('tbody')).each(function (idx, ele) {
                if (isShow) {
                    $(ele).removeClass('hide');
                } else {
                    $(ele).addClass('hide');
                }
            })

        } else {
            GetRobotRDTicketDetail(tr);

            span.removeClass('icon-chevron-right').addClass('icon-chevron-down');
        }
    }

    function GetRobotRDTicketDetail(tr) {
        let td = $('.robot-rd-match-td', tr);
        let custID = td.attr('data-custid');
        let matchID = td.attr('data-matchid');
        let matchGuid = td.attr('data-matchguid');
        let rangeType = tr.attr('data-rangetype');

        $.ajax(common.modifyRelativeUrl('Account/GetRobotRDTicketDetail'), {
            type: 'POST',
            data: {
                rangetype: rangeType,
                custID: custID,
                matchID: matchID
            },
            success: function (response) {
                let childRow = NewRowRDTicketDetail(response.data, rangeType, matchGuid);
                $(tr).after(childRow);

                tr.addClass('match-ticket-loaded');
            },
            beforeSend: function () {
                td.prop('disabled', true);
                robotRDTable.find('tbody').addClass('loading');
            },
            complete: function () {
                td.prop('disabled', false);
                robotRDTable.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {

                td.prop('disabled', false);
                robotRDTable.find('tbody').removeClass('loading');
            }
        });
    }

    function NewRowRDTicketDetail(data, rangeType, matchGuid) {
        let html = '';

        for (let i = 0; i < data.length; i++) {
            let item = ValidateValuesOfItemForDisplay(data[i]);

            html += `<tr class='robot-rd-match robot-rd-match-ticket robot-rd-body-td' data-rangetype=${rangeType} data-matchguid=${matchGuid}>
                        <td class="td.td1 text-center">${item.TransDate}</td>
                        <td class="td.td2 text-center">${item.TransID}</td>
                        <td class="td.td3 text-center">${item.IP}</td>
                        <td class="td.td4 text-center">${item.BetTypeName}</td>
                        <td class="td.td5 text-center">${item.BetTeam}</td>
                        <td class="td.td6 text-center">${item.Stake.toLocaleString('en-US', { maximumFractionDigits: 2 })}</td>
                        <td class="td.td7 text-center">${item.Odds}</td>
                        <td class="td.td8 text-center">${item.LiveScore}</td>
                    </tr>`;
        }

        return $(html);
    }

    function ValidateValuesOfItemForDisplay(item) {
        const result = { ...item };
        Object.keys(result).forEach(key => {
            if (result[key] === null || result[key] === "") {
                result[key] = "-";
            }
        });

        return result;
    }

    function GenerateRobotImperva(rbImpervas) {
        let td = `<td class="robot-imperva imperva" colspan="8">`;
        td += `<span>Imperva -`;

        $.each(rbImpervas, function (index, value) {
            let cssClass = index == 0 ? 'title-robot' : 'title-robot imperva-line';
            td += `<i class="${cssClass}"> CreatedDate: ${value.CreatedDate} - Platform: ${value.PlatformName}</i><br/>`;
        })
        td += `</span></td>`;

        let tr = `<tr role="row" class="odd">${td}</tr>`;

        $('tbody', robotImpervaTable).append(tr);
    }

    function CreateGuid() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }

    function GetRobotBetting(custID) {
        $.ajax(common.modifyRelativeUrl('Account/GetRobotBetting'), {
            type: "POST",
            data: {
                custID: custID
            },
            beforeSend: function () {
                robotSCETable.find('tbody').addClass('loading');
                robotSCETable.addClass('hide');
                robotRDTable.addClass('hide');
            },
            success: function (response) {
                if (response.data.FirstRobotDate != '' && response.data.LastRobotDate != '') {
                    firstRobotDate.text("First Robot Date: " + response.data.FirstRobotDate);
                    lastRobotDate.text("Last Robot Date: " + response.data.LastRobotDate);
                }

                if (response.data.RobotBettingAI.length == 0
                    && response.data.RobotImpervas.length == 0
                ) {
                    robotSCETable.DataTable().clear().rows.add([]).draw();
                    robotSCETable.removeClass('hide');
                } else {
                    const robotSCE = response.data.RobotBettingAI?.filter(item => item.RangeType <= RobotPeriodRangeType.SCEUglyStake) ?? [];
                    const robotOCRD = response.data.RobotBettingAI?.filter(item => item.RangeType == RobotPeriodRangeType.OCRD) ?? [];
                    const robotTD = response.data.RobotBettingAI?.filter(item => item.RangeType == RobotPeriodRangeType.TD) ?? [];
                    const robotImpervas = response.data.RobotImpervas;

                    if (response.data.RobotAILP.length > 0) {
                        robotLPTable.DataTable().clear().rows.add(response.data.RobotAILP).draw();
                    }

                    if (robotSCE.length > 0) {
                        robotSCETable.removeClass('hide');
                        robotSCETable.DataTable().clear().rows.add(robotSCE).draw();
                    }

                    robotRDTable.DataTable().clear()
                    if (robotOCRD.length > 0) {
                        robotRDTable.removeClass('hide');
                        robotRDTable.DataTable().rows.add(robotOCRD).draw();
                    }

                    if (robotTD.length > 0) {
                        robotRDTable.removeClass('hide');
                        robotRDTable.DataTable().rows.add(robotTD).draw();
                    }

                    if (robotImpervas.length > 0) {
                        GenerateRobotImperva(robotImpervas);
                    }
                }
            },
            complete: function () {
                robotSCETable.find('tbody').removeClass('loading');
            },
            error: function (error) {
                robotSCETable.find('tbody').removeClass('loading');
            }
        });
    }

    this.Initial = function (custID) {
        GenerateGridRobotLP();
        GenerateGridRobotSCE();
        GenerateGridRobotRD();

        GetRobotBetting(custID);
    }
}