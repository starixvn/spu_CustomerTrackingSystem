function SpecialCustomerClass() {
    let dataTable;

    let domElements = {
        cbbSporType: $('#ccbSportType'),
        specialCustomerClassTable: $('#tblSpecialCustomerClass'),
        divCustomerClass: $('#divAddCustomerClass'),
        btnAddCustomerClass: $('#btnAdd'),
        btnGenerate: $('#btnGenerate'),
        btnSave: $('#btnSave'),
        btnCancel: $('#btnCancel'),
        btnIsAffectToAssociation: $('#isAffectedToAssociation'),
        inputTagUsername: $('#inputTagUsername'),
        txtCustomerClass: $('#txtCustomerClass'),
        txtSearchUsername: $('#txtSearchUsername'),
        hdIsHostPermission: $('#hdIsHostPermission'),
        spanAddSpecialCustomerAccountUserName: $('#spanAddProblemAccountUserName'),
        spanAddSpecialCustomerAccountUserNameMaxTag: $('#spanAddProblemAccountUserNameMaxTag'),
        spanAddSpecialCustomerAccountChangeHidden: $('#spanAddProblemAccountChangeHidden'),
        spanSpecialCustomerAccountActionHidden: $('#spanProblemAccountActionHidden'),
        maxTags: 1001,
        divAddProblemAccount: $('#divAddProblemAccount'),
        txtComment: $('#txtComment'),
        formValidate: $('.form-validate'),
        btnRemoveCustomerClass: $('#btnRemove')
    };

    let isHostPermission = domElements.hdIsHostPermission.attr('IsHostPermission') === "True";

    function InitGrid() {
        return domElements.specialCustomerClassTable.DataTable({
            serverSide: false,
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            pagingType: "full_numbers",
            language: {
                lengthMenu: "Page size _MENU_",
                paginate: {
                    first: "<span class=\"icon-chevron-left\"></span><span class=\"icon-chevron-left\"></span>",
                    last: "<span class=\"icon-chevron-right\"></span><span class=\"icon-chevron-right\"></span>",
                    next: "",
                    previous: ""
                },
                // info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
                // infoEmpty: "",
                zeroRecords: "There's no data"
            },
            infoCallback: function (settings, start, end, max, total, pre) {
                if (!total) {
                    $('.wrapper-info').append(`
                        <div class=\"tblSpecialCustomerClass_buttons\">
                            <button class=\"button button-primary-outline\" id=\"btnAdd\">Add</button>
                            <button class=\"button button-primary-outline\" id=\"btnRemove\">Remove</button>
                        </div>
                    `);
                    return "<div class=\"display-total-ticket\"></div>";
                }
                else {
                    $('.display-total-ticket').html(`Displaying ${start} to ${end} of ${total} item(s)`);
                }
            },
            ordering: false,
            searching: false,
            lengthMenu: [20, 50, 100, 200],
            pageLength: 50,
            scrollY: `${$(window).height() - 250}px`,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                {
                    targets: [0], className: "text-center", render: function (data, type, full, meta) {
                        return '<label><input type="checkbox" class="col-checkbox" name="chkbx" /></label>';
                    }
                },
                { targets: [0], width: "1%" },
                { targets: [1, 2, 5], className: "text-left col-username" },
                { targets: [3, 4], className: "text-center col-username" },
                { targets: [6], className: "text-left", width: "30%" },
                { targets: [7], className: "text-center col-date" },
                { targets: [8], className: "text-center col-action" },
            ],
            columns: [
                {
                    data: obj => () => {
                        return `<label><input type="checkbox" name="chkbx" class="col-checkbox" /></label>`;
                    }
                },
                { data: obj => obj.Username },
                { data: obj => obj.SubscriberName },
                { data: obj => obj.SubscriberName },
                { data: obj => obj.CustomerClass },
                { data: obj => obj.CreatedBy },
                { data: obj => obj.CreatedDateString },
                { data: obj => obj.CreatedDateString },
                {
                    data: obj => () => {
                        return html = '<span class="sub-icon - delete" id="sub-icon " title="Remove Account"><i class="icon-delete ' + (!isHostPermission ? ' disabled"' : '') + '"></i></span>';

                    }, createdCell: (cell, _, row) => {
                        if (isHostPermission) {
                            $('.icon-delete', cell).click(() => {
                                $('td #sub-icon').addClass('none-pointer');
                                let html = "<div class='popup - content'>Do you want remove <strong>" + row.Username + "</strong> from Special Customer Class list? </div>";

                                common.showConfirmation(html, function () {
                                    RemoveCustomerClass(row.CustID, row.CTSCustID);
                                }, () => {
                                    $('td #sub-icon').removeClass('none-pointer');
                                });
                            });
                        }
                    }
                }
            ],
            fnDrawCallback: function () {
                domElements.btnAddCustomerClass = $('#btnAdd');
                domElements.btnAddCustomerClass.prop('disabled', !isHostPermission);
                domElements.btnRemoveCustomerClass = $('#btnRemove');
                domElements.btnAddCustomerClass.prop('disabled', !isHostPermission);

                if (isHostPermission) {
                    domElements.btnAddCustomerClass.on('click', function () {
                        domElements.inputTagUsername.val("");
                        domElements.txtCustomerClass.val("");
                        domElements.btnIsAffectToAssociation.prop('checked', false);

                        InitInputTagUsername();
                        domElements.divAddProblemAccount.dialog({
                            modal: true,
                            width: '500px',
                            title: 'ADD CUSTOMER CLASS',
                            resizable: true,
                            draggable: true,
                            open: function () {
                                domElements.formValidate.css('display', 'block');
                            },
                            close: function () {
                                $(this).dialog('close');
                            }
                        });
                    });

                    domElements.btnRemoveCustomerClass.on('click', function () {
                        domElements.divAddProblemAccount.dialog({
                            modal: true,
                            width: '500px',
                            title: 'REMOVE CUSTOMER CLASS',
                            resizable: true,
                            draggable: true,
                            open: function () {
                                domElements.formValidate.css('display', 'none');
                            },
                            close: function () {
                                $(this).dialog('close');
                            }
                        });
                    })
                }
            }
        });
    }

    function RemoveCustomerClass(custID, ctsCustID) {
        var url = common.modifyRelativeUrl('/SpecialCustomerClass/RemoveCustomerClass');
        var params = {};
        params.custID = custID;
        params.ctsCustID = ctsCustID;

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.ErrorCode === 0) {
                    common.showSuccess('Remove the Customer Class successful');
                    LoadGrid(domElements.txtSearchUsername.val(), true);
                }
                else {
                    common.showError('Failed! ' + response.ErrorMsg);
                }
            },
            beforeSend: function () {
                common.showLoading();
            },
            complete: function () {
                common.closeLoading();
            },
            error: function (err) {
                console.log(err);
                common.closeLoading();
            }
        });
    }

    function ValidateInputs() {
        if (domElements.inputTagUsername.val() === "") {
            common.showError('Username "' + domElements.inputTagUsername.val() + '" is invalid.');
            return false;
        }

        let inputClass = domElements.txtCustomerClass.val();

        if (inputClass === "") {
            common.showError('Class "' + domElements.txtCustomerClass.val() + '" is invalid.');
            return false;
        }

        let parseInputClass = parseInt(inputClass);

        if (parseInputClass === undefined) {
            common.showError('Class "' + domElements.txtCustomerClass.val() + '" is number.');

            return false;
        }

        return true;
    }

    function AddCustomerClass() {
        console.log(domElements.inputTagUsername.val());
        if (false && ValidateInputs()) {
            var url = common.modifyRelativeUrl('/SpecialCustomerClass/AddCustomerClass');
            var params = {};
            params.UserName = domElements.inputTagUsername.val();
            params.custClass = domElements.txtCustomerClass.val();
            params.isAffectToAss = domElements.btnIsAffectToAssociation.is(':checked');

            $.ajax(url, {
                type: 'POST',
                data: params,
                success: function (response) {
                    if (response !== undefined) {
                        if (response.ErrorCode === 0) {
                            common.showSuccess("Add the Customer Class successful");
                            domElements.divCustomerClass.dialog('close');
                        } else {
                            common.showError("Failed! " + response.ErrorMessage);
                        }
                    }
                },
                beforeSend: function () {
                    common.showLoading();
                },
                complete: function () {
                    common.closeLoading();
                },
                error: function (err) {
                    console.log(err);
                    common.closeLoading();
                }
            });
        }
    }

    function LoadGrid(username, isSkip) {
        var url = common.modifyRelativeUrl('/SpecialCustomerClass/GetCustomerClass?userName=' + username + '&isSkipTracking=' + isSkip);

        $.ajax(url, {
            type: 'GET',
            success: function (response) {
                domElements.specialCustomerClassTable.DataTable().clear().rows.add(response.data).draw();
            },
            beforeSend: function () {
                domElements.btnGenerate.prop('disabled', true);
                domElements.specialCustomerClassTable.find('tbody').addClass('loading');
            },
            complete: function () {
                domElements.btnGenerate.prop('disabled', false);
                domElements.specialCustomerClassTable.find('tbody').removeClass('loading');
            },
            error: function (err) {
                console.log(err);
                domElements.specialCustomerClassTable.find('tbody').removeClass('loading');
            }
        });
    }

    function SetupMultiselect(ccb, selectName, menuWidth = 260, menuHeight = 280) {
        ccb.multiselect({
            menuWidth: menuWidth,
            menuHeight: menuHeight,
            selectedText: "# of # select",
            noneSelectedText: `Select ${selectName}`
        });
    }

    function InitInputTagUsername() {
        domElements.spanAddSpecialCustomerAccountUserNameMaxTag.text(domElements.maxTags);
        domElements.spanAddSpecialCustomerAccountChangeHidden.text("0");

        function AddBorderTags() {
            domElements.spanAddSpecialCustomerAccountUserName.find('tags').addClass('ctrlA');
            let allTags = domElements.spanAddSpecialCustomerAccountUserName.find('tags tag.tagify__tag');
            allTags.each(function (_, tag) {
                $(tag).css('border', '1px solid #6187E7');
            });
        }

        function ResetMaxTag() {
            let currentTagsLength = domElements.spanAddSpecialCustomerAccountUserName.find('tags tag').length;
            if (currentTagsLength < domElements.maxTags)
                domElements.spanAddSpecialCustomerAccountUserNameMaxTag.text(domElements.maxTags - currentTagsLength + 1);
        }

        function EnableButtonAdd() {
            let validTags = domElements.spanAddSpecialCustomerAccountUserName.find('tags tag.valid');
            let invalidDuplicatedTags = domElements.spanAddSpecialCustomerAccountUserName.find('tags tag.invalid-duplicated');
            let invalidUsernameTags = domElements.spanAddSpecialCustomerAccountUserName.find('tags tag.invalid-username');
            let numOfTagExceeded = domElements.spanAddSpecialCustomerAccountUserName.find('tags tag.tagify--notAllowed');

            let notInvalid = invalidDuplicatedTags.length + invalidUsernameTags.length + numOfTagExceeded.length;
            if (notInvalid <= 1 && validTags.length > 1)
                domElements.btnSave.prop('disabled', false);
            else
                domElements.btnSave.prop('disabled', true);
        }

        function IsTextUnicode(text) {
            var pattern = /^[\x20-\x7E]*$/;
            return pattern.test(text);
        }

        function OnChangeTags(tagName, cssClass) {
            let maxCountTags = parseInt(domElements.spanAddSpecialCustomerAccountUserNameMaxTag.text());
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
                    // ValidateProblemAccountInput(currentTag.text().trim());
                    // EnableButtonAdd();
                });

                domElements.spanAddSpecialCustomerAccountUserNameMaxTag.text(maxCountTags - 1);
            } else {
                domElements.inputTagUsername.tagify().data('tagify').removeTags(tagName.tag);
                let $tag = $(tagName.tag);
                let btnRemove = $tag.find('x');
                btnRemove.on('click', function (e) {
                    let currentTag = $(e.target).parent();

                    ResetMaxTag();
                    // ValidateProblemAccountInput(currentTag.text().trim());
                    // EnableButtonAdd();
                });
            }
            // domElements.btnSave.prop('disabled', true);
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
                domElements.spanAddSpecialCustomerAccountChangeHidden.text("1");
            })
            .off('invalid').on('invalid', function (_, tagName) {
                OnChangeTags(tagName, 'invalid-duplicated');
                domElements.spanAddSpecialCustomerAccountChangeHidden.text("1");
            });

        domElements.inputTagUsername.off('keydown').on('keydown', function () {
            let keyCode = event.keyCode;

            if (keyCode === 46) {
                if (domElements.spanAddSpecialCustomerAccountUserName.find('tags').hasClass('ctrlA')) {
                    domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
                    domElements.spanAddSpecialCustomerAccountUserNameMaxTag.text(domElements.maxTags);
                    domElements.spanAddSpecialCustomerAccountUserName.find('tags').removeClass('ctrlA');
                }
            }

            if (event.ctrlKey) {
                if (keyCode === 65 || keyCode === 97) {
                    return AddBorderTags();
                }
            }
        });

        $(document).off('mouseup').on('mouseup', function (e) {
            let isOutSiteInput = !domElements.spanAddSpecialCustomerAccountUserName.is(e.target) && domElements.spanAddSpecialCustomerAccountUserName.has(e.target).length === 0;
            let isTagsFocus = domElements.spanAddSpecialCustomerAccountUserName.find('tags.tagify--focus').length > 0;
            let isChange = domElements.spanAddSpecialCustomerAccountChangeHidden.text();
            if (isOutSiteInput && isTagsFocus || isChange === "1") {
                domElements.txtComment.focus();
                domElements.spanAddSpecialCustomerAccountUserName.addClass("loading-value");
                setTimeout(() => {
                    // ValidateProblemAccountInput('');
                    domElements.spanAddSpecialCustomerAccountUserName.removeClass("loading-value");
                    domElements.spanAddSpecialCustomerAccountChangeHidden.text("0");
                }, 500);
            }
        });

        domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
        // domElements.spanAddProblemAccountUserName.find('tags').attr('tabindex', "1");
    }

    function GetinputTagUsername(textTagRemove) {
        let userNameList = [];
        let tags = domElements.spanAddSpecialCustomerAccountUserName.find('tags tag');

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

    function Initial() {
        SetupMultiselect(domElements.cbbSporType, "Sport Type")
        InitGrid();

        if (isHostPermission) {
            domElements.btnGenerate.on('click', function () {
                LoadGrid(domElements.txtSearchUsername.val(), false);
            });

            domElements.txtSearchUsername.keypress(function (event) {
                if (event.keyCode === 13) {
                    LoadGrid(domElements.txtSearchUsername.val(), false);
                }
            });

            domElements.btnSave.on('click', function () {
                if (domElements.formValidate.css('display') != 'none') {
                    common.showSuccess('Add the Customer Class successful');
                    AddCustomerClass();
                }
                else {
                    common.showSuccess('Remove the Customer Class successful');
                }
            });

            domElements.btnCancel.on('click', () => {
                domElements.divAddProblemAccount.dialog('close');
            });
        }
    }

    $(document).ready(function () {
        Initial();
    });

    $(window).resize(function () {
        $('.dataTables_scrollBody').css('max-height', ($(window).height() - 250));
    });
}

$(SpecialCustomerClass);
