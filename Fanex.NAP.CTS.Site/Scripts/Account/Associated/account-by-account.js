function account_by_account() {
    var divAddAssociation = $('#divAddAssociation');
    var btnAddAssociation = $('#btnAddAssociation');
    var btnSearch = $('#account-by-account-btn-search');
    var btnViewOnGraph = $('#account-by-account-btn-on-graph');
    var associatedByAccountTable = $('#account-by-account-table');    
    var asscomment = $('#txtasscomment');
    var btnSaveAssociation = $('#btnsaveassociation');
    var btnCancelAssociation = $('#btncancelassociation');
    var cbbAssociationAccountStatus = $('#cbbAssociationAccountStatus');
    var cbbAssociationSubscriber = $('#cbbAssociationSite');
    var _username;
    var dataTable;
    let isHostPermission;
    let spanAddAssociationUserName = $('#spanAddAssociationUserName');
    let spanAddAssociationUserNameMaxTag = $('#spanAddAssociationUserNameMaxTag');
    let spanAddAssociationChangeHidden = $('#spanAddAssociationChangeHidden');
    let inputTagUsername = $('#inputTagUsername'); 
    let maxTags = 50;
    let divMatchBettingPattern = $('#divMatchBettingPattern');
    let tableMatchBettingPattern = $('#tblMatchBettingPattern');
    let dataTableMatchBettingPattern = null;
    let spanBettingPatternTotalMatch = $('#spanBettingPatternTotalMatch');
	let divIPAssociation = $('#divIPAssociation');
    let tblIPAssociation = $('#tblIPAssociation');
    let dataTableIPAssociation = null;
    let spanIPAssociationTotalMatches = $('#spanIPAssociationTotalMatches');
    let spanIPAssociationTotalIPSharing = $('#spanIPAssociationTotalIPSharing');    let cbbAssociationCategory = $('#cbbAssociationCategory');
    let isSkipTracking = true;

    function RenderTag(obj) {
        let html = "";

        if (obj.IsFlaggedTag)
            html += "<span class='icon icon-Flagged' title='Flagged'></span>";        

        if (obj.IsAffectedTag)
            html += "<span class='icon icon-Affected'  title='Affected'></span>";            

        return html === "" ? "-" : html;
    }

    function RenderUsername(obj) {
        if (obj.Username !== null) {
            var url = common.modifyRelativeUrl('Account/Index/' + obj.CTSCustId);

            return '<a class="info-bng" href="' + url + '">' + obj.Username + '</a>';
        }

        return null;
    } 

    function RenderCategoryNames(categoryNames) {
        let html = '';
        let categoryList = categoryNames != undefined
            ? categoryNames.split(',')
            : [];

        if (categoryList.length > 2)
            html = "<span>" + categoryList[0] + ", " + categoryList[1] + ",... </span>";
        else if (categoryList.length === 2)
            html = "<span>" + categoryList[0] + ", " + categoryList[1] + "</span>";
        else
            html = "<span>" + categoryNames + "</span>";

        return html;
    }

    function RenderGrid() {
        let url = common.modifyRelativeUrl('Account/GetAssociatedByAccount');
        let ctsCustId = $("#hdCTSCustId").val();
        let subscriberId = $("#hdSubscriberId").val();

        let table = associatedByAccountTable.DataTable({
            serverSide: true,
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            pagingType: "full_numbers",
            ajax: {
                url: url,
                data: {
                    CTSCustId: function () {
                        return $("#hdCTSCustId").val();
                    },
                    AssociationStatus: function () {
                        return $('#association-cbb-status').val();
                    },
                    AssociationType: function () {
                        return $("#account-by-account-cbb-association-type").val();
                    },
                    IsDirect: function () {
                        return true;
                    },
                    AccountStatusIds: function () {
                        return cbbAssociationAccountStatus.val();
                    },
                    CategoryIds: function () {
                        return cbbAssociationCategory.val();
                    },
                    SubscriberIds: function () {
                        var selectedSubscribers = [];
                        cbbAssociationSubscriber.find('option:selected').each(function () { selectedSubscribers.push($(this).val()); });
                        if (selectedSubscribers.length === 0) {
                            cbbAssociationSubscriber.find('option').not(':selected').each(function () { selectedSubscribers.push($(this).val()); });
                        }
                        return selectedSubscribers.join(',');
                    },
                    IsSkipTracking: function () { return isSkipTracking; }
                },
                type: "POST",
                beforeSend: function () {
                    associatedByAccountTable.find('tbody').addClass('loading');
                },
                complete: function () {
                    associatedByAccountTable.find('tbody').removeClass('loading');

                    btnSearch.prop('disabled', false);
                }
            },
            language: {
                lengthMenu: "Page size _MENU_",
                paginate: {
                    first: "<span class=\"icon-chevron-left\"></span><span class=\"icon-chevron-left\"></span>",
                    last: "<span class=\"icon-chevron-right\"></span><span class=\"icon-chevron-right\"></span>",
                    next: "",
                    previous: ""
                },
                info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            ordering: false,
            searching: false,
            lengthMenu: [10, 20, 50, 100, 200],
            pageLength: 50,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [-2, -1], className: "text-center" },
                { targets: [0, 1, 2, 3, 4, 5], className: "text-center" },
                { targets: [0], width: "5%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "15%" },
                { targets: [3], width: "10%" },
                { targets: [4], visible: false },
                { targets: [5], width: "10%", className: "text-center" },
                { targets: [6], width: "10%", className: "text-center" },
                { targets: [7], width: "10%", className: "text-center" },
                { targets: [8], width: "10%", className: "text-center" },
                { targets: [9], width: "10%", className: "text-center" }
            ],
            columns: [
                { data: obj => RenderTag(obj) },
                { data: obj => RenderUsername(obj, ctsCustId) },
                { data: obj => obj.LoginId },
                { data: obj => obj.Status },
                { data: obj => obj.SubscriberName },
                {
                    data: obj => obj.AssociationTypeName,
                    createdCell: (cell, _, row) => {
                        $('.betting-pattern', cell).click(() =>
                            ShowMatchBettingPattern(row.CustId, row.GroupIDs, row.OriginGroupIDs)
                        );

                        $('.ip-association', cell).click(() =>
                            ShowIPAssociation(row.CustId)
                        );
                    }
                },
                {
                    data: obj => RenderCategoryNames(obj.CategoryNames), createdCell: (cell, _, row) => {
                        common.renderCategoryColor(cell, row);
                    }
                },
                { data: obj => obj.CustomerClasses },
                {
                    data: obj => AssociationStatus(obj.IsLinked), createdCell: (cell, _, row) => {
                        if (isHostPermission) {
                            $('.toggle', cell).click(function (e) {
                                e.preventDefault();

                                ActiveRemove(row, ctsCustId, subscriberId);
                            });
                        }
                    }
                },
                {
                    data: obj => obj.AssociationDate
                }
            ],
            createdRow: function (row, obj) {
                $(row).attr('data-site-id', obj.SubscriberId);
            },
            drawCallback: function () {
                let api = this.api();
                let rows = api.rows({ page: 'current' }).nodes();
                var data = api.rows({ page: 'current' }).data();
                let last = null;

                api.column(4, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        let html = '<tr class="group group-site">';
                        html += '<td colspan="9">';
                        html += '<span class="column-group-site" data-site-id=' + data[i].SubscriberId + '><i class="icon icon-chevron-down icon-open"></i> ' + group + ' </span>';
                        html += '</td></tr>';

                        $(rows).eq(i).before(html);
                        last = group;
                    }
                });
            }
        });

        $('tbody', associatedByAccountTable).on('click', 'tr.group', function () {
            let trGroup = $(this);
            let subscriberId = trGroup.find('.column-group-site').attr('data-site-id');

            if (subscriberId !== undefined) {
                $('tr[data-site-id="' + subscriberId + '"]', associatedByAccountTable).each(function () {
                    let trCurrent = $(this);
                    if (trCurrent.hasClass('hide')) {
                        trCurrent.removeClass('hide');
                        $('i.icon', trGroup).removeClass('icon-chevron-right').addClass('icon-chevron-down');
                    } else {
                        trCurrent.addClass('hide');
                        $('i.icon', trGroup).removeClass('icon-chevron-down').addClass('icon-chevron-right');
                    }
                });
            }
        });
        
        return table;
    }  

    function AssociationStatus(isLinked) {
        var html = '<div class="toggle ' + (isLinked ? 'toggle-on' : '') + '" id="switch">';
        html += '<div class="toggle-text-off' + (!isHostPermission ? ' disabled"' : '') + '"> Unlinked</div >';
        html += '<div class="glow-comp"></div> <div class="toggle-button ' + (!isHostPermission ? ' disabled"' : '') + '"></div>';
        html += '<div class="toggle-text-on' + (!isHostPermission ? ' disabled"' : '') + '"> Linked</div>';
        html += '</div>';

        return html;
    }
   
    function ActiveRemove(obj, fromCTSCustId, fromSubscriberId) {
        common.showConfirmation("<div class='popup-content'><div class='popup-title'>Do you want to update Association Status between <b>" + _username + "</b> and <b>" + obj.Username + "</b> ?</div></div>",
            function () {
                RemoveAssociation(fromSubscriberId, fromCTSCustId, obj.SubscriberId, obj.CTSCustId, !obj.IsLinked);
            });

    }

    function GetTotalAssociation(ctsCustId) {
        $.ajax(common.modifyRelativeUrl('Account/GetCustomerTotalAssociationByCTSCustId'), {
            type: 'POST',
            data: {
                ctsCustId: ctsCustId
            },
            success: function (response) {
                totalAssAccount = response.TotalAccounts;
                $('#spanTotalAssociated').html(response.TotalAccounts + " / " + response.TotalDevices);
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    } 

    function RemoveAssociation(fromSubscriberId, fromCTSCustId, toSubscriberId, toCTSCustId, isLinked) {
        var url = common.modifyRelativeUrl('Account/ActiveRemoveAssociation');
        var params = {};
        params.fromSubscriberId = parseInt(fromSubscriberId);
        params.fromCTSCustId = parseInt(fromCTSCustId);
        params.toSubscriberId = toSubscriberId;
        params.toCTSCustId = toCTSCustId;
        params.Remark = "";
        params.IsLinked = isLinked;

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.errorCode === 0) {
                    common.showSuccess('Updated association successful!', 2000);
                }
                else if (response.errorCode === -1) {
                    common.showError('This association is already unlinked!');
                }
                else {
                    common.showError('Updated association fail!');
                }
            },
            complete: function () {
                setTimeout(() => {
                    LoadGrid(true);
                }, 2000);
            }
        });
    }

    function ValidateManualAssociationMultiple(fromCTSCustId, textTagRemove) {
        let userNameList = GetInputUserName(textTagRemove);

        if (userNameList !== '') {
            let params = {};
            params.fromCTSCustId = fromCTSCustId;
            params.toUserNameList = userNameList;

            $.ajax(common.modifyRelativeUrl('Account/ValidateManualAssociationMultiple'), {
                type: 'POST',
                data: params,
                beforeSend: function () {
                    spanAddAssociationUserName.addClass("loading-value");
                },
                success: function (response) {
                    if (response.IsException)
                        BindExceptionValidateManualAssociationMultiple();
                    else
                        BindResultValidatedManualAssociationMultiple(response);
                },
                complete: function () {
                    spanAddAssociationUserName.removeClass("loading-value");
                },
                error: function (errMsg) {
                    console.log(errMsg);
                }
            });
        }
    }

    function BindExceptionValidateManualAssociationMultiple() {
        spanAddAssociationUserName.find('tags tag').each(function (_, tag) {
            $(tag).removeClass('valid').addClass('invalid-username');
        });
    }

    function BindResultValidatedManualAssociationMultiple(data) {
        spanAddAssociationUserName.find('tags tag').each(function (_, tag) {
            $(tag).removeClass('invalid-username').removeClass('invalid-duplicated').addClass('valid');
        });

        data.UserNameInvalidList.map(function (x) {
            let usernameEqual = $("tag[data-user-name='" + x + "']");
            if (usernameEqual !== undefined) {
                usernameEqual.removeClass('valid').addClass('invalid-username');
            }            
        });

        data.UserNameDuplicatedList.map(function (x) {
            let usernameEqual = $("tag[data-user-name='" + x + "']");
            if (usernameEqual !== undefined) {
                usernameEqual.removeClass('valid').addClass('invalid-duplicated');
            }
        });

        let isEnableAdd = data.UserNameInvalidList.length === 0 && data.UserNameDuplicatedList.length === 0;

        if (isEnableAdd)
            btnSaveAssociation.prop('disabled', false);
        else
            btnSaveAssociation.prop('disabled', true);
    }

    function InsertManualAssociationMultiple(fromSubscriberId, fromCTSCustId) {
        let userNameList = GetInputUserName();

        if (userNameList !== '') {
            let params = {};
            params.fromSubscriberId = fromSubscriberId;
            params.fromCTSCustId = fromCTSCustId;
            params.toUserNameList = userNameList;
            params.remark = asscomment.val().trim();

            $.ajax(common.modifyRelativeUrl('Account/InsertManualAssociationMultiple'), {
                type: 'POST',
                data: params,
                beforeSend: function () {
                    divAddAssociation.addClass("loading-block");
                },
                success: function (response) {
                    if (response.ErrorCode === 0) {
                        common.showSuccess("Add association successful");
                        setTimeout(() => {
                            divAddAssociation.dialog('close');
                            $('#association-cbb-status').val(0);
                            $("#account-by-account-cbb-association-type").val(3);
                            LoadGrid(true);
                            GetTotalAssociation(fromCTSCustId);
                        }, 1000);                        
                    } else {
                        BindResultValidatedManualAssociationMultiple(response.ResultValidated);
                    }
                },
                complete: function () {
                    divAddAssociation.removeClass("loading-block");                   
                },
                error: function (errMsg) {
                    console.log(errMsg);
                }
            });
        }
    }

    function GetInputUserName(textTagRemove) {
        let userNameList = [];
        let tags = spanAddAssociationUserName.find('tags tag');

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

    function InitInputTagUsername(fromCTSCustid) {
        spanAddAssociationUserNameMaxTag.text(maxTags);
        spanAddAssociationChangeHidden.text("0");

        function AddBorderTags() {
            spanAddAssociationUserName.find('tags').addClass('ctrlA');
            let allTags = spanAddAssociationUserName.find('tags tag.tagify__tag');
            allTags.each(function (_, tag) {
                $(tag).css('border', '1px solid #6187E7');
            });
        }

        function ResetMaxTag() {
            let currentTagsLength = spanAddAssociationUserName.find('tags tag').length;
            if (currentTagsLength < maxTags)
                spanAddAssociationUserNameMaxTag.text(maxTags - currentTagsLength + 1);
        }

        function EnableButtonAdd() {
            let validTags = spanAddAssociationUserName.find('tags tag.valid');
            let invalidDuplicatedTags = spanAddAssociationUserName.find('tags tag.invalid-duplicated');
            let invalidUsernameTags = spanAddAssociationUserName.find('tags tag.invalid-username');
            let notInvalid = invalidDuplicatedTags.length + invalidUsernameTags.length;
            if (notInvalid <= 1 && validTags.length > 1)
                btnSaveAssociation.prop('disabled', false);
            else
                btnSaveAssociation.prop('disabled', true);
        }

        function IsTextUnicode(text) {
            var pattern = /^[\x20-\x7E]*$/;
            return pattern.test(text);
        }

        function OnChangeTags(tagName, cssClass) {
            let maxCountTags = parseInt(spanAddAssociationUserNameMaxTag.text());
            let isTextUnicode = IsTextUnicode($(tagName.tag).text()); 

            if (!isTextUnicode) {
                inputTagUsername.tagify().data('tagify').removeTags(tagName.tag);                
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
                    ValidateManualAssociationMultiple(fromCTSCustid, currentTag.text().trim());
                    EnableButtonAdd();
                });

                spanAddAssociationUserNameMaxTag.text(maxCountTags - 1);
            } else {
                inputTagUsername.tagify().data('tagify').removeTags(tagName.tag);                
            }
            btnSaveAssociation.prop('disabled', true);
        }
        
        inputTagUsername.tagify({
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
                spanAddAssociationChangeHidden.text("1");
            })
            .off('invalid').on('invalid', function (_, tagName) {
                OnChangeTags(tagName, 'invalid-duplicated');
                spanAddAssociationChangeHidden.text("1");
            });            
        
        inputTagUsername.off('keydown').on('keydown', function () {
            let keyCode = event.keyCode;

            if (keyCode === 46) {
                if (spanAddAssociationUserName.find('tags').hasClass('ctrlA')) {
                    inputTagUsername.tagify().data('tagify').removeAllTags();
                    spanAddAssociationUserNameMaxTag.text(maxTags);
                    spanAddAssociationUserName.find('tags').removeClass('ctrlA');
                }
            }
            
            if (event.ctrlKey) {
                if (keyCode === 65 || keyCode === 97) {
                    return AddBorderTags();
                }
            }

        });

        $(document).on('mouseup', function (e) {
            let isOutSiteInput = !spanAddAssociationUserName.is(e.target) && spanAddAssociationUserName.has(e.target).length === 0;
            let isTagsFocus = spanAddAssociationUserName.find('tags.tagify--focus').length > 0;
            let isChange = spanAddAssociationChangeHidden.text();
            if (isOutSiteInput && isTagsFocus || isChange === "1") {               
                asscomment.focus();
                spanAddAssociationUserName.addClass("loading-value");
                setTimeout(() => {
                    ValidateManualAssociationMultiple(fromCTSCustid, "");
                    spanAddAssociationUserName.removeClass("loading-value");
                    spanAddAssociationChangeHidden.text("0");
                }, 500);
            }
        });

        inputTagUsername.tagify().data('tagify').removeAllTags();
        spanAddAssociationUserName.find('tags').attr('tabindex', "1");
        btnSaveAssociation.prop('disabled', true);
    }

    function ShowMatchBettingPattern(custId, groupIDs, originGroupIDs) {        

        divMatchBettingPattern.dialog({
            modal: true,
            width: '1200px',
            title: 'ASSOCIATION DETAILS',
            resizable: false,
            draggable: true,
            close: function () {
                $(this).dialog('close');
                $('.ui-dialog .ui-dialog-content').css('padding-bottom', '16px');
                $('.ui-dialog .ui-dialog-content').css('padding-top', '27px');
                $('.ui-dialog .ui-dialog-content').css('padding-left', '12px');
            }
        });

        if (dataTableMatchBettingPattern === null) {
            dataTableMatchBettingPattern = tableMatchBettingPattern.DataTable({
                language: {
                    zeroRecords: "There's no data"
                },
                info: false,
                ordering: false,
                searching: false,
                scrollX: false,
                paging: false,
                columnDefs: [
                    { targets: "_all", defaultContent: "-" },
                    { targets: [-1, -2], className: "text-center" },
                    { targets: [0], visible: false },
                    { targets: [1], width: "5%", className: "text-center" },
                    { targets: [2], width: "15%", className: "text-left" },
                    { targets: [3], width: "15%", className: "text-left" },
                    { targets: [4], width: "15%", className: "text-center" },
                    { targets: [5], width: "15%", className: "text-left" },
                    { targets: [6], width: "10%", className: "text-left" },
                    { targets: [7], width: "5%", className: "text-left" },
                    { targets: [8], width: "10%", className: "text-center" },
                    { targets: [9], width: "5%", className: "text-center" },
                    { targets: [10], width: "5%", className: "text-center" }
                ],
                columns: [
                    { data: obj => obj.MatchDesc },
                    { data: obj => obj.GroupID },
                    { data: obj => obj.Description },
                    { data: obj => obj.UserName },
                    { data: obj => obj.TransDateString },
                    { data: obj => obj.TransId },
                    { data: obj => obj.BetType },
                    { data: obj => obj.BetTeam },
                    { data: obj => obj.Stake },
                    { data: obj => obj.Odds },
                    { data: obj => obj.LiveScore }
                ],
                createdRow: function (row, _) {
                    if (!$(row).hasClass('detail')) {
                        $(row).addClass('hide');
                    }
                },
                drawCallback: function () {
                    let api = this.api();
                    let rows = api.rows({ page: 'current' }).nodes();
                    var data = api.rows({ page: 'current' }).data();
                    let last = null;

                    api.column(0, { page: 'current' }).data().each(function (group, i) {
                        if (last !== group) {
                            let html = '<tr class="group group-site"><td colspan="10"><span class="column-group-match"';
                            html += ' data-match-id=' + data[i].MatchID;
                            html += ' data-event-date-string=' + data[i].EventDateString;
                            html += ' data-custID1=' + data[i].CustID1;
                            html += ' data-custID2=' + data[i].CustID2;
                            html += ' data-isHavingSGB=' + data[i].IsHavingSGB;
                            html += ' data-bettypeIDs=' + data[i].BettypeIDs;
                            html += ' data-isHavingGGB=' + data[i].IsHavingGGB;
                            html += ' data-states=' + data[i].States;
                            html += ' data-groupIds=' + data[i].GroupIDs+ '>';
                            html += '<i class="icon icon-chevron-right icon-open"></i> ' + group;
                            html += '</span></td></tr>';

                            $(rows).eq(i).before(html);
                            last = group;
                        }
                    });
                }
            }); 

            $('tbody', tableMatchBettingPattern).on('click', 'tr.group', function () {
                let trGroup = $(this);
                let colGroupMatch = trGroup.find('.column-group-match');
                if (colGroupMatch !== undefined) {
                    let iGroup = $('i.icon', trGroup);

                    if (!iGroup.hasClass('has-detail')) {                        
                        let params = {
                            MatchId: colGroupMatch.attr('data-match-id'),
                            EventDateString: colGroupMatch.attr('data-event-date-string'),
                            CustID1: colGroupMatch.attr('data-custID1'),
                            CustID2: colGroupMatch.attr('data-custID2'),
                            IsHavingSGB: colGroupMatch.attr('data-isHavingSGB'),
                            BettypeIDs: colGroupMatch.attr('data-bettypeIDs'),
                            IsHavingGGB: colGroupMatch.attr('data-isHavingGGB'),
                            States: colGroupMatch.attr('data-states'),
                            GroupIDs: colGroupMatch.attr('data-groupIds')
                        }

                        GetMatchBettingPatternDetail(params, trGroup);

                        iGroup.removeClass('icon-chevron-right').addClass('icon-chevron-down');
                    } else {                        
                        if (iGroup.hasClass('icon-chevron-right'))
                            iGroup.removeClass('icon-chevron-right').addClass('icon-chevron-down');
                        else
                            iGroup.removeClass('icon-chevron-down').addClass('icon-chevron-right');
                        
                        $('tr[data-match-id="' + colGroupMatch.attr('data-match-id') + '"]', tableMatchBettingPattern).each(function () {
                            let trCurrent = $(this);
                            if (trCurrent.hasClass('hide'))
                                trCurrent.removeClass('hide');
                            else
                                trCurrent.addClass('hide');
                        });
                    }
                }
            });
        }

        GetMatchBettingPattern($('#hdCustId').val(), custId, groupIDs, originGroupIDs);    

        $('.ui-dialog .ui-dialog-content').css('padding', '0');
    }

    function GetMatchBettingPattern(custId1, custId2, groupIDs, originGroupIDs) {
        spanBettingPatternTotalMatch.text("");        

        var url = common.modifyRelativeUrl('Account/GetMatchesAssociationByAI');

        $.ajax(url, {
            type: 'POST',
            beforeSend: function () {
                tableMatchBettingPattern.find('tbody').addClass('loading');
            },
            data: {
                custID1: custId1,
                custID2: custId2,
                groupIDs: groupIDs,
                originGroupIDs: originGroupIDs
            },
            success: function (response) {
                if (response !== undefined) {
                    if (response.ErrorMessage === null || response.ErrorMessage === '' && response.MatchesInfo.length > 0) {
                        tableMatchBettingPattern.DataTable().clear().rows.add(response.MatchesInfo).draw();                        
                        tableMatchBettingPattern.DataTable().columns.adjust();
                        spanBettingPatternTotalMatch.text(response.TotalMatchCount);
                    } else {
                        console.log(response.ErrorMessage);
                        common.showError(response.ErrorMessage);
                    }                    
                } 
            },
            complete: function () {
                tableMatchBettingPattern.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function GetMatchBettingPatternDetail(params, target) {
        var url = common.modifyRelativeUrl('Account/GetMatchesAssociationDetailByAI');
        
        $.ajax(url, {
            type: 'POST',
            beforeSend: function () {
                tableMatchBettingPattern.find('tbody').addClass('loading');
            },
            data: params,
            success: function (response) {
                if (response !== undefined) {                    
                    for (var i = 0; i < response.length; i++) {
                        let newRow = $('<tr role="row" class="odd detail" data-match-id=' + params.MatchId + '>')
                            .append('<td class="hide">' + response[i]["MatchDesc"] + '</td>')
                            .append('<td class="text-center">' + response[i]["GroupID"] + '</td>')
                            .append('<td class="text-center">' + response[i]["Description"] + '</td>')
                            .append('<td class="text-center">' + response[i]["UserName"] + '</td>')
                            .append('<td class="text-center">' + response[i]["TransDateString"] + '</td>')
                            .append('<td class="text-center">' + response[i]["TransId"] + '</td>')
                            .append('<td class="text-center">' + response[i]["BetType"] + '</td>')
                            .append('<td class="text-center">' + response[i]["BetTeam"] + '</td>')
                            .append('<td class="text-center">' + response[i]["Stake"] + '</td>')
                            .append('<td class="text-center"><span ' + (response[i]["Odds"] < 0 ? 'class="red"' : '') + '>' + response[i]["Odds"] + '</span></td>')
                            .append('<td class="text-center">' + response[i]["LiveScore"] + '</td>')
                            .append('</tr>');

                        target.last().after(newRow);
                    }

                    tableMatchBettingPattern.DataTable().columns.adjust();

                    let iGroup = $('i.icon', target);
                    iGroup.addClass('has-detail');
                }
            },
            complete: function () {
                tableMatchBettingPattern.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function ShowIPAssociation(custId) {
        divIPAssociation.dialog({
            modal: true,
            width: '1400px',
            title: 'ASSOCIATION DETAILS',
            resizable: false,
            draggable: true,
            close: function () {
                $(this).dialog('close');
                $('.ui-dialog .ui-dialog-content').css('padding-bottom', '16px');
                $('.ui-dialog .ui-dialog-content').css('padding-top', '27px');
                $('.ui-dialog .ui-dialog-content').css('padding-left', '12px');
            }
        });

        if (dataTableIPAssociation == null) {
            dataTableIPAssociation = tblIPAssociation.DataTable({
                language: {
                    zeroRecords: "There's no data"
                },
                info: false,
                ordering: false,
                searching: false,
                scrollX: false,
                paging: false,                
                columnDefs: [
                    { targets: "_all", defaultContent: "-" },
                    { targets: [0], width: "15%", className: "text-center" },
                    { targets: [1], width: "15%", className: "text-center" },
                    { targets: [2], className: "text-center col-league" },
                    { targets: [3], width: "15%", className: "text-center" },
                    { targets: [4], width: "15%", className: "text-center col-ip" },
                    { targets: [5], width: "15%", className: "text-center" }
                ],
                columns: [
                    { data: obj => obj.MatchID },
                    { data: obj => obj.EventName },
                    { data: obj => obj.LeagueName },
                    { data: obj => obj.EventDateString },
                    { data: obj => obj.IP },
                    { data: obj => obj.BetTypes }
                ]
            });
        }

        GetIPAssociation($('#hdCustId').val(), custId);        

        $('.ui-dialog .ui-dialog-content').css('padding', '0');
    }

    function GetIPAssociation(custId1, custId2) {
        spanIPAssociationTotalMatches.text("");
        spanIPAssociationTotalIPSharing.text("");

        var url = common.modifyRelativeUrl('Account/GetIPAssociation');

        $.ajax(url, {
            type: 'POST',
            beforeSend: function () {
                tblIPAssociation.find('tbody').addClass('loading');
            },
            data: {
                custId1: custId1,
                custId2: custId2
            },
            success: function (response) {
                if (response !== undefined) {
                    if (response.ErrorCode == 0) {
                        tblIPAssociation.DataTable().clear().rows.add(response.ItemList);
                        tblIPAssociation.DataTable().columns.adjust().draw();

                        spanIPAssociationTotalMatches.text(response.TotalMatchCount);
                        spanIPAssociationTotalIPSharing.text(response.TotalIPSharingCount);
                    } else {
                        console.log(response.ErrorMessage);
                        common.showError(response.ErrorMessage);
                    }
                }
            },
            complete: function () {
                tblIPAssociation.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function DisableButton(hostPermission) {
        isHostPermission = hostPermission === "True";

        if (!isHostPermission)
            btnAddAssociation.prop('disabled', true);
    }

    function LoadGrid(isSkip) {
        $(this).prop('disabled', true);

        isSkipTracking = isSkip;

        dataTable
            ? dataTable.ajax.reload(null, true)
            : dataTable = RenderGrid();
    } 

    this.Initial = function (ctsCustid, username, subscriberId, subscriber, hostPermission) {
        _username = username;        

        DisableButton(hostPermission);

        $("#account-by-account-cbb-status").multiselect();

        cbbAssociationSubscriber.multiselect({
            menuWidth: 320,
            menuHeight: 320,
            selectedText: "# of # selected",
            noneSelectedText: "Select Subscriber",
            enableClickableOptGroups: true,
            includeSelectAllOption: true,
            groupsCollapsable: true,
            optGroupChecked: true
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
        });

        cbbAssociationCategory.multiselect({
            menuWidth: 276,
            menuHeight: 330,
            selectedText: "# of # selected",
            noneSelectedText: "Select Category",
            enableClickableOptGroups: true,
            includeSelectAllOption: true,
            groupsCollapsable: true,
            optGroupChecked: true
        }).multiselectfilter({
            width: 150,
            placeholder: '',
            label: ''
        });

        dataTable = RenderGrid();

        btnSearch.on('click', () => LoadGrid(false));

        btnViewOnGraph.on('click', function () {
            let info = dataTable.page.info();
            let selectedSubscribers = [];
            cbbAssociationSubscriber.find('option:selected').each(function () { selectedSubscribers.push($(this).val()); });
            if (selectedSubscribers.length === 0) {
                cbbAssociationSubscriber.find('option').not(':selected').each(function () { selectedSubscribers.push($(this).val()); });
            }
            let subscriberIds = selectedSubscribers.join(',');

            let selectedCategories = [];
            cbbAssociationCategory.find('option:selected').each(function () { selectedCategories.push($(this).val()); });
            if (selectedCategories.length === 0) {
                cbbAssociationCategory.find('option').not(':selected').each(function () { selectedCategories.push($(this).val()); });
            }
            let categoryIds = selectedCategories.join(',');

            let params = {};
            params.CTSCustId = ctsCustid;
            params.displayName = username;
            params.AssociationStatus = $('#association-cbb-status').val();
            params.AssociationType = $("#account-by-account-cbb-association-type").val();
            params.AccountStatusIds = cbbAssociationAccountStatus.val();
            params.CategoryIds = categoryIds;
            params.SubscriberIds = subscriberIds;
            params.start = info.start;
            params.length = info.length;

            let formGraph = new graph();
            formGraph.GenerateFullPath(params);
            formGraph.OpenModal();
        });

        associatedByAccountTable.on('length.dt', function () {
            btnSearch.prop('disabled', true);
        });

        btnAddAssociation.on('click', function () {
            asscomment.val('');

            InitInputTagUsername(ctsCustid);            

            divAddAssociation.dialog({
                modal: true,
                width: '800px',
                title: 'ADD ASSOCIATION',
                resizable: true,
                draggable: true,
                close: function () {                    
                    $(this).dialog('close');
                }
            });            
        });        
        btnSaveAssociation.on('click', () => InsertManualAssociationMultiple(subscriberId, ctsCustid));
        btnCancelAssociation.on('click', () => {
            divAddAssociation.dialog('close');
        });    
    };
}