function AssociationDetection() {
    let dataTable;
    let tableAssociationDetectDetail = null;
    let tableAssociationDetectDetailIP = null;
    let tableAssociationDetectDetailMatch = null;
    let tableAssociationDetectDetailAgent = null;
    let oldData = null;
    let maxTags = 5000;    
    let domElements = {
        hdIsHostPermission: $('#hdIsHostPermission'),
        btnResultValidationClose: $('#btnResultValidationClose'),
        divReportContainer: $('#divReportContainer'),
        divAddUserName: $('#divAddUserName'),
        spanAddUserName: $('#spanAddUserName'),
        spanAddUserNameMaxTag: $('#spanAddUserNameMaxTag'),
        spanAddChangeHidden: $('#spanAddChangeHidden'),
        inputTagUsername: $('#inputTagUsername'),
        btnRemoveInvalidUserName: $('#btnRemoveInvalidUserName'),
        tickCheckAll: $('#tickCheckAll'),
        tickUncheckAll: $('#tickUncheckAll'),
        inputOptBP: $('#inputOptBP'),
        inputOptDevice: $('#inputOptDevice'),
        inputOptIP: $('#inputOptIP'),
        inputOptAgent: $('#inputOptAgent'),
        inputOptIPLast3Days: $('#inputOptIPLast3Days'),
        inputOpt3MatchesLast7Days: $('#inputOpt3MatchesLast7Days'),
        btnDetect: $('#btnAssociationDetection'),
        btnExportToExcel: $('#btnExportToExcel'),
        tblAssociationDetection: $('#tblAssociationDetection'),
        divAssociationDetectDetail: $('#divAssociationDetectDetail'),
        tblAssociationDetectDetail: $("#tblAssociationDetectDetail"),
        divAssociationDetectDetailIP: $('#divAssociationDetectDetailIP'),
        tblAssociationDetectDetailIP: $('#tblAssociationDetectDetailIP'),
        divAssociationDetectDetailMatch: $('#divAssociationDetectDetailMatch'),
        tblAssociationDetectDetailMatch: $('#tblAssociationDetectDetailMatch'),
        divAssociationDetectDetailAgent: $('#divAssociationDetectDetailAgent'),
        tblAssociationDetectDetailAgent: $('#tblAssociationDetectDetailAgent'),
        cbbCC: $('#cbbCC'),
        cbbDanger: $('#cbbDanger'), 
        txtBotTrans: $('#inputBotTrans'),
        txtInvalidBrowserTrans: $('#inputInvalidBrowserTrans'),
        btnFilterGenerate: $('#btnFilterGenerate')
    };

    let parameters = {
        UserNames: "",
        IsBP: false,
        IsDevice: false,
        IsIP: false,
        IsAgent: false,
        IsIPLast3Days: false,
        Is3MatchesLast7Days: false,      
    }

    function GenerateGrid(data) {
        let table = domElements.tblAssociationDetection.DataTable({
            language: {
                lengthMenu: "",
                info: "",
                infoEmpty: "",
                zeroRecords: "",
                emptyTable: "No Group from input list"
            },
            info: false,
            ordering: false,
            searching: false,
            scrollX: false,
            scrollCollapse: true,
            paging: false,
            searching: false,
            autoWidth: false,
            columnDefs: [
                { "visible": false, "targets": 0 },
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11], className: "text-center" },     
                { targets: [0], width: "0% " },
                { targets: [1], width: "12%" },
                { targets: [2], width: "2%" },
                { targets: [3], width: "11%" },
                { targets: [4], width: "3%" },
                { targets: [5], width: "12%" },
                { targets: [6], width: "6%" },
                { targets: [7], width: "10%" },
                { targets: [8], width: "8%" },
                { targets: [9], width: "16%" },
                { targets: [10], width: "10%" },
                { targets: [11], width: "9%" },           
            ],
            columns: [
                { data: obj => obj.GroupID },
                {
                    data: obj => () => {
                        let url = common.modifyRelativeUrl('Account/Index/' + obj.CTSCustID);
                        return `<a class='.view-profile-byusername' href=${url}>${obj.UserName}</a>`;
                    }               
                },
                { data: obj => obj.CustStatus },
                {
                    data: obj => common.renderCategoryName(obj.CustomerCategory), createdCell: (cell, _, row) => {
                        common.renderCategoryColor(cell, row);
                    }
                },
                { data: obj => obj.CustomerClassToString },
                { data: obj => obj.DangerDisplay },
                { data: obj => FormatTransInfo(obj.BotTransSum, obj.BotTransPercentage) },
                { data: obj => FormatTransInfo(obj.InvalidBrowserInfoTransSum, obj.InvalidBrowserInfoTransPercentage) },
                { data: obj => obj.Evidences },
                {
                    data: obj => obj.AssociatedTypes, createdCell: (cell, _, row) => {
                        $('.association-detect-match', cell).click(() => {
                            ShowAssociationDetectMatch(row);
                        });

                        $('.icon-Detail', cell).click(() => {
                            ShowAssociationDetectDetail(row);
                        });
                    }
                },
                {
                    data: obj => () => {
                        if (obj.AgentCTSCustID != 0) {
                            let url = common.modifyRelativeUrl('Account/Index/' + obj.AgentCTSCustID);
                            return `<a class='.view-profile-byusername' href=${url}>${obj.AgentUsername}</a>`;
                        } else {
                            return obj.AgentUsername;
                        }                        
                    } 
                },
                { data: obj => obj.AgentEvidence }
            ],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;

                api.column(0, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        $(rows).eq(i).before(
                            '<tr class="group"><td colspan="11"><span><i class="icon-chevron-down"></i>' + 'Group ' + group + '</span></td></tr>'
                        );

                        last = group;
                    }                   
                });
            }
        }).rows.add(data).draw();

        $('tbody', domElements.tblAssociationDetection).on('click', 'tr', function () {
            if ($(this).hasClass('group')) {
                let $next = $(this).next();
                while ($next.length && $next.is('[role=row]')) {
                    $next.slideToggle();
                    $next = $next.next();
                }

                let $i = $('i', $(this));
                if ($i.hasClass('icon-chevron-down')) {
                    $i.removeClass('icon-chevron-down')
                        .addClass('icon-chevron-right');
                } else {
                    $i.removeClass('icon-chevron-right')
                        .addClass('icon-chevron-down');
                }
            }
        });            
        return table;
    }    

    function ShowAssociationDetectDetail(fromObj) {       
        if (tableAssociationDetectDetail == null) {
            tableAssociationDetectDetail = domElements.tblAssociationDetectDetail.DataTable({
                language: {
                    lengthMenu: "",
                    info: "",
                    infoEmpty: "",
                    zeroRecords: "There's no data",
                    emptyTable: ""
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
                    { targets: [-2, -1, 0, 1, 2], className: "text-center" },
                    { targets: [0], width: "30%" },
                    { targets: [1], width: "30%" },
                    { targets: [2], width: "40%" }
                ],
                columns: [
                    { data: obj => obj.UserName },
                    { data: obj => obj.TransitiveUsername },
                    {
                        data: obj => obj.AssociatedTypes,
                        createdCell: (cell, _, row) => {
                            $('.association-detect-detail-ip', cell).click(() => {
                                let span = $(cell).find('.association-detect-detail-ip');
                                ShowAssociationDetectDetailIP(span.attr('data-fromusername'), row.UserName, span.attr('data-ips'));
                            });

                            $('.association-detect-detail-match', cell).click(() => {
                                let span = $(cell).find('.association-detect-detail-match');
                                ShowAssociationDetectDetailMatch(span.attr('data-fromusername'), row.UserName, parseInt(span.attr('data-fromcustid')), row.CustID);
                            });

                            $('.association-detect-detail-agent', cell).click(() => {
                                let span = $(cell).find('.association-detect-detail-agent');
                                ShowAssociationDetectDetailAgent(span.attr('data-fromusername'), row.UserName, row.AgentInfoJson);
                            });
                        }
                    }
                ]
            });
        }

        domElements.tblAssociationDetectDetail.DataTable().clear();

        let custIDs = [];
        let ctsCustIDs = [];
        let userNames = [];

        domElements.tblAssociationDetection.find('.view-detail').each(function () {
            if ($(this).attr("data-groupid") == fromObj.GroupID) {
                custIDs.push($(this).attr('data-custid'));
                ctsCustIDs.push($(this).attr('data-ctscustid'));
                userNames.push($(this).attr('data-username'));
            }
        });

        GetAssociationDetectDetail(fromObj.UserName, fromObj.CustID, fromObj.CTSCustID, custIDs.join(), ctsCustIDs.join(), userNames.join());

        domElements.divAssociationDetectDetail.dialog({
            modal: true,
            width: '600px',
            title: "ASSOCIATION DETAILS: " + fromObj.UserName,
            resizable: false,
            draggable: true,
            close: function () {
                $(this).dialog('close');                                
            }
        });             
        $('.ui-dialog .ui-dialog-content').css('padding', '12px');
        $('.ui-dialog .ui-dialog-content').css('margin-bottom', '0');
    }

    function GetAssociationDetectDetail(userName, custID, ctsCustID, custIDList, ctsCustIDList, userNames) {
        $.ajax(common.modifyRelativeUrl('AssociationDetection/GetDetectAssociationDetail'), {
            type: 'POST',
            beforeSend: function () {
                domElements.tblAssociationDetectDetail.find('tbody').addClass('loading');                
            },
            data: {
                UserName: userName,
                UserNameList: userNames,
                CustID: custID,
                CTSCustID: ctsCustID,
                CustIDList: custIDList,
                CTSCustIDList: ctsCustIDList,                               
                IsBP: parameters.IsBP,
                IsDevice: parameters.IsDevice,
                IsIP: parameters.IsIP,
                IsAgent: parameters.IsAgent,
                IsIPLast3Days: parameters.IsIPLast3Days,
                Is3MatchesLast7Days: parameters.Is3MatchesLast7Days              
            },
            success: function (response) {
                if (response !== undefined) {                   
                    domElements.tblAssociationDetectDetail.DataTable().rows.add(response.data).draw();
                    domElements.tblAssociationDetectDetail.DataTable().columns.adjust();
                }
            },
            complete: function () {
                domElements.tblAssociationDetectDetail.find('tbody').removeClass('loading');

            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);                
            }
        })
    }

    function ShowAssociationDetectDetailIP(userName1, userName2, ips) {                 
        if (tableAssociationDetectDetailIP == null) {
            tableAssociationDetectDetailIP = domElements.tblAssociationDetectDetailIP.DataTable({
                language: {
                    lengthMenu: "",
                    info: "",
                    infoEmpty: "",
                    zeroRecords: "",
                    emptyTable: ""
                },
                info: false,
                ordering: false,
                searching: false,
                scrollX: false,                 
                scrollCollapse: false,
                paging: false,
                searching: false,
                columnDefs: [
                    { targets: "_all", defaultContent: "-" },
                    { targets: [-1, 0 ], className: "text-center" },
                    { targets: [0], width: "100%" }                  
                ],
                columns: [
                    { data: obj => obj.IP }
                ]
            });
        }

        domElements.tblAssociationDetectDetailIP.DataTable().clear();

        GetAssociationDetectDetailIP(ips);

        domElements.divAssociationDetectDetailIP.dialog({
            modal: true,
            width: '500px',
            title: "",
            resizable: false,
            draggable: true,
            close: function () {
                $(this).dialog('close');               
            }
        });

        $('.ui-dialog .ui-dialog-content').css('padding', '12px');
        $('.ui-dialog .ui-dialog-content').css('margin-bottom', '12px');
        
        domElements.divAssociationDetectDetailIP.parent().find('.ui-dialog-title').html("<div>ASSOCIATION DETAIL - IP LIST (in the last 3 days): <br/>" + userName1 + " - " + userName2 + "</div>");      
    }

    function GetAssociationDetectDetailIP(ips) {
        $.ajax(common.modifyRelativeUrl('AssociationDetection/GetDetectAssociationDetailIP'), {
            type: 'POST',
            beforeSend: function () {
                domElements.tblAssociationDetectDetailIP.find('tbody').addClass('loading');                
            },
            data: {                  
                ips: ips
            },
            success: function (response) {
                if (response !== undefined) {
                    domElements.tblAssociationDetectDetailIP.DataTable().rows.add(response.data).draw();
                    domElements.tblAssociationDetectDetailIP.DataTable().columns.adjust();
                }
            },
            complete: function () {
                domElements.tblAssociationDetectDetailIP.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        })
    }

    function ShowAssociationDetectMatch(fromObj) {
        RenderGridMatch();

        let custIDs = [];        
        domElements.tblAssociationDetection.find('.association-detect-match').each(function () {
            if ($(this).attr("data-groupid") == fromObj.GroupID) {
                custIDs.push($(this).attr('data-custid'));
            }
        });

        GetAssociationDetectDetailMatch(fromObj.CustID, -1, custIDs.join());

        ShowDialogMatch(fromObj.UserName, "");
    }

    function ShowAssociationDetectDetailMatch(userName1, userName2, custID1, custID2) {               
        RenderGridMatch();
      
        GetAssociationDetectDetailMatch(custID1, custID2, "");
        
        ShowDialogMatch(userName1, userName2);
    }

    function RenderGridMatch() {
        if (tableAssociationDetectDetailMatch == null) {
            tableAssociationDetectDetailMatch = domElements.tblAssociationDetectDetailMatch.DataTable({
                language: {
                    lengthMenu: "",
                    info: "",
                    infoEmpty: "",
                    zeroRecords: "",
                    emptyTable: ""
                },
                info: false,
                ordering: false,
                searching: false,
                scrollX: false,
                scrollCollapse: false,
                paging: false,
                searching: false,
                columnDefs: [
                    { "visible": false, "targets": 0 },
                    { targets: "_all", defaultContent: "-" },
                    { targets: [-1, 0], className: "text-center" },
                    { targets: [0], width: "5%" },
                    { targets: [1], width: "15%" },
                    { targets: [2], width: "20%" },
                    { targets: [3], width: "10%" },
                    { targets: [4], width: "10%" },
                    { targets: [5], width: "10%" },
                    { targets: [6], width: "10%" },
                    { targets: [7], width: "10%" },
                    { targets: [8], width: "10%" },
                    { targets: [9], width: "10%" }
                ],
                columns: [
                    { data: obj => obj.MatchID },
                    { data: obj => obj.UserName },
                    { data: obj => obj.TransDate },
                    { data: obj => obj.TransID },
                    { data: obj => obj.BetTypeName },
                    { data: obj => obj.BetTeam },
                    { data: obj => obj.Stake },
                    { data: obj => obj.Odd },
                    { data: obj => obj.LiveScore },
                    { data: obj => obj.Result }
                ],
                drawCallback: function (settings) {
                    var api = this.api();
                    var rows = api.rows({ page: 'current' }).nodes();
                    var data = api.rows({ page: 'current' }).data();
                    var last = null;

                    api.column(0, { page: 'current' }).data().each(function (group, i) {
                        if (last !== group) {
                            $(rows).eq(i).before(
                                `<tr class="match"><td colspan="9">${data[i].MatchName} - ${data[i].EventDate} - ${data[i].LeagueName}</td></tr>`
                            );

                            last = group;
                        }
                    });
                }
            });
        }

        domElements.tblAssociationDetectDetailMatch.DataTable().clear();
    }

    function ShowDialogMatch(userName1, userName2) {
        let title = "ASSOCIATION DETAILS - MATCH (in the last 7 days): " + userName1;

        if (userName2 != "") {
            title += " - " + userName2;
        }

        domElements.divAssociationDetectDetailMatch.dialog({
            modal: true,
            width: '1050px',
            title: title,
            resizable: false,
            draggable: true,
            close: function () {
                $(this).dialog('close');                              
            }
        });

        $('.ui-dialog .ui-dialog-content').css('padding', '12px');
        $('.ui-dialog .ui-dialog-content').css('margin-bottom', '12px');
    }

    function GetAssociationDetectDetailMatch(custID1, custID2, custIDList) {
        $.ajax(common.modifyRelativeUrl('AssociationDetection/GetDetectAssociationDetailMatch'), {
            type: 'POST',
            beforeSend: function () {
                domElements.tblAssociationDetectDetailMatch.find('tbody').addClass('loading');                
            },
            data: {
                custID1: custID1,
                custID2: custID2,
                custIDList: custIDList
            },
            success: function (response) {
                if (response !== undefined) {                   
                    domElements.tblAssociationDetectDetailMatch.DataTable().rows.add(response.data).draw();
                    domElements.tblAssociationDetectDetailMatch.DataTable().columns.adjust();
                }
            },
            complete: function () {
                domElements.tblAssociationDetectDetailMatch.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        })
    }

    function ShowAssociationDetectDetailAgent(userName1, userName2, agentInfoJson) {                
        if (tableAssociationDetectDetailAgent === null) {
            tableAssociationDetectDetailAgent = domElements.tblAssociationDetectDetailAgent.DataTable({
                language: {
                    lengthMenu: "",
                    info: "",
                    infoEmpty: "",
                    zeroRecords: "",
                    emptyTable: ""
                },
                info: false,
                ordering: false,
                searching: false,
                scrollX: false,
                scrollCollapse: false,
                paging: false,
                searching: false,                
                columnDefs: [
                    { targets: "_all", defaultContent: "-" },
                    { targets: [-1, 0, 1, 2], className: "text-center" },
                    { targets: [0], width: "25%" },
                    { targets: [1], width: "25%" },
                    { targets: [2], width: "25%" },
                    { targets: [3], width: "25%" }
                ],
                columns: [
                    { data: obj => obj.UserName },
                    { data: obj => obj.AgentName },
                    { data: obj => obj.TransitiveUsername },
                    { data: obj => obj.AgentAssociationTypes }
                ]
            });
        }

        domElements.tblAssociationDetectDetailAgent.DataTable().clear().rows.add([]);

        GetAssociationDetectDetailAgent(agentInfoJson);

        domElements.divAssociationDetectDetailAgent.dialog({
            modal: true,
            width: '600px',
            title: "ASSOCIATION DETAILS - AGENT: " + userName1 + " - " + userName2,
            resizable: false,
            draggable: true,
            close: function () {
                $(this).dialog('close');                
            }
        });      

        $('.ui-dialog .ui-dialog-content').css('padding', '12px');
        $('.ui-dialog .ui-dialog-content').css('margin-bottom', '12px');
    }

    function GetAssociationDetectDetailAgent(agentInfoJson) {
        $.ajax(common.modifyRelativeUrl('AssociationDetection/GetDetectAssociationDetailAgent'), {
            type: 'POST',
            beforeSend: function () {
                domElements.tblAssociationDetectDetailAgent.find('tbody').addClass('loading');                
            },
            data: {
                agentInfoJson: agentInfoJson
            },
            success: function (response) {
                if (response !== undefined) {                                       
                    let tbody = domElements.tblAssociationDetectDetailAgent.find('tbody');

                    $(tbody).empty();

                    let items = response.data;
                                   
                    for (var i = 0; i < items.length; i++) {
                        if (i == 0) {
                            $(tbody).append($('<tr role="row" class="odd">')
                                .append('<td class="text-center">' + items[i]['UserName'] + '</td>')
                                .append('<td class="text-center">' + items[i]['AgentName'] + '</td>')
                                .append('<td rowspan="2" class="text-center">' + items[i]['TransitiveUsername'] + '</td>')
                                .append('<td rowspan="2" class="text-center">' + items[i]['AgentAssociationTypes'] + '</td>')
                                .append('</tr>'));
                        } else {
                            $(tbody).append($('<tr role="row" class="odd">')
                                .append('<td class="text-center">' + items[i]['UserName'] + '</td>')
                                .append('<td class="text-center">' + items[i]['AgentName'] + '</td>')                               
                                .append('</tr>'));
                        }                      
                    }

                    domElements.tblAssociationDetectDetailAgent.DataTable().columns.adjust();
                }
            },
            complete: function () {
                domElements.tblAssociationDetectDetailAgent.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        })
    }

    function CheckSelectFilter() {
        if (domElements.inputOptBP.is(":checked")) {
            domElements.btnDetect.removeClass('checkselectfilter-notpass'); return true;
        }     

        if (domElements.inputOptDevice.is(":checked")) {
            domElements.btnDetect.removeClass('checkselectfilter-notpass'); return true;
        }

        if (domElements.inputOptIP.is(":checked")) {
            domElements.btnDetect.removeClass('checkselectfilter-notpass'); return true;
        }

        if (domElements.inputOptAgent.is(":checked")) {
            domElements.btnDetect.removeClass('checkselectfilter-notpass'); return true;
        }           

        if (domElements.inputOptIPLast3Days.is(":checked")) {
            domElements.btnDetect.removeClass('checkselectfilter-notpass'); return true;
        }
            
        if (domElements.inputOpt3MatchesLast7Days.is(":checked")) {
            domElements.btnDetect.removeClass('checkselectfilter-notpass'); return true;
        }            

        domElements.btnDetect.addClass('checkselectfilter-notpass'); return false;
    }

    function ChangeStateBtnDetect() {
        if (domElements.btnDetect.hasClass('checkselectfilter-notpass')) {
            ValidateUsernames();
        } else if (!CheckSelectFilter()) {
            if (!domElements.btnDetect.is(':disabled')) {
                domElements.btnDetect.prop('disabled', true);
                domElements.btnDetect.addClass('disabled-by-selectoption');
            }
        } else if (domElements.btnDetect.hasClass('disabled-by-selectoption')) {
            domElements.btnDetect.removeClass('disabled-by-selectoption');
            domElements.btnDetect.prop('disabled', false);
        }
    }

    function ValidateUsernames(textTagRemove, textTagRemoveList) {
        let numOfTagExceeded = domElements.spanAddUserName.find('tags tag.tagify--notAllowed');

        if (textTagRemove != '' && numOfTagExceeded.length > 1) return;
        else if (textTagRemove == '' && numOfTagExceeded.length > 0) return;
      
        let userNames = GetInputUserName(textTagRemove, textTagRemoveList);

        if (userNames !== '' && CheckSelectFilter()) {
            let params = {};
            params.userNames = userNames;

            $.ajax(common.modifyRelativeUrl('AssociationDetection/ValidateUsernames'), {
                type: 'POST',
                data: params,
                beforeSend: function () {
                    domElements.spanAddUserName.addClass('loading-value');
                },
                success: function (response) {
                    if (response.IsException)
                        BindExceptionValidatedAccount();
                    else
                        BindResultValidatedAccounts(response);
                },
                complete: function () {
                    domElements.spanAddUserName.removeClass("loading-value");
                },
                error: function (errMsg) {
                    console.log(errMsg);
                }
            });
        } else {
            domElements.btnRemoveInvalidUserName.removeClass('delete-invalid');
        }
    }    

    function GetInputUserName(textTagRemove, textTagRemoveList) {
        let userNameList = [];
        let tags = domElements.spanAddUserName.find('tags tag');

        tags.map(function (_, ele) {
            let text = $(ele).text().trim();
            if (textTagRemoveList != undefined && textTagRemoveList.length > 0) {
                if (ContainsObject(text, textTagRemoveList))
                    return;
            } else if (textTagRemove !== "" && text === textTagRemove){
                textTagRemove = "";
                return;
            }
            
            userNameList.push(text);
        });

        return userNameList.join();
    }

    function ContainsObject(obj, list) {
        var i;
        for (i = 0; i < list.length; i++) {
            if (list[i] === obj) {
                return true;
            }
        }

        return false;
    } 
   
    function BindExceptionValidatedAccount() {
        domElements.spanAddUserName.find('tags tag').each(function (_, tag) {
            $(tag).removeClass('valid').addClass('invalid-username');
        });
    }

    function BindResultValidatedAccounts(data) {
        domElements.spanAddUserName.find('tags tag').each(function (_, tag) {
            $(tag).removeClass('invalid-username').addClass('valid');
        });

        data.UserNameInvalidList.map(function (x) {
            let usernameEqual = $("tag[data-user-name='" + x + "']");
            if (usernameEqual !== undefined) {
                usernameEqual.removeClass('valid').addClass('invalid-username');
                usernameEqual.prop('title', 'Invalid username');
            }
        });     

        let isEnableDetect = data.UserNameInvalidList.length === 0 && data.UserNameValidList.length >= 2;

        domElements.btnDetect.prop('disabled', !isEnableDetect);

        let invalidTags = domElements.spanAddUserName.find('.invalid-username', 'tags tag');
        if (invalidTags.length > 0) {
            domElements.btnRemoveInvalidUserName.addClass('delete-invalid');
        } else {
            domElements.btnRemoveInvalidUserName.removeClass('delete-invalid');
        }
    }

    function RemoveAllInvalidUsername() {
        if (domElements.btnRemoveInvalidUserName.hasClass('delete-invalid')) {
            common.showConfirmation("<div class='popup-content'><div class='popup-title'>Are you sure you want to remove all invalid Usernames?</div></div>",
                function () {                                   
                    let invalidTags = domElements.spanAddUserName.find('.invalid-username', 'tags tag');
                    invalidTags.each(function (idx, tag) {
                        domElements.inputTagUsername.tagify().data('tagify').removeTags(tag);

                        if (idx == invalidTags.length - 1)
                            RefreshRemoveAllInvalidUsername(invalidTags);                                       
                    });
                }
            )
        }
    }

    function RefreshRemoveAllInvalidUsername(invalidTags) {
        let maxCountTags = parseInt(domElements.spanAddUserNameMaxTag.text());

        domElements.spanAddUserNameMaxTag.text(maxCountTags + invalidTags.length);
        domElements.btnRemoveInvalidUserName.removeClass('delete-invalid');

        setTimeout(() => {
            let invalidUsernameList = [];
            invalidTags.map(function (_, ele) { invalidUsernameList.push($(ele).text().trim()); });

            ValidateUsernames(null, invalidUsernameList);
        }, 120);
    }

    function DetectUserName() {
        let userNames = GetInputUserName();

        if (userNames !== '') {
            parameters.UserNames = userNames;
            parameters.IsBP = domElements.inputOptBP.is(":checked");
            parameters.IsDevice = domElements.inputOptDevice.is(":checked");
            parameters.IsIP = domElements.inputOptIP.is(":checked");
            parameters.IsAgent = domElements.inputOptAgent.is(":checked");
            parameters.Is3MatchesLast7Days = domElements.inputOpt3MatchesLast7Days.is(":checked");
            parameters.IsIPLast3Days = domElements.inputOptIPLast3Days.is(":checked");            

            SetStorageFilter("AssDetectCheckboxBP", parameters.IsBP);
            SetStorageFilter("AssDetectCheckboxDevice", parameters.IsDevice);
            SetStorageFilter("AssDetectCheckboxIP", parameters.IsIP);
            SetStorageFilter("AssDetectCheckboxAgent", parameters.IsAgent);            
            SetStorageFilter("AssDetectCheckboxIP3Days", parameters.IsIPLast3Days);
            SetStorageFilter("AssDetectCheckbox3Matches", parameters.Is3MatchesLast7Days);

            $.ajax(common.modifyRelativeUrl('AssociationDetection/DetectAssociationByUserNames'), {
                type: 'POST',
                data: parameters,
                beforeSend: function () {
                    domElements.divAddUserName.addClass("loading-block");
                    domElements.btnDetect.prop('disabled', true);
                },
                success: function (response) {
                    if (response.data.ErrorCode == 0) {
                        SetupDangerFilter(response.filters.DangerFilter);
                        SetupCCFilter(response.filters.CustClassFilter);
                        SetupTextBox();

                        dataTable
                            ? domElements.tblAssociationDetection.DataTable().clear().rows.add(response.data.ItemList).draw()
                            : dataTable = GenerateGrid(response.data.ItemList);

                        oldData = response.data.ItemList;

                        domElements.tblAssociationDetection.DataTable().columns.adjust();

                        domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
                        domElements.spanAddUserNameMaxTag.text(maxTags);
                        domElements.spanAddChangeHidden.text("0");
                        domElements.btnDetect.prop('disabled', true);                       
                    } else {
                        common.showError(response.data.ErrorMessage);

                        dataTable
                            ? domElements.tblAssociationDetection.DataTable().clear().draw()
                            : dataTable = GenerateGrid([]);
                    }
              
                    domElements.divReportContainer.removeClass('hide');
                },
                complete: function () {
                    domElements.divAddUserName.removeClass("loading-block");              
                    domElements.btnDetect.prop('disabled', false);
                },
                error: function (errMsg) {
                    console.log(errMsg);
                    domElements.divAddUserName.removeClass("loading-block");             
                    domElements.btnDetect.prop('disabled', false);
                }
            });
        }
    }   

    function InitInputTagUsername() {
        domElements.spanAddUserNameMaxTag.text(maxTags);
        domElements.spanAddChangeHidden.text("0");

        function AddBorderTags() {
            domElements.spanAddUserName.find('tags').addClass('ctrlA');
            let allTags = domElements.spanAddUserName.find('tags tag.tagify__tag');
            allTags.each(function (_, tag) {
                $(tag).css('border', '1px solid #6187E7');
            });
        }

        function ResetMaxTag() {
            let currentTagsLength = domElements.spanAddUserName.find('tags tag').length;
            if (currentTagsLength < maxTags)
                domElements.spanAddUserNameMaxTag.text(maxTags - currentTagsLength + 1);
        }
       
        function IsTextUnicode(text) {
            var pattern = /^[\x20-\x7E]*$/;
            return pattern.test(text);
        }

        function OnChangeTags(tagName, cssClass) {
            let maxCountTags = parseInt(domElements.spanAddUserNameMaxTag.text());
            let isTextUnicode = IsTextUnicode($(tagName.tag).text());

            if (!isTextUnicode) {
                domElements.inputTagUsername.tagify().data('tagify').removeTags(tagName.tag);
            }
            else if (maxCountTags > 0) {
                let $tag = $(tagName.tag);
                let textReplaced = $tag.text();
                textReplaced = textReplaced.replace(new RegExp("'", 'g'), "").replace(new RegExp("\"", 'g'), "");
                if (textReplaced.length > 50)
                    textReplaced = textReplaced.substr(0, 50);
                $tag.val(textReplaced);
                $tag.attr('data-user-name', textReplaced);
                $tag.prop('title', textReplaced);
                $tag.find('span.tagify__tag-text').text(textReplaced);
                $tag.addClass(cssClass);
                let btnRemove = $tag.find('x');

                btnRemove.on('click', function (e) {
                    let currentTag = $(e.target).parent();

                    ResetMaxTag();
                    ValidateUsernames(currentTag.text().trim());                    
                });

                domElements.spanAddUserNameMaxTag.text(maxCountTags - 1);
            } else {               
                let $tag = $(tagName.tag);
                let btnRemove = $tag.find('x');
                btnRemove.on('click', function (e) {
                    let currentTag = $(e.target).parent();

                    ResetMaxTag();
                    ValidateUsernames(currentTag.text().trim());
                });
            }

            domElements.btnDetect.prop('disabled', true);
        }

        domElements.inputTagUsername.tagify({
            pattern: null,
            delimiters: /\r?\n|\r| |,/,
            keepInvalidTags: true,
            skipInvalid: false,
            callbacks: {},
            addTagOnBlur: true,
            duplicates: true,
            editTags: 0,
            whitelist: [],
            blacklist: [],
            maxTags: maxTags,
            enforceWhitelist: false,
            dropdown: {
                position: "input",
                enabled: 0
            }
        })
            .off('add').on('add', function (_, tagName) {
                OnChangeTags(tagName, 'valid');
                domElements.spanAddChangeHidden.text("1");
            })
            .off('invalid').on('invalid', function (_, tagName) {
                OnChangeTags(tagName, 'invalid-duplicated');
                domElements.spanAddChangeHidden.text("1");
            });

        domElements.inputTagUsername.off('keydown').on('keydown', function () {
            let keyCode = event.keyCode;

            if (keyCode === 46) {
                if (domElements.spanAddUserName.find('tags').hasClass('ctrlA')) {
                    domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
                    domElements.spanAddUserNameMaxTag.text(maxTags);
                    domElements.spanAddUserName.find('tags').removeClass('ctrlA');
                    domElements.btnDetect.prop('disabled', true);
                }
            }

            if (event.ctrlKey) {
                if (keyCode === 65 || keyCode === 97) {
                    return AddBorderTags();
                }
            }
        });

        $(document).on('mouseup', function (e) {
            let isOutSiteInput = !domElements.spanAddUserName.is(e.target) && domElements.spanAddUserName.has(e.target).length === 0;
            let isTagsFocus = domElements.spanAddUserName.find('tags.tagify--focus').length > 0;
            let isChange = domElements.spanAddChangeHidden.text();
            if (isOutSiteInput && isTagsFocus || isChange === "1") {
                domElements.spanAddUserName.addClass("loading-value");
                setTimeout(() => {
                    ValidateUsernames("");
                    domElements.spanAddUserName.removeClass("loading-value");
                    domElements.spanAddChangeHidden.text("0");
                }, 500);
            }
        });

        domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
        domElements.spanAddUserName.find('tags').attr('tabindex', "1");
        domElements.btnDetect.prop('disabled', true);
    }

    function ExportToExcel() {      
        common.downloadFileExcel(common.modifyRelativeUrl('AssociationDetection/ExportToExcelAssociationDetect'), parameters);
    }

    function ToggleTickCheckAll(isCheckAll) {
        domElements.inputOptBP.prop("checked", isCheckAll);
        domElements.inputOptDevice.prop("checked", isCheckAll);
        domElements.inputOptIP.prop("checked", isCheckAll);
        domElements.inputOptAgent.prop("checked", isCheckAll);
        domElements.inputOpt3MatchesLast7Days.prop("checked", isCheckAll);
        domElements.inputOptIPLast3Days.prop("checked", isCheckAll);
    }

    function SetCacheForFilter(chkboxElement, cacheItem) {        
        if (cacheItem != null && cacheItem == "true") {            
            chkboxElement.prop("checked", true);
        } else {
            chkboxElement.prop("checked", false);
        }        
    }

    function SetDefaultCheckboxFilter() {        
        if (domElements.inputOptBP.is(":checked")) return true;
        if (domElements.inputOptDevice.is(":checked")) return true;
        if (domElements.inputOptIP.is(":checked")) return true;
        if (domElements.inputOptAgent.is(":checked")) return true;
        if (domElements.inputOptIPLast3Days.is(":checked")) return true;
        if (domElements.inputOpt3MatchesLast7Days.is(":checked")) return true;

        domElements.inputOptBP.prop("checked", true);
        domElements.inputOptDevice.prop("checked", true);
        domElements.inputOptIP.prop("checked", true);

        return true;
    }

    function GetStorageFilter(key) {
        if (typeof (Storage) !== "undefined") {
            var value = localStorage.getItem(key);
            return value;
        }

        return null;
    }

    function SetStorageFilter(key, value) {
        if (typeof (Storage) !== "undefined") {
            localStorage.setItem(key, value);
        }
    }

    function FormatTransInfo(sum, percentage) {
        var percentage = " (" + percentage.toFixed(2) + "%)";

        return FormatNumber(sum) + percentage;
    }

    function FormatNumber(num) {
        return num ? num.toLocaleString() : 0;
    }

    function SetupDangerFilter(data) {
        if (data) {
            let dangerSelector = domElements.cbbDanger.empty();

            if (data.NoneDanger.length > 0) dangerSelector.append(BuildDangerOptgroup(0, "None", data.NoneDanger));

            if (data.Danger1.length > 0) dangerSelector.append(BuildDangerOptgroup(1, "Original", data.Danger1));

            if (data.Danger2.length > 0) dangerSelector.append(BuildDangerOptgroup(2, "ABI", data.Danger2));

            if (data.Danger3.length > 0) dangerSelector.append(BuildDangerOptgroup(3, "Other Sports", data.Danger3));

            if (data.Danger4.length > 0) dangerSelector.append(BuildDangerOptgroup(4, "Basketball", data.Danger4));

            if (data.Danger5.length > 0) dangerSelector.append(BuildDangerOptgroup(5, "E-Sports", data.Danger5));

            domElements.cbbDanger.multiselect('refresh');
        }
    }

    function BuildDangerOptgroup(optgroupValue, optgroupName, items) {
        let optgroup = `<optgroup label="${optgroupName}" DangerType=${optgroupValue}>`;

        items.map(function (x) {
            optgroup += `<option selected value=${optgroupValue}-${x.Danger} DangerValue=${x.Danger}>${x.DangerName}</option>`;
        })

        optgroup += "</optgroup>";

        return optgroup;
    }


    function SetupCCFilter(data) {
        if (data) {
            let ccSelector = domElements.cbbCC.empty();

            data.forEach(item => {
                ccSelector.append(BuildCCGroup(item.CCList, item.CCGroupName));
            });

            domElements.cbbCC.multiselect('refresh');
        }
    }

    function SetupTextBox() {
        domElements.txtBotTrans.val("");
        domElements.txtInvalidBrowserTrans.val("");
    }

    function BuildCCGroup(ccItems, groupName) {
        let optgroup = `<optgroup label="${groupName}">`;

        ccItems.map(function (item) {
            let itemName = item == -1 ? '-' : item;
            optgroup += `<option selected value=${item}>${itemName}</option>`;
        });

        optgroup += "</optgroup>";

        return optgroup;
    }

    function SetupMultiselectFilter(ccb, selectName, menuWidth = 260, menuHeight = 280) {
        ccb.multiselect({
            menuWidth: menuWidth,
            menuHeight: menuHeight,
            selectedText: "# of # select",
            noneSelectedText: `Select ${selectName}`,
            enableClickableOptGroups: true,
            includeSelectAllOption: true,
            groupsCollapsable: true,
            optGroupChecked: true
        }).multiselectfilter({
            width: 150,
            placeholder: '',
            label: ''
        });
    }

    function Search() {
        var table = domElements.tblAssociationDetection.DataTable();
        var ccOptions = domElements.cbbCC.val();
        var dangerOptions = domElements.cbbDanger.val();
        var textBot = VerifyTextBox(domElements.txtBotTrans);
        var textInvalidBrowser = VerifyTextBox(domElements.txtInvalidBrowserTrans);

        var filterData = oldData.filter(c => findElement(ccOptions, c.CustomerClass)
                                        && (dangerOptions.includes("0"+c.DangerDisplay+"0")
                                        || (dangerOptions.includes("1-" + c.Danger1)
                                        || dangerOptions.includes("2-" + c.Danger2)
                                        || dangerOptions.includes("3-" + c.Danger3)
                                        || dangerOptions.includes("4-" + c.Danger4)
                                        || dangerOptions.includes("5-" + c.Danger5))));

        if (textBot !== "") {
            if (textBot.includes("%")) {
                var floatnumber = parseFloat(textBot.replace("%", "").trim());
                filterData = filterData.filter(c => c.BotTransPercentage >= floatnumber);
            }
            else {
                var floatnumber = parseInt(textBot.replace(",", "").trim());
                filterData = filterData.filter(c => c.BotTransSum >= floatnumber);
            }

            domElements.txtBotTrans.parent().removeClass("invalid-input");
        }

        if (textInvalidBrowser !== "") {
            if (textInvalidBrowser.includes("%")) {
                var floatnumber = parseFloat(textInvalidBrowser.replace("%", "").trim());
                filterData = filterData.filter(c => c.InvalidBrowserInfoTransPercentage >= floatnumber);
            }
            else {
                var floatnumber = parseInt(textInvalidBrowser.replace(",", "").trim());
                filterData = filterData.filter(c => c.InvalidBrowserInfoTransSum >= floatnumber);
            }

            domElements.txtInvalidBrowserTrans.parent().removeClass("invalid-input");
        }

        table.clear();
        table.clear().rows.add(filterData).draw();
    }


    function findElement(filters, element) {
        for (let i = 0; i < filters.length; i++) {
            if (filters[i] == element)
                return true;
        }

        return false;
    }

    function VerifyTextBox(textBox) {
        var text = textBox.val();

        if (text === "")
            return text;

        if (!text.includes("%")) {
            if (text.length < 4 && text.includes(",")) {
                textBox.parent().addClass("invalid-input");
                return "";
            }
        }

        var numberic = text.replace("%", "");

        if (!(!isNaN(parseFloat(numberic))  && isFinite(numberic))) {
            textBox.parent().addClass("invalid-input");
            return "";
        }

        return text;
    }

    function Initial() { 
        InitInputTagUsername();

        domElements.btnDetect.on('click', () => DetectUserName());       

        domElements.btnExportToExcel.on('click', () => ExportToExcel());       

        domElements.inputOptBP.on('change', () => ChangeStateBtnDetect());

        domElements.inputOptDevice.on('change', () => ChangeStateBtnDetect());

        domElements.inputOptIP.on('change', () => ChangeStateBtnDetect());

        domElements.inputOptAgent.on('change', () => ChangeStateBtnDetect());

        domElements.inputOptIPLast3Days.on('change', () => ChangeStateBtnDetect());

        domElements.inputOpt3MatchesLast7Days.on('change', () => ChangeStateBtnDetect());

        domElements.tickCheckAll.on('click', function () {
            ToggleTickCheckAll(true);
            ChangeStateBtnDetect();
        });

        domElements.tickUncheckAll.on('click', function () {
            ToggleTickCheckAll(false);
            ChangeStateBtnDetect();
        });

        domElements.btnRemoveInvalidUserName.on('click', () => RemoveAllInvalidUsername());

        SetCacheForFilter(domElements.inputOptBP, GetStorageFilter("AssDetectCheckboxBP"));
        SetCacheForFilter(domElements.inputOptDevice, GetStorageFilter("AssDetectCheckboxDevice"));
        SetCacheForFilter(domElements.inputOptIP, GetStorageFilter("AssDetectCheckboxIP"));
        SetCacheForFilter(domElements.inputOptAgent, GetStorageFilter("AssDetectCheckboxAgent"));
        SetCacheForFilter(domElements.inputOpt3MatchesLast7Days, GetStorageFilter("AssDetectCheckbox3Matches"));
        SetCacheForFilter(domElements.inputOptIPLast3Days, GetStorageFilter("AssDetectCheckboxIP3Days"));

        SetDefaultCheckboxFilter();

        SetupMultiselectFilter(domElements.cbbCC, "CC");
        SetupMultiselectFilter(domElements.cbbDanger, "Danger");

        domElements.btnFilterGenerate.on('click', function () {
            Search();
        }); 
    }

    $(document).ready(function () {
        Initial();
    });
}

$(AssociationDetection);