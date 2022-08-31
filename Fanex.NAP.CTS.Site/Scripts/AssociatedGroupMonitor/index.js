function AssociatedGroupMonitor() {
    let dataTable;    
    let domElements = {
        btnAddGroup: $('#btnAddGroup'),
        tblAssociatedGroup: $('#tblAssociatedGroup'),        
        divAddAssociatedGroup: $('#divAddAssociatedGroup'),
        txtGroupNameAssociatedGroup: $('#txtGroupNameAssociatedGroup'),
        txtCommentAssociatedGroup: $('#txtCommentAssociatedGroup'),
        cbbSites: $('#cbbSites'),
        cbbAssociatedTypes: $('#cbbAssociatedTypes'),
        cbbABIAssociatedGroup: $('#cbbABIAssociatedGroup'),
        txtCommentAssociatedAccount: $('#txtCommentAssociatedAccount'),
        divAddAssociatedAccount: $('#divAddAssociatedAccount'),
        inputTagUsername: $('#inputTagUsername'),
        btnSaveAssociatedGroup: $('#btnSaveAssociatedGroup'),
        btnCancelAssociatedGroup: $('#btnCancelAssociatedGroup'),
        btnSaveAssociatedAccount: $('#btnSaveAssociatedAccount'),
        btnCancelAssociatedAccount: $('#btnCancelAssociatedAccount'),
        spanAddAssociatedAccountUserName: $('#spanAddAssociatedAccountUserName'),
        spanAddAssociatedAccountUserNameMaxTag: $('#spanAddAssociatedAccountUserNameMaxTag'),
        spanAddAssociatedAccountChangeHidden: $('#spanAddAssociatedAccountChangeHidden'),
        spanAddAssociatedAccountGroupIDHidden: $('#spanAddAssociatedAccountGroupIDHidden'),
        spanAddAssociatedAccountGroupNameHidden: $('#spanAddAssociatedAccountGroupNameHidden'),
        spanAddAssociatedAccountGroupABIHidden: $('#spanAddAssociatedAccountGroupABIHidden'),
        hdIsHostPermission: $('#hdIsHostPermission'),
        maxTags: 1001
    };

    let isHostPermission = domElements.hdIsHostPermission.attr('IsHostPermission') === "True";

    function RenderGrid() {
        let url = common.modifyRelativeUrl('AssociatedGroupMonitor/GetAssociatedGroupMonitor');

        return domElements.tblAssociatedGroup.DataTable({
            serverSide: false,
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            pagingType: "full_numbers",
            ajax: {
                url: url,               
                type: "POST",
                beforeSend: function () {
                    domElements.tblAssociatedGroup.find('tbody').addClass('loading');
                },
                complete: function () {
                    domElements.tblAssociatedGroup.find('tbody').removeClass('loading');
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
            lengthMenu: [20, 50, 100, 200],
            pageLength: 50,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5, 6, 7, 8 , 9], className: "text-center" },
                { targets: [0], width: "10%" },
                { targets: [1], width: "20%" },
                { targets: [2], width: "5%" },
                { targets: [3], width: "10%" },
                { targets: [4], width: "20%" },
                { targets: [5], width: "10%" },
                { targets: [6], width: "10%" },
                { targets: [7], width: "5%" },
                { targets: [8], width: "5%" },
                { targets: [9], width: "5%" }
            ],
            columns: [
                { data: obj => obj.GroupID },
                { data: obj => obj.GroupName },
                { data: obj => obj.ABI },
                { data: obj => obj.TotalAccount },
                { data: obj => obj.Remark },
                { data: obj => obj.CreatedDate },
                { data: obj => obj.CreatedBy },   
                {
                    data: obj => {
                        return '<span class="sub-icon - edit" id="sub-icon " title="Edit Group"><i class="icon-edit ' + (!isHostPermission ? ' disabled"' : '') + '"></i></span>';
                    }, createdCell: (cell, _, row) => {
                        if (isHostPermission) {
                            $('.icon-edit', cell).click(() => {

                            });
                        }
                    }
                },
                {
                    data: obj => {
                        return '<span class="sub-icon - delete" id="sub-icon " title="Remove Group"><i class="icon-delete ' + (!isHostPermission ? ' disabled"' : '') + '"></i></span>';
                    }, createdCell: (cell, _, row) => {
                        if (isHostPermission) {
                            $('.icon-delete', cell).click(() => {
                                $('td #sub-icon').addClass('none-pointer');
                                let html = "<div class='popup - content'>Are you sure you want to remove this group <strong>" + row.GroupName + "</strong>? </div>";

                                common.showConfirmation(html, function () {
                                    RemoveAssociatedGroupMonitor(row.GroupID, row.GroupName);
                                }, () => {
                                    $('td #sub-icon').removeClass('none-pointer');
                                });
                            });
                        }
                    }
                },                
                {
                    data: obj => {
                        return '<span class="sub-icon - edit" id="sub-icon " title="Add Accounts"><i class="icon-add-account' + (!isHostPermission ? ' disabled"' : '') + '"></i></span>';
                    }, createdCell: (cell, _, row) => {
                        $('.icon-add-account', cell).click(() => {
                            $('td #sub-icon').addClass('none-pointer');
                            
                            domElements.spanAddAssociatedAccountGroupIDHidden.val(row.GroupID);
                            domElements.spanAddAssociatedAccountGroupNameHidden.val(row.GroupName);
                            domElements.spanAddAssociatedAccountGroupABIHidden.val(row.ABI);                            
                            domElements.txtCommentAssociatedAccount.val('');
                            domElements.spanAddAssociatedAccountUserNameMaxTag.text(domElements.maxTags);
                            domElements.spanAddAssociatedAccountChangeHidden.text("0");
                            domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
                            domElements.spanAddAssociatedAccountUserName.find('tags').attr('tabindex', "1");
                            domElements.btnSaveAssociatedAccount.prop('disabled', true);                           

                            $.widget("ui.dialog", $.extend({}, $.ui.dialog.prototype, {
                                _title: function (title) {
                                    let $title = this.options.title || '&nbsp;'
                                    if (("titleIsHtml" in this.options) && this.options.titleIsHtml == true)
                                        title.html($title);
                                    else title.text($title);
                                }
                            }));

                            domElements.divAddAssociatedAccount.dialog({
                                modal: true,
                                width: '700px',
                                title: '<div>ADD NEW ACCOUNTS - <label style="color:#FF0000">' + row.GroupName + '</label></div>',
                                titleIsHtml: true,
                                resizable: true,
                                draggable: true,
                                close: function () {
                                    $(this).dialog('close');
                                }
                            });
                        });
                    }
                },
                {
                    data: obj => {
                        return '<span class="sub-icon - excel" id="sub-icon " title="Export To Excel"><i class="icon-excel ' + (!isHostPermission ? ' disabled"' : '') + '"></i></span>';
                    }, createdCell: (cell, _, row) => {
                        $('.icon-excel', cell).click(() => {
                            $('td #sub-icon').addClass('none-pointer');

                            ExportToExcelAssociatedAccount(row.GroupID, row.GroupName);
                        })
                    }
                }
            ]
        });
    }

    function GetAssociatedGroup() {
        dataTable
            ? dataTable.ajax.reload(null, true)
            : dataTable = RenderGrid();
    }

    function ExportToExcelAssociatedAccount(groupID, groupName) {
        let params = {};
        params.GroupID = groupID;
        params.GroupName = groupName;

        common.showSuccess('Your filter export is being processed. Please wait, this may take some minutes.', 5000);

        common.showLoading();

        let request = new XMLHttpRequest();
        request.open('POST', common.modifyRelativeUrl('AssociatedGroupMonitor/ExportToExcelAssociatedAccount'), true);
        request.setRequestHeader('Content-type', 'application/json');
        request.responseType = 'blob';
        request.timeout = 600000;
        request.addEventListener("timeout", function (e) {
            common.closeLoading();
            common.showError("Export failed (timeout)");
        });
        request.onload = function (e) {
            if (this.status === 200) {
                let extension = ".xlsx";
                let date = GetFormattedDate();
                let filenameFormat = "Group_{0}{1}";
                let filename = filenameFormat.format(date, extension);
                let blob = this.response;

                if (window.navigator.msSaveOrOpenBlob) {
                    window.navigator.msSaveBlob(blob, filename);
                } else {
                    let downloadLink = window.document.createElement('a');
                    let contentTypeHeader = request.getResponseHeader("Content-Type");
                    downloadLink.href = window.URL.createObjectURL(new Blob([blob], { type: contentTypeHeader }));

                    var disposition = request.getResponseHeader('Content-Disposition');
                    if (disposition && disposition.indexOf('filename') !== -1) {
                        var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                        var matches = filenameRegex.exec(disposition);
                        if (matches != null && matches[1]) {
                            filename = matches[1].replace(/['"]/g, '');
                        }
                    }

                    downloadLink.download = filename;
                    document.body.appendChild(downloadLink);
                    downloadLink.click();
                    document.body.removeChild(downloadLink);
                }
            } else {
                console.log(this);
                common.showError("Export failed");
            }

            common.closeLoading();
        };       
        request.send(JSON.stringify(params));      
    }

    function GetFormattedDate() {
        let date = new Date();

        let stringDateFormat = date.getUTCFullYear().toString()
            + ('0' + (date.getUTCMonth() + 1)).slice(-2).toString()
            + ('0' + date.getUTCDate()).slice(-2).toString()
            + '_'
            + ('0' + date.getUTCHours()).slice(-2).toString()
            + ('0' + date.getUTCMinutes()).slice(-2).toString()
            + ('0' + date.getUTCSeconds()).slice(-2).toString();

        return stringDateFormat;
    }

    function RemoveAssociatedGroupMonitor(groupID, groupName) {
        let params = {};
        params.GroupID = groupID;       

        $.ajax(common.modifyRelativeUrl('AssociatedGroupMonitor/RemoveAssociatedGroupMonitor'), {
            type: 'POST',
            data: params,
            beforeSend: function () {
                domElements.tblAssociatedGroup.find('tbody').addClass('loading');
            },
            success: function (response) {
                if (response.errorCode != -1) {
                    common.showSuccess("Group " + groupName + " is removed successfully.");                  

                    GetAssociatedGroup();
                } else {
                    common.showError("An unexpected Error occurred. Please try again.");
                }
            },
            complete: function () {
                domElements.tblAssociatedGroup.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                domElements.tblAssociatedGroup.find('tbody').removeClass('loading');
                common.showError("An unexpected Error occurred. Please try again.");
                console.log(errMsg);
            }
        });
    }

    function AddAssociatedGroupMonitor() {
        if (ValidateInputAddAssociatedGroupMonitor()) {
            common.showConfirmation("<div class='popup-content'><div class='popup-title'>Are you sure you want to add new group '" + domElements.txtGroupNameAssociatedGroup.val().trim() +"'</div></div>",
                function () {
                    let params = {};
                    params.GroupName = domElements.txtGroupNameAssociatedGroup.val().trim();
                    params.ABI = domElements.cbbABIAssociatedGroup.val();
                    params.Comment = domElements.txtCommentAssociatedGroup.val().trim();

                    $.ajax(common.modifyRelativeUrl('AssociatedGroupMonitor/AddAssociatedGroupMonitor'), {
                        type: 'POST',
                        data: params,
                        beforeSend: function () {
                            domElements.tblAssociatedGroup.find('tbody').addClass('loading');
                        },
                        success: function (response) {
                            if (response.errorCode != -1) {
                                common.showSuccess("New group " + params.GroupName + " is added successfully.");
                                domElements.divAddAssociatedGroup.dialog('close');

                                GetAssociatedGroup();
                            } else {
                                common.showError("An unexpected Error occurred. Please try again.");
                            }
                        },
                        complete: function () {
                            domElements.tblAssociatedGroup.find('tbody').removeClass('loading');
                        },
                        error: function (errMsg) {
                            domElements.tblAssociatedGroup.find('tbody').removeClass('loading');
                            common.showError("An unexpected Error occurred. Please try again.");
                            console.log(errMsg);
                        }
                    });
                }
            )
        }       
    }

    function ValidateInputAddAssociatedGroupMonitor() {
        if (domElements.txtGroupNameAssociatedGroup.val() == "") {
            common.showError('Group Name may not be empty.');
            return false;
        }

        let pattern = /^[a-zA-Z0-9_.-\s]*$/;
        let groupName = domElements.txtGroupNameAssociatedGroup.val();
      
        if (!pattern.test(RemoveAscent(groupName))) {
            common.showError('Group Name "' + groupName + '" is invalid.');
            return false;
        }
        
        if (domElements.cbbABIAssociatedGroup.val() == "" || domElements.cbbABIAssociatedGroup.val() == null) {
            common.showError('ABI may not be empty.');
            return false;
        }

        return true;
    }

    function RemoveAscent(text) {
        if (text === null || text === undefined)
            return text;
        text = text.toLowerCase();
        text = text.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
        text = text.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
        text = text.replace(/ì|í|ị|ỉ|ĩ/g, "i");
        text = text.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
        text = text.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
        text = text.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
        text = text.replace(/đ/g, "d");

        return text;
    }

    function AddAssociatedAccount() {
        let params = {};
        params.GroupID = domElements.spanAddAssociatedAccountGroupIDHidden.val();
        params.GroupName = domElements.spanAddAssociatedAccountGroupNameHidden.val();
        params.ABI = domElements.spanAddAssociatedAccountGroupABIHidden.val();
        params.UserNames = GetInputUserName();
        params.Comment = domElements.txtCommentAssociatedAccount.val().trim();

        common.showConfirmation("<div class='popup-content'><div class='popup-title'>Are you sure you want to add new account(s) into group '" + params.GroupName  + "'?</div></div>",
            function () {               
                $.ajax(common.modifyRelativeUrl('AssociatedGroupMonitor/AddAssociatedAccount'), {
                    type: 'POST',
                    data: params,
                    beforeSend: function () {
                        domElements.tblAssociatedGroup.find('tbody').addClass('loading');
                    },
                    success: function (response) {
                        if (response.errorCode != -1) {
                            common.showSuccess("New account(s) added into group " + params.GroupName + " successfully.");
                            domElements.divAddAssociatedAccount.dialog('close');

                            GetAssociatedGroup();
                        }
                        else {
                            common.showError("An unexpected Error occurred. Please try again.");
                        }
                    },
                    complete: function () {
                        domElements.tblAssociatedGroup.find('tbody').removeClass('loading');
                    },
                    error: function (errMsg) {
                        domElements.tblAssociatedGroup.find('tbody').removeClass('loading');
                        common.showError("An unexpected Error occurred. Please try again.");
                        console.log(errMsg);
                    }
                });

            }
        )        
    }

    function InitInputTagUsername() {
        domElements.spanAddAssociatedAccountUserNameMaxTag.text(domElements.maxTags);
        domElements.spanAddAssociatedAccountChangeHidden.text("0");

        function AddBorderTags() {
            domElements.spanAddAssociatedAccountUserName.find('tags').addClass('ctrlA');
            let allTags = domElements.spanAddAssociatedAccountUserName.find('tags tag.tagify__tag');
            allTags.each(function (_, tag) {
                $(tag).css('border', '1px solid #6187E7');
            });
        }

        function ResetMaxTag() {
            let currentTagsLength = domElements.spanAddAssociatedAccountUserName.find('tags tag').length;
            if (currentTagsLength < domElements.maxTags)
                domElements.spanAddAssociatedAccountUserNameMaxTag.text(domElements.maxTags - currentTagsLength + 1);
        }

        function EnableButtonAdd() {
            let validTags = domElements.spanAddAssociatedAccountUserName.find('tags tag.valid');
            let invalidDuplicatedTags = domElements.spanAddAssociatedAccountUserName.find('tags tag.invalid-duplicated');
            let invalidUsernameTags = domElements.spanAddAssociatedAccountUserName.find('tags tag.invalid-username');
            let notInvalid = invalidDuplicatedTags.length + invalidUsernameTags.length;
            if (notInvalid <= 1 && validTags.length > 1)
                domElements.btnSaveAssociatedAccount.prop('disabled', false);
            else
                domElements.btnSaveAssociatedAccount.prop('disabled', true);
        }

        function IsTextUnicode(text) {
            var pattern = /^[\x20-\x7E]*$/;
            return pattern.test(text);
        }

        function OnChangeTags(tagName, cssClass) {
            let maxCountTags = parseInt(domElements.spanAddAssociatedAccountUserNameMaxTag.text());
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
                    ValidateAddAssociatedAccountInput(currentTag.text().trim());
                    EnableButtonAdd();
                });

                domElements.spanAddAssociatedAccountUserNameMaxTag.text(maxCountTags - 1);
            } else {              
                let $tag = $(tagName.tag);
                let btnRemove = $tag.find('x');
                btnRemove.on('click', function (e) {
                    let currentTag = $(e.target).parent();

                    ResetMaxTag();
                    ValidateAddAssociatedAccountInput(currentTag.text().trim());
                    EnableButtonAdd();
                });
            }
            domElements.btnSaveAssociatedAccount.prop('disabled', true);
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
            maxTags: domElements.maxTags,
            enforceWhitelist: false,
            dropdown: {
                position: "input",
                enabled: 0
            }
        }).off('add').on('add', function (_, tagName) {
            OnChangeTags(tagName, 'valid');
            domElements.spanAddAssociatedAccountChangeHidden.text("1");
        })
        .off('invalid').on('invalid', function (_, tagName) {
            OnChangeTags(tagName, 'invalid-duplicated');
            domElements.spanAddAssociatedAccountChangeHidden.text("1");
        });

        domElements.inputTagUsername.off('keydown').on('keydown', function () {
            let keyCode = event.keyCode;

            if (keyCode === 46) {
                if (domElements.spanAddAssociatedAccountUserName.find('tags').hasClass('ctrlA')) {
                    domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
                    domElements.spanAddAssociatedAccountUserNameMaxTag.text(domElements.maxTags);
                    domElements.spanAddAssociatedAccountUserName.find('tags').removeClass('ctrlA');
                }
            }

            if (event.ctrlKey) {
                if (keyCode === 65 || keyCode === 97) {
                    return AddBorderTags();
                }
            }
        });

        $(document).on('mouseup', function (e) {
            let isOutSiteInput = !domElements.spanAddAssociatedAccountUserName.is(e.target) && domElements.spanAddAssociatedAccountUserName.has(e.target).length === 0;
            let isTagsFocus = domElements.spanAddAssociatedAccountUserName.find('tags.tagify--focus').length > 0;
            let isChange = domElements.spanAddAssociatedAccountChangeHidden.text();
            if (isOutSiteInput && isTagsFocus || isChange === "1") {
                domElements.txtCommentAssociatedAccount.focus();
                domElements.spanAddAssociatedAccountUserName.addClass("loading-value");
                setTimeout(() => {
                    ValidateAddAssociatedAccountInput("");
                    domElements.spanAddAssociatedAccountUserName.removeClass("loading-value");
                    domElements.spanAddAssociatedAccountChangeHidden.text("0");
                }, 500);
            }
        });

        domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
        domElements.spanAddAssociatedAccountUserName.find('tags').attr('tabindex', "1");
        domElements.btnSaveAssociatedAccount.prop('disabled', true);
    }

    function ValidateAddAssociatedAccountInput(textTagRemove) {
        let numOfTagExceeded = domElements.spanAddAssociatedAccountUserName.find('tags tag.tagify--notAllowed');

        if (numOfTagExceeded.length > 0) {
            return;
        }

        domElements.btnSaveAssociatedAccount.prop('disabled', false);
        let userNames = GetInputUserName(textTagRemove);

        if (userNames !== '') {            
            $.ajax(common.modifyRelativeUrl('AssociatedGroupMonitor/ValidateUserNameInput'), {
                type: 'POST',
                data: {
                    userNames: userNames
                },
                beforeSend: function () {
                    domElements.spanAddAssociatedAccountUserName.addClass("loading-value");
                },
                success: function (response) {
                    BindResultValidatedInputUserName(response);
                },
                complete: function () {
                    domElements.spanAddAssociatedAccountUserName.removeClass("loading-value");
                },
                error: function (errMsg) {
                    domElements.spanAddAssociatedAccountUserName.removeClass("loading-value");
                    common.showError("An unexpected Error occurred. Please try again.");
                    console.log(errMsg);
                }
            });
        }
    }

    function GetInputUserName(textTagRemove) {
        let userNameList = [];
        let tags = domElements.spanAddAssociatedAccountUserName.find('tags tag');

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

    function BindResultValidatedInputUserName(data) {
        domElements.spanAddAssociatedAccountUserName.find('tags tag').each(function (_, tag) {
            $(tag).removeClass('invalid-username').removeClass('invalid-duplicated').addClass('valid');
        });

        data.AccountInvalidList.map(function (x) {
            let usernameEqual = $("tag[data-user-name='" + x.UserName + "']");
            if (usernameEqual !== undefined) {
                usernameEqual.removeClass('valid').addClass('invalid-username');
                usernameEqual.prop('title', x.ErrMsg);
            }
        });

        if (data.AccountInvalidList.length === 0) {
            domElements.btnSaveAssociatedAccount.prop('disabled', false);
        }
        else
            domElements.btnSaveAssociatedAccount.prop('disabled', true);
    }   

    function Initial() {
        if (!isHostPermission) {
            domElements.btnAddGroup.prop('disabled', true);
        }

        domElements.cbbSites.multiselect({
            menuWidth: 270,
            menuHeight: 320,
            selectedText: "# of # selected",
            noneSelectedText: "Select Sites",
            enableClickableOptGroups: true,
            includeSelectAllOption: true,
            groupsCollapsable: true,
            optGroupChecked: true
        }).multiselectfilter({
            width: 210,
            placeholder: '',
            label: ''
        });

        domElements.cbbAssociatedTypes.multiselect({
            menuWidth: 190,
            menuHeight: 190,
            selectedText: "# of # selected",
            noneSelectedText: "Select Associated Types"
        });

        GetAssociatedGroup();

        domElements.btnAddGroup.on('click', function () {   
            $('#cbbABIAssociatedGroup').val(0).trigger('change.select2');
            domElements.txtGroupNameAssociatedGroup.val('');                       
            domElements.txtCommentAssociatedGroup.val('');

            if ($.ui && $.ui.dialog && $.ui.dialog.prototype._allowInteraction) {
                var ui_dialog_interaction = $.ui.dialog.prototype._allowInteraction;
                $.ui.dialog.prototype._allowInteraction = function (e) {
                    if ($(e.target).closest('.select2-dropdown').length) return true;
                    return ui_dialog_interaction.apply(this, arguments);
                };
            }

            domElements.divAddAssociatedGroup.dialog({
                modal: true,
                width: '700px',
                title: 'CREATE NEW GROUP',
                _allowInteraction: function (event) {
                    return !!$(event.target).is(".select2-input") || this._super(event);
                },
                resizable: true,
                draggable: true,
                close: function () {
                    $(this).dialog('close');
                }
            });           
        });

        domElements.btnSaveAssociatedGroup.on('click', () => AddAssociatedGroupMonitor());

        domElements.btnCancelAssociatedGroup.on('click', () => { domElements.divAddAssociatedGroup.dialog('close') });

        domElements.cbbABIAssociatedGroup.select2();        
       
        domElements.btnSaveAssociatedAccount.on('click', () => AddAssociatedAccount());

        domElements.btnCancelAssociatedAccount.on('click', () => { domElements.divAddAssociatedAccount.dialog('close') });

        InitInputTagUsername();
    }

    $(document).ready(function () {
        Initial();
    });
}

$(AssociatedGroupMonitor);