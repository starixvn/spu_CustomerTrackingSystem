function SpecialCustomerClass() {
    let userList;

    let domElements = {
        ccbSportTypeSingle: $('#ccbSportTypeSingle'),
        cbbSportTypeMulti: $('#ccbSportType'),
        specialCustomerClassTable: $('#tblSpecialCustomerClass'),
        divCustomerClass: $('#divAddCustomerClass'),
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
        inputSelectAll: $('#inputSelectAll'),
        btnAddCustomerClass: $('#btnAdd'),
        btnRemoveCustomerClass: $('#btnRemove'),
    };

    let isHostPermission = domElements.hdIsHostPermission.attr('IsHostPermission') === "True";

    function InitGrid() {
        domElements.specialCustomerClassTable.DataTable({
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
                { targets: [0], className: "text-center" },
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
                        return `<label><input type="checkbox" name="chkbx" class="col-checkbox"/></label>`;
                    }
                },
                { data: obj => obj.Username },
                { data: obj => obj.SubscriberName },
                { data: obj => obj.SportType },
                { data: obj => obj.CustomerClass },
                { data: obj => obj.CreatedBy },
                { data: obj => obj.Comment },
                { data: obj => obj.CreatedDateString },
                {
                    data: obj => () => {
                        return html = '<span class="sub-icon - delete" id="sub-icon " title="Remove Account"><i class="icon-delete ' + (!isHostPermission ? ' disabled"' : '') + '"></i></span>';

                    }, createdCell: (cell, _, row) => {
                        if (isHostPermission) {
                            $('.icon-delete', cell).click(() => {
                                $('td #sub-icon').addClass('none-pointer');
                                let html = "<div class='popup - content'>Do you want remove <strong>" + row.Username + "</strong> from Special Customer Class list? </div>";

                                userList = [{ Username: row.Username, SportID: 0 }];
                                domElements.txtComment.val("");

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
                            });
                        }
                    }
                }
            ],
        });

        domElements.btnAddCustomerClass = $('#btnAdd');
        domElements.btnAddCustomerClass.prop('disabled', !isHostPermission);
        domElements.btnRemoveCustomerClass = $('#btnRemove');
        domElements.btnAddCustomerClass.prop('disabled', !isHostPermission);

        if (isHostPermission) {
            domElements.btnAddCustomerClass.on('click', function () {
                domElements.inputTagUsername.val("");
                domElements.txtCustomerClass.val("");
                domElements.txtComment.val("");
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
                userList = [];
                domElements.specialCustomerClassTable.DataTable().rows(function (idx, x, node) {
                    if ($(node).find('input[type="checkbox"][name="chkbx"]').prop('checked')) {
                        userList.push({ Username: x.Username, SportID: 0 });
                    }
                });

                if (userList.length <= 0) {
                    common.showError("Please select at least 1 record to proceed!");
                    return;
                }

                domElements.txtComment.val("");

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

    function RemoveCustomerClass(userList) {
        var url = common.modifyRelativeUrl('/SpecialCustomerClass/RemoveCustomerClass');
        var params = {};
        params.Comment = domElements.txtComment.val();
        params.UserList = userList;

        $.ajax(url, {
            type: 'POST',
            tryCount: 0,
            retryLimit: 3,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(params),
            success: function (response) {
                LoadGrid(true);
                common.showSuccess('Remove the Customer Class successful');
            },
            beforeSend: function () {
                common.showLoading();
            },
            complete: function () {
                common.closeLoading();
            },
            error: function (xhr, textStatus, error) {
                if (textStatus == 'timeout') {
                    this.tryCount++;
                    if (this.tryCount <= this.retryLimit) {
                        //try again
                        $.ajax(this);
                        return;
                    }
                }

                console.log(xhr);
                if (xhr.status === 400 && xhr.responseJSON?.Users) {
                    $.toast({
                        text: `<div class='toast-content'>${ShowInValidUserNameString(xhr.responseJSON)}</div>`, // Text that is to be shown in the toast
                        heading: 'Invalid Accounts', // Optional heading to be shown on the toast
                        icon: 'error', // Type of toast icon
                        showHideTransition: 'fade', // fade, slide or plain
                        allowToastClose: true, // Boolean value true or false
                        hideAfter: false, // false to make it sticky or number representing the miliseconds as time after which toast needs to be hidden
                        stack: 5, // false if there should be only one toast at a time or a number representing the maximum number of toasts to be shown at a time
                        position: 'top-center', // bottom-left or bottom-right or bottom-center or top-left or top-right or top-center or mid-center or an object representing the left, right, top, bottom values
                        textAlign: 'left',  // Text alignment i.e. left, right or center
                        loader: true,  // Whether to show loader or not. True by default
                        loaderBg: '#fff',  // Background color of the toast loader
                    });
                }

                if (xhr.status === 500)
                {
                    common.showError("Please contact admin!");
                }

                common.closeLoading();
            }
        });
    }

    function ValidateInputs() {
        var result = true;

        if (domElements.inputTagUsername.val().trim().length == 0) {
            common.showError('Username ' + domElements.inputTagUsername.val() + ' is invalid.');
            result = false;
        }

        let inputClass = domElements.txtCustomerClass.val();

        if (!inputClass || !parseInt(inputClass)) {
            common.showError('Class ' + domElements.txtCustomerClass.val() + ' is invalid.');
            result = false;
        }

        return result;
    }

    function AddCustomerClass() {
        var url = common.modifyRelativeUrl('/SpecialCustomerClass/AddCustomerClass');
        var params = {};
        params.UserList = JSON.parse(domElements.inputTagUsername.val()).map(x => { return { Username: x.value, SportID: domElements.ccbSportTypeSingle.val() } });
        params.Comment = domElements.txtComment.val();
        params.CustomerClass = domElements.txtCustomerClass.val();

        $.ajax(url, {
            type: 'POST',
            tryCount: 0,
            retryLimit: 3,
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(params),
            success: function (response) {
                common.showSuccess('Add the Customer Class successful');
                LoadGrid(false);
            },
            beforeSend: function () {
                common.showLoading();
            },
            complete: function () {
                common.closeLoading();
            },
            error: function (xhr, textStatus, error) {
                if (textStatus == 'timeout') {
                    this.tryCount++;
                    if (this.tryCount <= this.retryLimit) {
                        //try again
                        $.ajax(this);
                        return;
                    }
                }

                console.log(xhr);
                if (xhr.status === 400 && xhr.responseJSON?.Users) {
                    $.toast({
                        text: `<div class='toast-content'>${ShowInValidUserNameString(xhr.responseJSON)}</div>`, // Text that is to be shown in the toast
                        heading: 'Invalid Accounts', // Optional heading to be shown on the toast
                        icon: 'error', // Type of toast icon
                        showHideTransition: 'fade', // fade, slide or plain
                        allowToastClose: true, // Boolean value true or false
                        hideAfter: false, // false to make it sticky or number representing the miliseconds as time after which toast needs to be hidden
                        stack: 5, // false if there should be only one toast at a time or a number representing the maximum number of toasts to be shown at a time
                        position: 'top-center', // bottom-left or bottom-right or bottom-center or top-left or top-right or top-center or mid-center or an object representing the left, right, top, bottom values
                        textAlign: 'left',  // Text alignment i.e. left, right or center
                        loader: true,  // Whether to show loader or not. True by default
                        loaderBg: '#fff',  // Background color of the toast loader
                    });
                }
                
                if (xhr.status === 500)
                {
                    common.showError("Please contact admin!");
                }

                common.closeLoading();
            }
        });
    }

    function ShowInValidUserNameString(responseJson) {
        var users = responseJson.Users;
        var result = '';

        for (var element of domElements.cbbSportTypeMulti[0].options) {
            var arr = users.filter(x => x.SportID == element.value && !x.IsValid).map(x => x.Username);
            if (arr.length > 0) {
                result += `${element.label}<br>`;
                result += `&emsp;${arr.join(', ')}<br>`;
            }
        }
        return result;
    }

    function LoadGrid(isSkip) {
        domElements.inputSelectAll.prop('checked', false);
        var url = common.modifyRelativeUrl('/SpecialCustomerClass/GetCustomerClass');

        var params = {
            isSkipTracking: isSkip,
            Username: domElements.txtSearchUsername.val(),
            SportIds: domElements.cbbSportTypeMulti.val().join()
        };

        $.ajax(url, {
            type: 'GET',
            tryCount: 0,
            retryLimit: 3,
            // dataType: 'json',
            // contentType: 'application/json; charset=utf-8',
            data: params,
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


        // $.ajax(url, {
        //     type: 'GET',
        //     success: function (response) {
        //         domElements.specialCustomerClassTable.DataTable().clear().rows.add(response.data).draw();
        //     },
        //     beforeSend: function () {
        //         domElements.btnGenerate.prop('disabled', true);
        //         domElements.specialCustomerClassTable.find('tbody').addClass('loading');
        //     },
        //     complete: function () {
        //         domElements.btnGenerate.prop('disabled', false);
        //         domElements.specialCustomerClassTable.find('tbody').removeClass('loading');
        //     },
        //     error: function (err) {
        //         console.log(err);
        //         domElements.specialCustomerClassTable.find('tbody').removeClass('loading');
        //     }
        // });
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

        domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
    }

    function ToggleGridSelectAll(input, table) {
        var nodes = table.rows({ 'search': 'applied' }).nodes();

        $('input[type="checkbox"][name="chkbx"]', nodes).prop('checked', input.is(":checked"));
    }

    function Initial() {
        SetupMultiselect(domElements.cbbSportTypeMulti, "Sport Type")
        InitGrid();

        domElements.inputSelectAll.on('click', function () {
            ToggleGridSelectAll(domElements.inputSelectAll, domElements.specialCustomerClassTable.DataTable());
        });

        if (isHostPermission) {
            LoadGrid(false);

            domElements.btnGenerate.on('click', function () {
                LoadGrid(false);
            });

            domElements.txtSearchUsername.keypress(function (event) {
                if (event.keyCode === 13) {
                    LoadGrid(false);
                }
            });

            domElements.btnSave.on('click', function () {
                if (domElements.formValidate.css('display') != 'none') {
                    if (ValidateInputs()) {
                        AddCustomerClass();
                    }
                }
                else {
                    if (userList.length > 0) {
                        RemoveCustomerClass(userList);
                    }
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
