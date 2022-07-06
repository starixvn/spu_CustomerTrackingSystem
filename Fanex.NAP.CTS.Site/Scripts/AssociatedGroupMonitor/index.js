function AssociatedGroupMonitor() {
    let dataTable;    
    let domElements = {
        btnAddGroup: $('#btnAddGroup'),
        tblAssociatedGroup: $('#tblAssociatedGroup'),        
        divAddAssociatedGroup: $('#divAddAssociatedGroup'),
        txtGroupNameAssociatedGroup: $('#txtGroupNameAssociatedGroup'),
        txtCommentAssociatedGroup: $('#txtCommentAssociatedGroup'),
        cbbABIAssociatedGroup: $('#cbbABIAssociatedGroup'),
        txtCommentAssociatedAccount: $('#txtCommentAssociatedAccount'),
        divAddAssociatedAccount: $('#divAddAssociatedAccount'),
        inputTagUsername: $('#inputTagUsername'),
        btnSaveAssociatedGroup: $('#btnSaveAssociatedGroup'),
        btnCancelAssociatedGroup: $('#btnCancelAssociatedGroup'),
        btnSaveAssociatedAccount: $('#btnSaveAssociatedAccount'),
        btnCancelAssociatedAccount: $('#btnCancelAssociatedAccount'),
        spanAddAssociatedGroupUserName: $('#spanAddAssociatedGroupUserName'),
        spanAddAssociatedGroupUserNameMaxTag: $('#spanAddAssociatedGroupUserNameMaxTag'),
        spanAddAssociatedGroupChangeHidden: $('#spanAddAssociatedGroupChangeHidden'),
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
                { targets: [1], width: "10%" },
                { targets: [2], width: "10%" },
                { targets: [3], width: "10%" },
                { targets: [4], width: "10%" },
                { targets: [5], width: "10%" },
                { targets: [6], width: "10%" },
                { targets: [7], width: "10%" },
                { targets: [8], width: "10%" },
                { targets: [9], width: "10%" }                
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
                        return html = '<span class="sub-icon - delete" id="sub-icon " title="Remove Group"><i class="icon-delete ' + (!isHostPermission ? ' disabled"' : '') + '"></i></span>';
                    }
                },
                {
                    data: obj => {
                        return html = '<span class="sub-icon - delete" id="sub-icon " title="Remove Group"><i class="icon-delete ' + (!isHostPermission ? ' disabled"' : '') + '"></i></span>';
                    }, createdCell: (cell, _, row) => {
                        if (isHostPermission) {
                            $('.icon-delete', cell).click(() => {
                                $('td #sub-icon').addClass('none-pointer');
                                let html = "<div class='popup - content'>Do you want remove <strong>" + row.GroupName + "</strong> from Associated Group Monitor list? </div>";

                                common.showConfirmation(html, function () {
                                    RemoveAssociatedGroupMonitor(row.GroupID);
                                }, () => {
                                    $('td #sub-icon').removeClass('none-pointer');
                                });
                            });
                        }
                    }
                },                
                {
                    data: obj => {
                        return html = '<span class="sub-icon - delete" id="sub-icon " title="Add Accounts"><i class="icon-delete ' + (!isHostPermission ? ' disabled"' : '') + '"></i></span>';
                    }, createdCell: (cell, _, row) => {
                        $('.icon-delete', cell).click(() => {
                            $('td #sub-icon').addClass('none-pointer');
                          
                            domElements.txtCommentAssociatedAccount.val('');
                            domElements.divAddAssociatedAccount.dialog({
                                modal: true,
                                width: '600px',
                                title: 'CREATE NEW MEMBERS',
                                resizable: true,
                                draggable: true,
                                close: function () {
                                    $(this).dialog('close');
                                }
                            });
                        });                        
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

    function RemoveAssociatedGroupMonitor(groupID) {
        let params = {};
        params.GroupID = groupID;       

        $.ajax(common.modifyRelativeUrl('AssociatedGroupMonitor/RemoveAssociatedGroupMonitor'), {
            type: 'POST',
            data: params,
            beforeSend: function () {
                domElements.tblAssociatedGroup.find('tbody').addClass('loading');
            },
            success: function (response) {
                console.log('successful');
            },
            complete: function () {
                domElements.tblAssociatedGroup.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                domElements.tblAssociatedGroup.find('tbody').removeClass('loading');
                console.log(errMsg);
            }
        });
    }

    function AddAssociatedGroupMonitor() {
        common.showConfirmation("<div class='popup-content'><div class='popup-title'>Do you want to add Associated Group?</div></div>",
            function () {
                if (ValidateInputAddAssociatedGroupMonitor()) {
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
                                common.showSuccess("Add Associated Group successful!");
                                domElements.divAddAssociatedGroup.dialog('close');
                            }
                        },
                        complete: function () {
                            domElements.tblAssociatedGroup.find('tbody').removeClass('loading');
                        },
                        error: function (errMsg) {
                            domElements.tblAssociatedGroup.find('tbody').removeClass('loading');
                            console.log(errMsg);
                        }
                    });
                }                
            }
        );        
    }

    function ValidateInputAddAssociatedGroupMonitor() {
        if (domElements.txtGroupNameAssociatedGroup.val() === "") {
            common.showError('Group Name "' + domElements.txtGroupNameAssociatedGroup.val() + '" is invalid.');
            return false;
        }

        if (domElements.cbbABIAssociatedGroup.val() === "") {
            common.showError('ABI "' + domElements.cbbABIAssociatedGroup.val() + '" is invalid.');
            return false;
        }       

        return true;
    }

    function AddAssociatedAccount(groupID) {
        let params = {};
        params.GroupID = groupID;
        //params.UserNames = domElements.cbbABIAssociatedGroup.val();
        params.Comment = domElements.txtCommentAssociatedGroup.val().trim();       

        $.ajax(common.modifyRelativeUrl('AssociatedGroupMonitor/AddAssociatedGroupMonitor'), {
            type: 'POST',
            data: params,
            beforeSend: function () {
                domElements.tblAssociatedGroup.find('tbody').addClass('loading');
            },
            success: function (response) {
                if (response.errorCode != -1) {
                    common.showSuccess("Add Associated Account successful!");
                    domElements.divAddAssociatedAccount.dialog('close');
                }
            },
            complete: function () {
                domElements.tblAssociatedGroup.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                domElements.tblAssociatedGroup.find('tbody').removeClass('loading');
                console.log(errMsg);
            }
        });
    }

    function InitInputTagUsername() {
        domElements.spanAddAssociatedGroupUserNameMaxTag.text(domElements.maxTags);
        domElements.spanAddAssociatedGroupChangeHidden.text("0");

        function AddBorderTags() {
            domElements.spanAddAssociatedGroupUserName.find('tags').addClass('ctrlA');
            let allTags = domElements.spanAddAssociatedGroupUserName.find('tags tag.tagify__tag');
            allTags.each(function (_, tag) {
                $(tag).css('border', '1px solid #6187E7');
            });
        }

        function ResetMaxTag() {
            let currentTagsLength = domElements.spanAddAssociatedGroupUserName.find('tags tag').length;
            if (currentTagsLength < domElements.maxTags)
                domElements.spanAddAssociatedGroupUserNameMaxTag.text(domElements.maxTags - currentTagsLength + 1);
        }

        function EnableButtonAdd() {
            let validTags = domElements.spanAddAssociatedGroupUserName.find('tags tag.valid');
            let invalidDuplicatedTags = domElements.spanAddAssociatedGroupUserName.find('tags tag.invalid-duplicated');
            let invalidUsernameTags = domElements.spanAddAssociatedGroupUserName.find('tags tag.invalid-username');
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
            let maxCountTags = parseInt(domElements.spanAddAssociatedGroupUserNameMaxTag.text());
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
                    //ValidateManualAssociationMultiple(currentTag.text().trim());
                    EnableButtonAdd();
                });

                domElements.spanAddAssociatedGroupUserNameMaxTag.text(maxCountTags - 1);
            } else {
                inputTagUsername.tagify().data('tagify').removeTags(tagName.tag);
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
            domElements.spanAddAssociatedGroupChangeHidden.text("1");
        })
        .off('invalid').on('invalid', function (_, tagName) {
            OnChangeTags(tagName, 'invalid-duplicated');
            domElements.spanAddAssociatedGroupChangeHidden.text("1");
        });

        domElements.inputTagUsername.off('keydown').on('keydown', function () {
            let keyCode = event.keyCode;

            if (keyCode === 46) {
                if (domElements.spanAddAssociatedGroupUserName.find('tags').hasClass('ctrlA')) {
                    domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
                    domElements.spanAddAssociatedGroupUserNameMaxTag.text(domElements.maxTags);
                    domElements.spanAddAssociatedGroupUserName.find('tags').removeClass('ctrlA');
                }
            }

            if (event.ctrlKey) {
                if (keyCode === 65 || keyCode === 97) {
                    return AddBorderTags();
                }
            }
        });

        $(document).on('mouseup', function (e) {
            let isOutSiteInput = !domElements.spanAddAssociatedGroupUserName.is(e.target) && domElements.spanAddAssociatedGroupUserName.has(e.target).length === 0;
            let isTagsFocus = domElements.spanAddAssociatedGroupUserName.find('tags.tagify--focus').length > 0;
            let isChange = domElements.spanAddAssociatedGroupChangeHidden.text();
            if (isOutSiteInput && isTagsFocus || isChange === "1") {
                domElements.txtCommentAssociatedAccount.focus();
                domElements.spanAddAssociatedGroupUserName.addClass("loading-value");
                setTimeout(() => {
                    //ValidateManualAssociationMultiple("");
                    domElements.spanAddAssociatedGroupUserName.removeClass("loading-value");
                    domElements.spanAddAssociatedGroupChangeHidden.text("0");
                }, 500);
            }
        });

        domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
        domElements.spanAddAssociatedGroupUserName.find('tags').attr('tabindex', "1");
        domElements.btnSaveAssociatedAccount.prop('disabled', true);
    }

    function IsDisableButton() {
        IsHostPermission = domElements.hdIsHostPermission.attr('IsHostPermission') === "True";

        if (!IsHostPermission) {
            domElements.btnAddGroup.prop('disabled', true);
        }
    }

    function Initial() {
        IsDisableButton();

        GetAssociatedGroup();

        domElements.btnAddGroup.on('click', function () {
            domElements.txtGroupNameAssociatedGroup.val('');
            domElements.txtCommentAssociatedGroup.val('');
                      
            domElements.divAddAssociatedGroup.dialog({
                modal: true,
                width: '600px',
                title: 'CREATE NEW GROUP',
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