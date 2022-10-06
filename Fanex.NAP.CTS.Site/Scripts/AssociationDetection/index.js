function AssociationDetection() {
    let dataTable;
    let tableAssociationDetectDetail = null;
    let tableAssociationDetectDetailIP = null;
    let tableAssociationDetectDetailMatch = null;
    let tableAssociationDetectDetailAgent = null;
    let maxTags = 1001;    
    let domElements = {
        hdIsHostPermission: $('#hdIsHostPermission'),                   
        btnResultValidationClose: $('#btnResultValidationClose'),  
        divReportContainer: $('#divReportContainer'),
        divAddUserName: $('#divAddUserName'),
        spanAddUserName : $('#spanAddUserName'),
        spanAddUserNameMaxTag: $('#spanAddUserNameMaxTag'),        
        spanAddChangeHidden: $('#spanAddChangeHidden'),
        inputTagUsername: $('#inputTagUsername'),
        inputOptBPManuallyDeviceIP: $('#inputOptBPManuallyDeviceIP'),
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
        tblAssociationDetectDetailAgent: $('#tblAssociationDetectDetailAgent')    
    };

    let parameters = {
        UserNames: "",
        IsBPManuallyDeviceIP: false,
        IsAgent: false,
        IsIPLast3Days: false,
        Is3MatchesLast7Days: false,
        AssociationByIPJson: ""
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
            columnDefs: [
                { "visible": false, "targets": 0 },
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4 , 5, 6, 7, 8], className: "text-center" },
                { targets: [0], width: "5%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "10%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "15%" },
                { targets: [6], width: "15%" },
                { targets: [7], width: "5%" },
                { targets: [8], width: "5%" }
            ],
            columns: [
                { data: obj => obj.GroupID },
                {
                    data: obj => () => {
                        let url = common.modifyRelativeUrl('Account/Index/' + obj.CTSCustID);
                        return `<a class='.view-profile-byusername' href=${url}> ${obj.UserName} </a>`;
                    },
                    createdCell: (cell, _, row) => {
                        $('.association-detect-detail', cell).click(() => {
                            ShowAssociationDetectDetail(row);
                        })
                    }
                },
                { data: obj => obj.CustStatus },
                { data: obj => obj.Evidences },
                {
                    data: obj => obj.AssociatedTypes, createdCell: (cell, _, row) => {
                        $('.association-detect-match', cell).click(() => {
                            ShowAssociationDetectMatch(row);
                        });

                        $('.icon-account_detail', cell).click(() => {
                            ShowAssociationDetectDetail(row);
                        });
                    }
                },
                {
                    data: obj => common.renderCategoryName(obj.CustomerCategory), createdCell: (cell, _, row) => {
                        common.renderCategoryColor(cell, row);
                    }
                },
                { data: obj => obj.CustomerClass },
                { data: obj => obj.AgentUsername },
                { data: obj => obj.AgentEvidence }
            ],
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;

                api.column(0, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        $(rows).eq(i).before(
                            '<tr class="group"><td colspan="8"><span><i class="icon-chevron-down"></i>' + 'Group ' + group + '</span></td></tr>'
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
                    { targets: [-2, -1, 0, 1], className: "text-center" },
                    { targets: [0], width: "50%" },
                    { targets: [1], width: "50%" },
                ],
                columns: [
                    { data: obj => obj.UserName },
                    {
                        data: obj => obj.AssociatedTypes,
                        createdCell: (cell, _, row) => {
                            $('.association-detect-detail-ip', cell).click(() => {
                                let span = $(cell).find('.association-detect-detail-ip');
                                ShowAssociationDetectDetailIP(span.attr('data-fromusername'), row.UserName);
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
        
        let ctsCustIDs = [];
        let userNames = [];

        domElements.tblAssociationDetection.find('.association-detect-match').each(function () {
            if ($(this).attr("data-groupid") == fromObj.GroupID) {
                ctsCustIDs.push($(this).attr('data-ctscustid'));
                userNames.push($(this).text().trim());
            }
        });
       
        GetAssociationDetectDetail(fromObj.UserName, fromObj.CustID, fromObj.CTSCustID, ctsCustIDs.join(), userNames.join());

        domElements.divAssociationDetectDetail.dialog({
            modal: true,
            width: '500px',
            title: "ASSOCIATION DETAILS: " + fromObj.UserName,
            resizable: false,
            draggable: true,
            close: function () {
                $(this).dialog('close');
                $('.ui-dialog .ui-dialog-content').css('padding-bottom', '16px');
                $('.ui-dialog .ui-dialog-content').css('padding-top', '27px');
                $('.ui-dialog .ui-dialog-content').css('padding-left', '12px');
            }
        });             
    }

    function GetAssociationDetectDetail(userName, custID, ctsCustID, ctsCustIDList, userNames) {
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
                CTSCustIDList: ctsCustIDList,                
                IsBPManuallyDeviceIP: parameters.IsBPManuallyDeviceIP,
                IsAgent: parameters.IsAgent,
                IsIPLast3Days: parameters.IsIPLast3Days,
                Is3MatchesLast7Days: parameters.Is3MatchesLast7Days,
                AssociationByIPJson: parameters.AssociationByIPJson     
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

    function ShowAssociationDetectDetailIP(userName1, userName2) {                 
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

        GetAssociationDetectDetailIP(userName1, userName2);

        domElements.divAssociationDetectDetailIP.dialog({
            modal: true,
            width: '500px',
            title: "",
            resizable: false,
            draggable: true,
            close: function () {
                $(this).dialog('close');
                $('.ui-dialog .ui-dialog-content').css('padding-bottom', '16px');
                $('.ui-dialog .ui-dialog-content').css('padding-top', '27px');
                $('.ui-dialog .ui-dialog-content').css('padding-left', '12px');
            }
        });

        domElements.divAssociationDetectDetailIP.parent().find('.ui-dialog-title').html("<div>ASSOCIATION DETAIL - IP LIST (in the last 3 days): <br/>" + userName1 + " - " + userName2 + "</div>");      
    }

    function GetAssociationDetectDetailIP(userName1, userName2) {
        $.ajax(common.modifyRelativeUrl('AssociationDetection/GetDetectAssociationDetailIP'), {
            type: 'POST',
            beforeSend: function () {
                domElements.tblAssociationDetectDetailIP.find('tbody').addClass('loading');                
            },
            data: {
                userNameList: userName1 + ',' + userName2,              
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

        GetAssociationDetectDetailMatch(fromObj.CustID, -1, custIDs);

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
            width: '950px',
            title: title,
            resizable: false,
            draggable: true,
            close: function () {
                $(this).dialog('close');
                $('.ui-dialog .ui-dialog-content').css('padding-bottom', '16px');
                $('.ui-dialog .ui-dialog-content').css('padding-top', '27px');
                $('.ui-dialog .ui-dialog-content').css('padding-left', '12px');
            }
        });
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
                    { targets: [-1, 0, 1], className: "text-center" },
                    { targets: [0], width: "35%" },
                    { targets: [1], width: "35%" },
                    { targets: [2], width: "30%" }
                ],
                columns: [
                    { data: obj => obj.UserName },
                    { data: obj => obj.Agent },
                    { data: obj => obj.AssociatedType }
                ]
            });
        }

        domElements.tblAssociationDetectDetailAgent.DataTable().clear().rows.add([]);

        GetAssociationDetectDetailAgent(agentInfoJson);

        domElements.divAssociationDetectDetailAgent.dialog({
            modal: true,
            width: '500px',
            title: "ASSOCIATION DETAILS - AGENT: " + userName1 + " - " + userName2,
            resizable: false,
            draggable: true,
            close: function () {
                $(this).dialog('close');
                $('.ui-dialog .ui-dialog-content').css('padding-bottom', '16px');
                $('.ui-dialog .ui-dialog-content').css('padding-top', '27px');
                $('.ui-dialog .ui-dialog-content').css('padding-left', '12px');
            }
        });      
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
                                .append('<td class="text-center">' + items[i]['Agent'] + '</td>')
                                .append('<td rowspan="2" class="text-center">' + items[i]['AssociatedType'] + '</td>')
                                .append('</tr>'));
                        } else {
                            $(tbody).append($('<tr role="row" class="odd">')
                                .append('<td class="text-center">' + items[i]['UserName'] + '</td>')
                                .append('<td class="text-center">' + items[i]['Agent'] + '</td>')                               
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
        if (domElements.inputOptBPManuallyDeviceIP.is(":checked")) {
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

    function ValidateUsernames(textTagRemove) {
        let numOfTagExceeded = domElements.spanAddUserName.find('tags tag.tagify--notAllowed');

        if (textTagRemove != '' && numOfTagExceeded.length > 1) return;
        else if (textTagRemove == '' && numOfTagExceeded.length > 0) return;
      
        let userNames = GetInputUserName(textTagRemove);

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
        }
    }    

    function GetInputUserName(textTagRemove) {
        let userNameList = [];
        let tags = domElements.spanAddUserName.find('tags tag');

        tags.map(function (_, ele) {
            let text = $(ele).text().trim();
            if (textTagRemove !== "" && text === textTagRemove) {
                textTagRemove = "";
                return;
            }
            userNameList.push(text);
        });

        return userNameList.join();
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

        let isEnableDetect = data.UserNameInvalidList.length === 0
            && data.UserNameValidList.length >= 2;           

        domElements.btnDetect.prop('disabled', !isEnableDetect);      
    }

    function DetectUserName() {
        let userNames = GetInputUserName();

        if (userNames !== '') {
            parameters.UserNames = userNames;
            parameters.IsBPManuallyDeviceIP = domElements.inputOptBPManuallyDeviceIP.is(":checked");
            parameters.IsAgent = domElements.inputOptAgent.is(":checked");
            parameters.IsIPLast3Days = domElements.inputOptIPLast3Days.is(":checked");
            parameters.Is3MatchesLast7Days = domElements.inputOpt3MatchesLast7Days.is(":checked");

            $.ajax(common.modifyRelativeUrl('AssociationDetection/DetectAssociationByUserNames'), {
                type: 'POST',
                data: parameters,
                beforeSend: function () {
                    domElements.divAddUserName.addClass("loading-block");
                },
                success: function (response) {
                    if (response.data.ErrorCode == 0) {
                        dataTable
                            ? domElements.tblAssociationDetection.DataTable().clear().rows.add(response.data.ItemList).draw()
                            : dataTable = GenerateGrid(response.data.ItemList);

                        domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
                        domElements.spanAddUserNameMaxTag.text(maxTags);
                        domElements.spanAddChangeHidden.text("0");
                        domElements.btnDetect.prop('disabled', true);

                        parameters.AssociationByIPJson = response.associationByIPJson;
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
                },
                error: function (errMsg) {
                    console.log(errMsg);
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

    function Initial() { 
        InitInputTagUsername();

        domElements.btnDetect.on('click', () => DetectUserName());

        //domElements.btnDetect.on('click', () => { DetectUserName(); ExportToExcel() });

        domElements.btnExportToExcel.on('click', () => ExportToExcel());       

        domElements.inputOptBPManuallyDeviceIP.on('change', () => ChangeStateBtnDetect());

        domElements.inputOptAgent.on('change', () => ChangeStateBtnDetect());

        domElements.inputOptIPLast3Days.on('change', () => ChangeStateBtnDetect());

        domElements.inputOpt3MatchesLast7Days.on('change', () => ChangeStateBtnDetect());
    }

    $(document).ready(function () {
        Initial();
    });
}

$(AssociationDetection);