function transfer_account() {
    var domElementsTransAcc = {
        tblAssociatedGroup: $('#tblAssociatedGroup'),        
        divTransferAccount: $('#divTransferAccount'),
        spanTransferAccountUserName: $('#spanTransferAccountUserName'),
        cbbTransferAccountNewGroup: $('#cbbTransferAccountNewGroup'),
        spanTransferAccountGroupIDHidden: $('#spanTransferAccountGroupIDHidden'),
        spanTransferAccountGroupNameHidden: $('#spanTransferAccountGroupNameHidden'),
        spanTransferAccountUserNameMaxTag: $('#spanTransferAccountUserNameMaxTag'),
        spanTransferAccountChangeHidden: $('#spanTransferAccountChangeHidden'),
        inputTransferAccountTagUsername: $('#inputTransferAccountTagUsername'),        
        btnSaveTransferAccount: $('#btnSaveTransferAccount'),
        btnCancelTransferAccount: $('#btnCancelTransferAccount'),
        maxTags: 1001
    };

    function AddBorderTags() {
        domElementsTransAcc.spanTransferAccountUserName.find('tags').addClass('ctrlA');
        let allTags = domElementsTransAcc.spanTransferAccountUserName.find('tags tag.tagify__tag');
        allTags.each(function (_, tag) {
            $(tag).css('border', '1px solid #6187E7');
        });
    }

    function ResetMaxTag() {
        let currentTagsLength = domElementsTransAcc.spanTransferAccountUserName.find('tags tag').length;
        if (currentTagsLength < domElementsTransAcc.maxTags)
            domElementsTransAcc.spanTransferAccountUserNameMaxTag.text(domElementsTransAcc.maxTags - currentTagsLength + 1);
    }

    function EnableButtonAdd() {
        let validTags = domElementsTransAcc.spanTransferAccountUserName.find('tags tag.valid');
        let invalidDuplicatedTags = domElementsTransAcc.spanTransferAccountUserName.find('tags tag.invalid-duplicated');
        let invalidUsernameTags = domElementsTransAcc.spanTransferAccountUserName.find('tags tag.invalid-username');
        let notInvalid = invalidDuplicatedTags.length + invalidUsernameTags.length;
        if (notInvalid <= 1 && validTags.length > 1)
            domElementsTransAcc.btnSaveTransferAccount.prop('disabled', false);
        else
            domElementsTransAcc.btnSaveTransferAccount.prop('disabled', true);
    }

    function IsTextUnicode(text) {
        var pattern = /^[\x20-\x7E]*$/;
        return pattern.test(text);
    }

    function OnChangeTags(tagName, cssClass) {
        let maxCountTags = parseInt(domElementsTransAcc.spanTransferAccountUserNameMaxTag.text());
        let isTextUnicode = IsTextUnicode($(tagName.tag).text());

        if (!isTextUnicode) {
            domElementsTransAcc.inputTransferAccountTagUsername.tagify().data('tagify').removeTags(tagName.tag);
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
                ValidateTransferAccountInput(currentTag.text().trim());
                EnableButtonAdd();
            });

            domElementsTransAcc.spanTransferAccountUserNameMaxTag.text(maxCountTags - 1);
        } else {
            let $tag = $(tagName.tag);
            let btnRemove = $tag.find('x');
            btnRemove.on('click', function (e) {
                let currentTag = $(e.target).parent();

                ResetMaxTag();
                ValidateTransferAccountInput(currentTag.text().trim());
                EnableButtonAdd();
            });
        }
        domElementsTransAcc.btnSaveTransferAccount.prop('disabled', true);
    }

    function InitInputTransferAccountTagUsername() {
        domElementsTransAcc.spanTransferAccountUserNameMaxTag.text(domElementsTransAcc.maxTags);
        domElementsTransAcc.spanTransferAccountChangeHidden.text("0");        

        domElementsTransAcc.inputTransferAccountTagUsername.tagify({
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
            maxTags: domElementsTransAcc.maxTags,
            enforceWhitelist: false,
            dropdown: {
                position: "input",
                enabled: 0
            }
        }).off('add').on('add', function (_, tagName) {
            OnChangeTags(tagName, 'valid');
            domElementsTransAcc.spanTransferAccountChangeHidden.text("1");
        })
            .off('invalid').on('invalid', function (_, tagName) {
                OnChangeTags(tagName, 'invalid-duplicated');
                domElements.spanTransferAccountChangeHidden.text("1");
            });

        domElementsTransAcc.inputTransferAccountTagUsername.off('keydown').on('keydown', function () {
            let keyCode = event.keyCode;

            if (keyCode === 46) {
                if (domElementsTransAcc.spanTransferAccountUserName.find('tags').hasClass('ctrlA')) {
                    domElementsTransAcc.inputTransferAccountTagUsername.tagify().data('tagify').removeAllTags();
                    domElementsTransAcc.spanTransferAccountUserNameMaxTag.text(domElementsTransAcc.maxTags);
                    domElementsTransAcc.spanTransferAccountUserName.find('tags').removeClass('ctrlA');
                }
            }

            if (event.ctrlKey) {
                if (keyCode === 65 || keyCode === 97) {
                    return AddBorderTags();
                }
            }
        });
      
        domElementsTransAcc.inputTransferAccountTagUsername.tagify().data('tagify').removeAllTags();
        domElementsTransAcc.spanTransferAccountUserName.find('tags').attr('tabindex', "1");
        domElementsTransAcc.btnSaveTransferAccount.prop('disabled', true);
    }

    function ValidateTransferAccountInput(textTagRemove) {
        let numOfTagExceeded = domElementsTransAcc.spanTransferAccountUserName.find('tags tag.tagify--notAllowed');
        if (numOfTagExceeded.length > 0) return;

        domElementsTransAcc.btnSaveTransferAccount.prop('disabled', false);
        let userNames = GetTransferAccountInputUserName(textTagRemove);

        if (userNames !== '') {
            $.ajax(common.modifyRelativeUrl('AssociatedGroupMonitor/ValidateUsernameTransferAccount'), {
                type: 'POST',
                data: {
                    userNames: userNames,
                    groupID: domElementsTransAcc.spanTransferAccountGroupIDHidden.val()
                },
                beforeSend: function () {
                    domElementsTransAcc.spanTransferAccountUserName.addClass("loading-value");
                },
                success: function (response) {
                    BindResultValidatedTransferAccountInputUserName(response);
                },
                complete: function () {
                    domElementsTransAcc.spanTransferAccountUserName.removeClass("loading-value");
                },
                error: function (errMsg) {
                    domElementsTransAcc.spanTransferAccountUserName.removeClass("loading-value");
                    common.showError("An unexpected Error occurred. Please try again.");
                    console.log(errMsg);
                }
            });
        }
    }

    function GetTransferAccountInputUserName(textTagRemove) {
        let userNameList = [];
        let tags = domElementsTransAcc.spanTransferAccountUserName.find('tags tag');

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

    function BindResultValidatedTransferAccountInputUserName(data) {
        domElementsTransAcc.spanTransferAccountUserName.find('tags tag').each(function (_, tag) {
            $(tag).removeClass('invalid-username').removeClass('invalid-duplicated').addClass('valid');
        });

        data.AccountInvalidList.map(function (x) {
            let usernameEqual = $("tag[data-user-name='" + x.UserName + "']");
            if (usernameEqual !== undefined) {
                usernameEqual.removeClass('valid').addClass('invalid-username');
                usernameEqual.prop('title', x.ErrMsg);
            }
        });

        if (data.AccountInvalidList.length === 0) domElementsTransAcc.btnSaveTransferAccount.prop('disabled', false);
        else domElementsTransAcc.btnSaveTransferAccount.prop('disabled', true);
    }   

    function SaveTransferAccount() {
        let params = {};
        params.UserNames = GetTransferAccountInputUserName();
        params.FromGroupID = domElementsTransAcc.spanTransferAccountGroupIDHidden.val();
        params.ToGroupID = domElementsTransAcc.cbbTransferAccountNewGroup.val();

        let fromGroupName = domElementsTransAcc.spanTransferAccountGroupNameHidden.val();
        let toGroupName = domElementsTransAcc.cbbTransferAccountNewGroup.find('option:selected').text();

        common.showConfirmation("<div class='popup-content'><div class='popup-title'>Are you sure you want to TRANSFER the account(s) from '" + fromGroupName + "' to '" + toGroupName +"'?</div></div>",
            function () {
                $.ajax(common.modifyRelativeUrl('AssociatedGroupMonitor/SaveTransferAccount'), {
                    type: 'POST',
                    data: params,
                    beforeSend: function () {
                        domElementsTransAcc.tblAssociatedGroup.find('tbody').addClass('loading');
                    },
                    success: function (response) {
                        if (response.errorCode != -1) {
                            common.showSuccess('The account(s) transferred successfully.');
                            domElementsTransAcc.divTransferAccount.dialog('close');
                        }
                        else {
                            common.showError("An unexpected Error occurred. Please try again.");
                        }
                    },
                    complete: function () {
                        domElementsTransAcc.tblAssociatedGroup.find('tbody').removeClass('loading');
                    },
                    error: function (errMsg) {
                        domElementsTransAcc.tblAssociatedGroup.find('tbody').removeClass('loading');
                        common.showError("An unexpected Error occurred. Please try again.");
                        console.log(errMsg);
                    }
                });

            }
        )
    }

    function GetGroupTransferAccount(groupID) {
        $.ajax(common.modifyRelativeUrl('AssociatedGroupMonitor/GetGroupTransferAccount'), {
            type: 'POST',
            data: {
                groupID: groupID
            },
            success: function (response) {
                if (response != undefined) {
                    domElementsTransAcc.cbbTransferAccountNewGroup.find('option').remove().end();

                    response.map(function (x) {                       
                        domElementsTransAcc.cbbTransferAccountNewGroup.append(`<option value=${x.GroupID}>${x.GroupName}</option>`);
                    })
                }
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function RefreshTitleIsHtml() {
        $.widget("ui.dialog", $.extend({}, $.ui.dialog.prototype, {
            _title: function (title) {
                let $title = this.options.title || '&nbsp;'
                if (("titleIsHtml" in this.options) && this.options.titleIsHtml == true)
                    title.html($title);
                else title.text($title);
            }
        }));
    }

    function InitTransferAccount(groupID, groupName) {
        domElementsTransAcc.spanTransferAccountGroupIDHidden.val(groupID);
        domElementsTransAcc.spanTransferAccountGroupNameHidden.val(groupName);
        domElementsTransAcc.spanTransferAccountUserNameMaxTag.text(domElementsTransAcc.maxTags);
        domElementsTransAcc.spanTransferAccountChangeHidden.text("0");
        domElementsTransAcc.inputTransferAccountTagUsername.tagify().data('tagify').removeAllTags();
        domElementsTransAcc.spanTransferAccountUserName.find('tags').attr('tabindex', "1");
        domElementsTransAcc.btnSaveTransferAccount.prop('disabled', true);

        RefreshTitleIsHtml();

        GetGroupTransferAccount(groupID);

        domElementsTransAcc.divTransferAccount.dialog({
            modal: true,
            width: '700px',
            title: '<div>TRANSFER ACCOUNTS - <label style="color:#FF0000">' + groupName + '</label></div>',
            titleIsHtml: true,
            resizable: true,
            draggable: true,
            close: function () {
                $(this).dialog('close');
            }
        });
    }

    this.Mouseup = function (e) {
            let isOutSiteInput = !domElementsTransAcc.spanTransferAccountUserName.is(e.target) && domElementsTransAcc.spanTransferAccountUserName.has(e.target).length === 0;
            let isTagsFocus = domElementsTransAcc.spanTransferAccountUserName.find('tags.tagify--focus').length > 0;
            let isChange = domElementsTransAcc.spanTransferAccountChangeHidden.text();
            if (isOutSiteInput && isTagsFocus || isChange === "1") {              
                domElementsTransAcc.spanTransferAccountUserName.addClass("loading-value");
                setTimeout(() => {
                    ValidateTransferAccountInput("");
                    domElementsTransAcc.spanTransferAccountUserName.removeClass("loading-value");
                    domElementsTransAcc.spanTransferAccountChangeHidden.text("0");
                }, 500);
            }
    }

    this.Initial = function (groupID, groupName) {
        InitTransferAccount(groupID, groupName);

        domElementsTransAcc.btnCancelTransferAccount.on('click', () => domElementsTransAcc.divTransferAccount.dialog('close'));

        domElementsTransAcc.btnSaveTransferAccount.on('click', () => SaveTransferAccount());      
    }

    this.InitInputTransferAccountTagUsername = function() {
        InitInputTransferAccountTagUsername();
    }
}