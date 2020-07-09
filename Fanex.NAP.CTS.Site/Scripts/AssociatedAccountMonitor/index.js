function AssociatedAccountMonitor() {
    let dataTable;
    let userID;
    let IsHostPermission;
    let validCTSCustID;

    let domElements = {
        cbbSubscriber: $('#cbbSubscriber'),
        btnAddUsername: $('#btnAddUsername'),
        txtAsscomment: $('#txtasscomment'),
        inputTagUsername: $('#inputTagUsername'),
        btnsaveusername: $('#btnsaveusername'),
        btncancelusername: $('#btncancelusername'),
        asociatedAccountMonitorTable: $('#asociatedAccountMonitorTable'),
        btnIconDelete: $('.sub-icon.-delete'),
        divAddUsername: $('#divAddUsername'),
        spanAddAssociationUserName : $('#spanAddAssociationUserName'),
        spanAddAssociationUserNameMaxTag : $('#spanAddAssociationUserNameMaxTag'),
        spanAddAssociationChangeHidden: $('#spanAddAssociationChangeHidden'),
        maxTags : 50
    };

    function RenderGrid() {
        var url = common.modifyRelativeUrl('AssociatedAccountMonitor/GetAssociatedAccountMonitor');

        return domElements.asociatedAccountMonitorTable.DataTable({
            serverSide: true,
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            pagingType: "full_numbers",
            ajax: {
                url: url,
                data: {
                },
                type: "POST",
                beforeSend: function () {
                    domElements.asociatedAccountMonitorTable.find('tbody').addClass('loading');
                },
                dataFilter: function (response) {
                    var json = jQuery.parseJSON(response);

                    RefreshFilter(json.data.UserName, json.data.Subscriber);

                    var newJson = {};
                    newJson.data = json.data.AssociatedAccount;
                    newJson.recordsTotal = json.recordsTotal;
                    newJson.recordsFiltered = json.recordsFiltered;

                    return JSON.stringify(newJson);
                },
                complete: function () {
                    domElements.asociatedAccountMonitorTable.find('tbody').removeClass('loading');
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
                { targets: [0, 1, 2, 3, 4, 5, 6, 7], className: "text-center" },
                { targets: [0], width: "2%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "12%" },
                { targets: [3], width: "12%" },
                { targets: [4], width: "12%" },
                { targets: [5], width: "14%" },
                { targets: [6], width: "11%" },
                { targets: [7], width: "11%" },
                { targets: [8], width: "11%" }
            ],
            columns: [
                {
                    render: function (data, type, full, meta) { return meta.row + 1; }
                },
                { data: obj => () => { return "<span class=currency data-currency-username=" + obj.Username + ">" + obj.Username + "</span>"; } },
                //{ data: obj => obj.Username },
                { data: obj => obj.Username2 },
                { data: obj => obj.AccountStatus },
                { data: obj => () => { return "<span class=subcriberGroup data-subcriber-group=" + obj.SubscriberName + ">" + obj.SubscriberName + "</span>"; } },
                //{ data: obj => obj.SubscriberName },
                { data: obj => obj.Remark },
                { data: obj => obj.CreatedBy },
                { data: obj => obj.CreatedTime},
                {
                //    'data': 'action',
                //    createdCell: (cell, _, row) => {
                //        let title = 'Confirmation';
                //        let text = '<div class="popup - content">Do you want remove <strong>CN88$Kevin </strong> from Associated Account Monitor list? </div>';
                //        $('.sub-icon.-delete', cell).click(() =>
                //            CreateFormConfirmation(text, title, function () {

                //            })
                //        );
                //    }
                    data: obj => () => {
                        var html = '<span class="sub-icon - delete' + (!IsHostPermission ? ' disabled"' : '') + '" title="Remove Category"><i class="icon-delete"></i></span>';
    
                        return obj.IsEnabledDelete ? html : '';
                    }, createdCell: (cell, _, row) => {
                        if (IsHostPermission) {
                            $('.icon-delete', cell).click(() => {
                                common.showConfirmation("Do you want to remove category <b>" + row.CategoryName + "</b> for <b>" + username + "</b> ?", function () {
                                    RemoveCustomerClassification(custId, ctsCustId, row.CategoryId);
                                });

                            });

                            $('.icon-edit', cell).click(() => {
                                EditCategory(custId, ctsCustId, subscriberId, row.CategoryName, row.CategoryId, row.SportGroupIdList, row.Remark);
                            });
                        }
                    }

                }
            ]
        });
    }


    function RefreshFilter(userName, subscriberName) {
        $('option', domElements.cbbSubscriber).filter(function () {
            return $(this).val() !== '';
        }).remove();

        subscriberName.map(function (x) {
            domElements.cbbSubscriber.append(`<option selected value='${x}'>${x}</option>`);
        });

        domElements.cbbSubscriber.multiselect("refresh");

    }

    function RenderAction() {
        let html = "<span class='sub-icon -delete'><i class='icon-delete'></i></span>";

        return html;
    }

    function LoadGrid() {
        $(this).prop('disabled', true);

        dataTable
            ? dataTable.ajax.reload(null, true)
            : dataTable = RenderGrid();
    }

    function CreateFormConfirmation(html, title, yesCallback, noCallback) {

        var formconfirmation = $("#formconfirmation").dialog({
            autoOpen: false,
            modal: true,
            draggable: true,
            resizable: false,
        });

        formconfirmation.empty().html(html).dialog("open");

        var yesBtn =
        {
            'class': 'button btn-cancel-outline',
            text: "Yes",
            click: function () {
                formconfirmation.dialog("close");

                if (yesCallback !== null) {
                    yesCallback(this)
                }
            }
        };

        var noBtn =
        {
            'class': 'button -add',
            text: "No",
            click: function () {
                formconfirmation.dialog("close");

                if (noCallback !== null) {
                    noCallback()
                }
            }
        }

        formconfirmation.dialog("option", {
            dialogClass: "dialog-confirmation-associated",
            title: title,
            buttons: [yesBtn, noBtn],
            width: 460,
            close: noCallback
        });
    }

    function ValidateManualAssociationMultiple(textTagRemove) {   //TODO input then space, enter press ??? -> still not handle
        domElements.btnsaveusername.prop('disabled', false);
        let userNameList = GetInputUserName(textTagRemove);

        if (userNameList !== '') {
            let params = {};
            params.userNameList = userNameList;

            $.ajax(common.modifyRelativeUrl('AssociatedAccountMonitor/ValidateManualAssociationMultiple'), {
                type: 'POST',
                data: params,
                beforeSend: function () {
                    domElements.spanAddAssociationUserName.addClass("loading-value");
                },
                success: function (response) {
                    if (response.IsException)
                        BindExceptionValidateManualAccountMultiple();
                    else
                        BindResultValidatedManualAccountMultiple(response);
                },
                complete: function () {
                    domElements.spanAddAssociationUserName.removeClass("loading-value");
                },
                error: function (errMsg) {
                    console.log(errMsg);
                }
            });
        }
    }

    function GetInputUserName(textTagRemove) {
        let userNameList = [];
        let tags = domElements.spanAddAssociationUserName.find('tags tag');

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

    function BindExceptionValidateManualAccountMultiple() {
        spanAddAssociationUserName.find('tags tag').each(function (_, tag) {
            $(tag).removeClass('valid').addClass('invalid-username');
        });
    }

    function BindResultValidatedManualAccountMultiple(data) {
        domElements.spanAddAssociationUserName.find('tags tag').each(function (_, tag) {
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
        {
            validCTSCustID = data.ValideCTSCustId;
            domElements.btnsaveusername.prop('disabled', false);
        }
        else
            domElements.btnsaveusername.prop('disabled', true);
    }

    function InsertManualAssociationMultiple(fromUserID) {
        let userNameList = GetInputUserName();

        if (validCTSCustID !== '') {
            let params = {};
            params.fromCTSCustId = fromUserID;
            params.custIDList = validCTSCustID;
            params.toUserNameList = userNameList;
            params.remark = domElements.txtAsscomment.val().trim();

            $.ajax(common.modifyRelativeUrl('AssociatedAccountMonitor/InsertManualAssociatedAccount'), {
                type: 'POST',
                data: params,
                beforeSend: function () {
                    domElements.divAddUsername.addClass("loading-block");  //TODO
                },
                success: function (response) {
                    if (response.ErrorCode === 0) {
                        common.showSuccess("Add associated account successful");
                        domElements.divAddUsername.dialog('close');
                        setTimeout(() => {
                            LoadGrid();
                            RefreshTotalAssociatedAccount();   //TODO
                        }, 1000);
                    } else {
                        BindResultValidatedManualAccountMultiple(response.ResultValidated);
                    }
                },
                complete: function () {
                    domElements.divAddUsername.removeClass("loading-block");
                },
                error: function (errMsg) {
                    console.log(errMsg);
                }
            });
        }
    }

    function RefreshTotalAssociatedAccount() {
        $.ajax(common.modifyRelativeUrl('AssociatedAccountMonitor/GetAssociatedAccountMonitor'), {
            type: 'POST',
            data: {},
            success: function (response) {
                ////totalAssAccount = response.TotalAccounts;

                //$('#totalAssAccount').html(response.TotalAccounts);
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    } 

    function InitInputTagUsername() {
        domElements.spanAddAssociationUserNameMaxTag.text(domElements.maxTags);
        domElements.spanAddAssociationChangeHidden.text("0");

        function AddBorderTags() {
            domElements.spanAddAssociationUserName.find('tags').addClass('ctrlA');
            let allTags = domElements.spanAddAssociationUserName.find('tags tag.tagify__tag');
            allTags.each(function (_, tag) {
                $(tag).css('border', '1px solid #6187E7');
            });
        }

        function ResetMaxTag() {
            let currentTagsLength = domElements.spanAddAssociationUserName.find('tags tag').length;
            if (currentTagsLength < domElements.maxTags)
                domElements.spanAddAssociationUserNameMaxTag.text(domElements.maxTags - currentTagsLength + 1);
        }

        function EnableButtonAdd() {
            let validTags = domElements.spanAddAssociationUserName.find('tags tag.valid');
            let invalidDuplicatedTags = domElements.spanAddAssociationUserName.find('tags tag.invalid-duplicated');
            let invalidUsernameTags = domElements.spanAddAssociationUserName.find('tags tag.invalid-username');
            let notInvalid = invalidDuplicatedTags.length + invalidUsernameTags.length;
            if (notInvalid <= 1 && validTags.length > 1)
                domElements.btnsaveusername.prop('disabled', false);
            else
                domElements.btnsaveusername.prop('disabled', true);
        }

        function IsTextUnicode(text) {
            var pattern = /^[\x20-\x7E]*$/;
            return pattern.test(text);
        }

        function OnChangeTags(tagName, cssClass) {
            let maxCountTags = parseInt(domElements.spanAddAssociationUserNameMaxTag.text());
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
                    ValidateManualAssociationMultiple(currentTag.text().trim());   
                    EnableButtonAdd();
                });

                domElements.spanAddAssociationUserNameMaxTag.text(maxCountTags - 1);
            } else {
                inputTagUsername.tagify().data('tagify').removeTags(tagName.tag);
            }
            domElements.btnsaveusername.prop('disabled', true);
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
        })

        .off('add').on('add', function (_, tagName) {
            OnChangeTags(tagName, 'valid');
            domElements.spanAddAssociationChangeHidden.text("1");
        })
        .off('invalid').on('invalid', function (_, tagName) {
            OnChangeTags(tagName, 'invalid-duplicated');
            domElements.spanAddAssociationChangeHidden.text("1");
        });

        domElements.inputTagUsername.off('keydown').on('keydown', function () {
            let keyCode = event.keyCode;

            if (keyCode === 46) {
                if (domElements.spanAddAssociationUserName.find('tags').hasClass('ctrlA')) {
                    domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
                    domElements.spanAddAssociationUserNameMaxTag.text(domElements.maxTags);
                    domElements.spanAddAssociationUserName.find('tags').removeClass('ctrlA');
                }
            }

            if (event.ctrlKey) {
                if (keyCode === 65 || keyCode === 97) {
                    return AddBorderTags();
                }
            }
        });

        $(document).on('mouseup', function (e) {
            let isOutSiteInput = !domElements.spanAddAssociationUserName.is(e.target) && domElements.spanAddAssociationUserName.has(e.target).length === 0;
            let isTagsFocus = domElements.spanAddAssociationUserName.find('tags.tagify--focus').length > 0;
            let isChange = domElements.spanAddAssociationChangeHidden.text();
            if (isOutSiteInput && isTagsFocus || isChange === "1") {
                domElements.txtAsscomment.focus();
                domElements.spanAddAssociationUserName.addClass("loading-value");
                setTimeout(() => {
                    ValidateManualAssociationMultiple("");
                    domElements.spanAddAssociationUserName.removeClass("loading-value");
                    domElements.spanAddAssociationChangeHidden.text("0");
                }, 500);
            }
        });

        domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
        domElements.spanAddAssociationUserName.find('tags').attr('tabindex', "1");
        domElements.btnsaveusername.prop('disabled', true);
    }

    function GetCustProfile() {
        $.ajax(common.modifyRelativeUrl('AssociatedAccountMonitor/GetCustProfile'), {
            type: 'GET',
            data: {},
            success: function (response) {
                userID = response.CustID;
                IsHostPermission = response.IsHostPermission;
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    } 

    function InitMultiselectFilter() {
        domElements.cbbSubscriber.multiselect({
            menuWidth: 180,
            menuHeight: 300,
            selectedText: "# of # selected",
            noneSelectedText: "Selected Ass Group",
            click: function (event, ui) { RefreshGridByFilter("td span.subcriberGroup", "data-subcriber-group", ui.value, ui.checked); },
            checkAll: function () { RefreshAllGridByFilter("td span.subcriberGroup", "data-subcriber-group", true); },
            uncheckAll: function () { RefreshAllGridByFilter("td span.subcriberGroup", "data-subcriber-group", false); }
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
        });
    }

    function Initial() {
        InitMultiselectFilter();

        domElements.cbbSubscriber.multiselect({
            header: ['checkAll', 'uncheckAll'],
            menuWidth: 170,
            menuHeight: 300,
            selectedText: "# of # selected", 
            noneSelectedText: "4 of 4 selected"
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
        });

        LoadGrid(); 
    }

    $(document).ready(function () {
        Initial();

        GetCustProfile();

        domElements.btnAddUsername.on('click', function () {
            domElements.txtAsscomment.val('');

            InitInputTagUsername();

            domElements.divAddUsername.dialog({
                modal: true,
                width: '800px',
                title: 'ADD ASSOCIATED ACCOUNT ',
                resizable: true,
                draggable: true,
                close: function () {
                    $(this).dialog('close');
                }
            });

            // handle freeze header
            $(window).scroll(function (event) {
                var scroll = $(window).scrollTop();
                if (scroll >= 96) {
                    let height_filter = $(".form-filter").height() + 16;
                    console.log(height_filter);

                    $(".asociatedAccountMonitorTable  thead tr:first-child th").css({ "top": height_filter + 108 })
                    $(".wrapper-info-table").css({ "top": height_filter + 96 })
                }
                console.log(scroll);
            });
        });

        domElements.btnsaveusername.on('click', () => InsertManualAssociationMultiple(userID));

        domElements.btncancelusername.on('click', () => {
            domElements.divAddUsername.dialog('close');
        });
    });
}

$(AssociatedAccountMonitor);