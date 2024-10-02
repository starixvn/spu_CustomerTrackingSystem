function ProblemAccountManagement() {
    let dataTable;
    let categoryGroup = {
        list: [],
        isIncludeMember: false,
        isIncludeAgency: false
    }

    let domElements = {
        hdIsHostPermission: $('#hdIsHostPermission'),
        dpkFromDate: $('#dpkFromDate'),
        dpkToDate: $('#dpkToDate'),
        cbbSite: $('#cbbSite'),
        cbbCategory: $('#cbbCategory'),
        cbbRole: $('#cbbRole'),
        cbbProblemAccountCategories: $('#cbbProblemAccountCategories'),
        inputUserName: $('#inputUserName'),
        inputSearchDownlines: $('#inputSearchDownlines'),
        btnGenerate: $('#btnGenerate'),
        btnExportToExcel: $('#btnExportToExcel'),
        tblProblemAccount: $('#tblProblemAccount'),
        btnMarkNewProblemAccount: $('#btnMarkNewProblemAccount'),
        btnUnmarkProblemAccount: $('#btnUnmarkProblemAccount'),
        divAddProblemAccount: $('#divAddProblemAccount'),
        grpCategories: $('#grpCategories'),
        grpCkboxAffectedDownline: $('#grpCkboxAffectedDownline'),
        inputCCAffectToAllDownlines: $('#inputCCAffectToAllDownlines'),
        ckbUnmarkDownlines: $('#ckbUnmarkDownlines'),
        txtComment: $('#txtComment'),
        btnCancel: $('#btnCancel'),
        btnSave: $('#btnSave'),
        spanAddProblemAccountUserName: $('#spanAddProblemAccountUserName'),
        spanAddProblemAccountUserNameMaxTag: $('#spanAddProblemAccountUserNameMaxTag'),
        spanAddProblemAccountChangeHidden: $('#spanAddProblemAccountChangeHidden'),
        spanProblemAccountActionHidden: $('#spanProblemAccountActionHidden'),
        inputTagUsername: $('#inputTagUsername'),
        maxTags: 1001,
        hdUserNames: $('#hdUserNames'),
    };

    function RenderGrid() {
        let url = common.modifyRelativeUrl('ProblemAccountManagement/GetProblemAccount');

        return domElements.tblProblemAccount.DataTable({
            serverSide: false,
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            pagingType: "full_numbers",
            ajax: {
                url: url,
                data: function () {
                    return GetParams(false);
                },
                type: "POST",
                beforeSend: function () {
                    domElements.tblProblemAccount.find('tbody').addClass('loading');
                },
                complete: function () {
                    domElements.tblProblemAccount.find('tbody').removeClass('loading');
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
                { targets: [0, 1, 2, 3, 4, 5, 6], className: "text-center" },
                { targets: [0], width: "20%" },
                { targets: [1], width: "15%" },
                { targets: [2], width: "10%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "25%" },
                { targets: [5], width: "10%" },
                { targets: [6], width: "10%" }
            ],
            columns: [
                { data: obj => obj.UserName },
                { data: obj => obj.Site },
                { data: obj => obj.Currency },
                { data: obj => obj.Category },
                { data: obj => obj.Comment },
                { data: obj => obj.CreatedDateStr },
                { data: obj => obj.CreatedBy }
            ]
        });
    }

    function GetParams(isExportExcel) {
        let params = {};

        params.FromDate = domElements.dpkFromDate.val();
        params.ToDate = domElements.dpkToDate.val();
        params.Sites = domElements.cbbSite.val().join();
        params.IsAllSites = domElements.cbbSite.find('option').not(':selected').length == 0;
        params.CategoryIDs = domElements.cbbCategory.val().join();
        params.UserName = domElements.inputUserName.val().trim();
        params.RoleIDs = domElements.cbbRole.val().join();
        params.HasDownline = domElements.inputSearchDownlines.is(":checked");

        if (isExportExcel) {
            params.IsAllCategories = domElements.cbbCategory.find('option').not(':selected').length == 0;
            params.CategoryNames = $('#cbbCategory option:selected').toArray().map(item => item.text).join();

            domElements.cbbSite.find('optgroup').each(function () {
                let txtSelected = $(this).find('option:selected').toArray().map(item => item.text).join();
                if ($(this).attr('label') == 'Credit') {
                    params.CreditSiteNames = txtSelected;
                } else if ($(this).attr('label') == 'Licensees') {
                    params.LicenseeSiteNames = txtSelected;
                } else {
                    params.CN88SiteNames = txtSelected;
                }
            });
        }

        return params;
    }

    function GetProblemAccountReport() {
        if (ValidateRangeDate()) {
            dataTable
                ? dataTable.ajax.reload(null, true)
                : dataTable = RenderGrid();
        }
    }

    function ValidateRangeDate() {
        let toDateValue = domElements.dpkToDate.datepicker("getDate");
        let toDateValueAllow = domElements.dpkFromDate.datepicker("getDate");
        toDateValueAllow.setDate(toDateValueAllow.getDate() + 30);

        let userName = domElements.inputUserName.val().trim();

        if (toDateValue > toDateValueAllow && userName == '') {
            common.showError('Due to size limitation, please press "Export to Excel" for more detail information');

            return false;
        }

        return true;
    }

    function ValidateRangeDateExport() {
        let toDateValue = domElements.dpkToDate.datepicker("getDate");
        let toDateValueAllow = domElements.dpkFromDate.datepicker("getDate");
        toDateValueAllow.setDate(toDateValueAllow.getDate() + 365);

        if (toDateValue > toDateValueAllow) {
            common.showError("Invalid date range, please select not more than 365 days");

            return false;
        }

        return true;
    }

    function SetRestrictFromDate() {
        let toDateValue = domElements.dpkToDate.datepicker("getDate");

        domElements.dpkFromDate.datepicker("option", "maxDate", toDateValue);
    }

    function SetRestrictToDate() {
        let fromDateValue = domElements.dpkFromDate.datepicker("getDate");

        domElements.dpkToDate
            .datepicker("option", "minDate", fromDateValue)
            .datepicker("option", "maxDate", moment(fromDateValue).add(1, "year").toDate());
    }

    function InitInputTagUsername() {
        domElements.spanAddProblemAccountUserNameMaxTag.text(domElements.maxTags);
        domElements.spanAddProblemAccountChangeHidden.text("0");

        function AddBorderTags() {
            domElements.spanAddProblemAccountUserName.find('tags').addClass('ctrlA');
            let allTags = domElements.spanAddProblemAccountUserName.find('tags tag.tagify__tag');
            allTags.each(function (_, tag) {
                $(tag).css('border', '1px solid #6187E7');
            });
        }

        function ResetMaxTag() {
            let currentTagsLength = domElements.spanAddProblemAccountUserName.find('tags tag').length;
            if (currentTagsLength < domElements.maxTags)
                domElements.spanAddProblemAccountUserNameMaxTag.text(domElements.maxTags - currentTagsLength + 1);
        }

        function EnableButtonAdd() {
            let validTags = domElements.spanAddProblemAccountUserName.find('tags tag.valid');
            let invalidDuplicatedTags = domElements.spanAddProblemAccountUserName.find('tags tag.invalid-duplicated');
            let invalidUsernameTags = domElements.spanAddProblemAccountUserName.find('tags tag.invalid-username');
            let numOfTagExceeded = domElements.spanAddProblemAccountUserName.find('tags tag.tagify--notAllowed');

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
            let maxCountTags = parseInt(domElements.spanAddProblemAccountUserNameMaxTag.text());
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
                    ValidateProblemAccountInput(currentTag.text().trim());
                    EnableButtonAdd();
                });

                domElements.spanAddProblemAccountUserNameMaxTag.text(maxCountTags - 1);
            } else {
                //domElements.inputTagUsername.tagify().data('tagify').removeTags(tagName.tag); 
                let $tag = $(tagName.tag);
                let btnRemove = $tag.find('x');
                btnRemove.on('click', function (e) {
                    let currentTag = $(e.target).parent();

                    ResetMaxTag();
                    ValidateProblemAccountInput(currentTag.text().trim());
                    EnableButtonAdd();
                });
            }
            domElements.btnSave.prop('disabled', true);
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
                domElements.spanAddProblemAccountChangeHidden.text("1");
            })
            .off('invalid').on('invalid', function (_, tagName) {
                OnChangeTags(tagName, 'invalid-duplicated');
                domElements.spanAddProblemAccountChangeHidden.text("1");
            });

        domElements.inputTagUsername.off('keydown').on('keydown', function () {
            let keyCode = event.keyCode;

            if (keyCode === 46) {
                if (domElements.spanAddProblemAccountUserName.find('tags').hasClass('ctrlA')) {
                    domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
                    domElements.spanAddProblemAccountUserNameMaxTag.text(domElements.maxTags);
                    domElements.spanAddProblemAccountUserName.find('tags').removeClass('ctrlA');
                }
            }

            if (event.ctrlKey) {
                if (keyCode === 65 || keyCode === 97) {
                    return AddBorderTags();
                }
            }
        });

        $(document).off('mouseup').on('mouseup', function (e) {
            let isOutSiteInput = !domElements.spanAddProblemAccountUserName.is(e.target) && domElements.spanAddProblemAccountUserName.has(e.target).length === 0;
            let isTagsFocus = domElements.spanAddProblemAccountUserName.find('tags.tagify--focus').length > 0;
            let isChange = domElements.spanAddProblemAccountChangeHidden.text();
            if (isOutSiteInput && isTagsFocus || isChange === "1") {
                domElements.txtComment.focus();
                domElements.spanAddProblemAccountUserName.addClass("loading-value");
                setTimeout(() => {
                    ValidateProblemAccountInput('');
                    domElements.spanAddProblemAccountUserName.removeClass("loading-value");
                    domElements.spanAddProblemAccountChangeHidden.text("0");
                }, 500);
            }
        });

        domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
        domElements.spanAddProblemAccountUserName.find('tags').attr('tabindex', "1");
        domElements.btnSave.prop('disabled', true);
    }

    function ValidateProblemAccountInput(textTagRemove) {
        let numOfTagExceeded = domElements.spanAddProblemAccountUserName.find('tags tag.tagify--notAllowed');

        if (numOfTagExceeded.length > 0) {
            return;
        }

        domElements.btnSave.prop('disabled', false);
        let userNames = GetInputUserName(textTagRemove);

        if (userNames !== '') {
            let params = {};
            params.UserNames = userNames;
            params.CategoryID = domElements.cbbProblemAccountCategories.val();
            params.CategoryGroup = $('option:selected', domElements.cbbProblemAccountCategories).attr('data-categorygroup');
            params.PAAction = domElements.spanProblemAccountActionHidden.val();

            $.ajax(common.modifyRelativeUrl('ProblemAccountManagement/ValidateProblemAccountInput'), {
                type: 'POST',
                data: params,
                beforeSend: function () {
                    domElements.spanAddProblemAccountUserName.addClass("loading-value");
                },
                success: function (response) {
                    if (response.IsException)
                        BindExceptionValidateInputUserName();
                    else
                        BindResultValidatedInputUserName(response);

                    if (params.PAAction == 2) { // Unmark
                        domElements.txtComment.val(response.Comment);
                        if (response.IsExistedSMA && response.IsCreditSite) {
                            domElements.grpCkboxAffectedDownline.removeClass('hide');
                        } else {
                            domElements.grpCkboxAffectedDownline.addClass('hide');
                        }
                    }
                },
                complete: function () {
                    domElements.spanAddProblemAccountUserName.removeClass("loading-value");
                },
                error: function (errMsg) {
                    domElements.spanAddProblemAccountUserName.removeClass("loading-value");
                    console.log(errMsg);
                }
            });
        }
    }

    function GetInputUserName(textTagRemove) {
        let userNameList = [];
        let tags = domElements.spanAddProblemAccountUserName.find('tags tag');

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

    function BindExceptionValidateInputUserName() {
        domElements.spanAddProblemAccountUserName.find('tags tag').each(function (_, tag) {
            $(tag).removeClass('valid').addClass('invalid-username');
            $(tag).prop('title', "Exception");
        });
    }

    function BindResultValidatedInputUserName(data) {
        domElements.spanAddProblemAccountUserName.find('tags tag').each(function (_, tag) {
            $(tag).removeClass('invalid-username').removeClass('invalid-duplicated').addClass('valid');
        });

        data.ProblemAccountInvalidList.map(function (x) {
            let usernameEqual = $("tag[data-user-name='" + x.UserName + "']");
            if (usernameEqual !== undefined) {
                usernameEqual.removeClass('valid').addClass('invalid-username');
                usernameEqual.prop('title', x.ErrMsg);
            }
        });

        if (data.ProblemAccountInvalidList.length === 0) {
            domElements.btnSave.prop('disabled', false);
        }
        else
            domElements.btnSave.prop('disabled', true);
    }

    function SaveProblemAccount() {
        let params = {};
        params.UserNames = GetInputUserName();
        params.CategoryID = domElements.cbbProblemAccountCategories.val();
        params.Comment = domElements.txtComment.val().trim();
        params.PAAction = domElements.spanProblemAccountActionHidden.val();
        params.IsUnmarkDownlines = domElements.ckbUnmarkDownlines.is(":checked");
        params.CategoryGroup = $('option:selected', domElements.cbbProblemAccountCategories).attr('data-categorygroup');
        params.IsRobot = $('option:selected', domElements.cbbProblemAccountCategories).attr('data-isrobot');
        params.CategoryIDAgency = $('option:selected', domElements.cbbProblemAccountCategories).attr('data-categoryidagency');
        params.CategoryGroupAgency = $('option:selected', domElements.cbbProblemAccountCategories).attr('data-categorygroupidagency');

        if (params.PAAction == 1) {
            SaveMarkProblemAccount(params);

        } else if (params.PAAction == 2) {
            SaveUnmarkProblemAccount(params);

        }
    }

    function SaveMarkProblemAccount(params) {
        $.ajax(common.modifyRelativeUrl('ProblemAccountManagement/SaveMarkProblemAccount'), {
            type: 'POST',
            data: params,
            beforeSend: function () {
                domElements.spanAddProblemAccountUserName.addClass("loading-block");
            },
            success: function (response) {
                if (response.ErrorCode === 0) {
                    common.showSuccess("Mark Problem Account successful. Please wait, Data Synchronization may take a few minutes!");
                    domElements.divAddProblemAccount.dialog('close');
                    setTimeout(() => {
                        GetProblemAccountReport();
                    }, 1000);
                } else {
                    if (response.ErrorMessage != '') {
                        common.showError(response.ErrorMessage, 2000);
                    }
                    BindResultValidatedInputUserName(response.ResultValidated);
                }
            },
            complete: function () {
                domElements.spanAddProblemAccountUserName.removeClass("loading-block");
            },
            error: function (errMsg) {
                domElements.spanAddProblemAccountUserName.removeClass("loading-block");
                console.log(errMsg);
            }
        });
    }

    function SaveUnmarkProblemAccount(params) {
        common.showConfirmation("<div class='popup - content'>Are you sure to unmark this problem account?</div>",
            () => {
                $.ajax(common.modifyRelativeUrl('ProblemAccountManagement/SaveUnmarkProblemAccount'), {
                    type: 'POST',
                    data: params,
                    beforeSend: function () {
                        domElements.spanAddProblemAccountUserName.addClass("loading-block");
                    },
                    success: function (response) {
                        if (response.ErrorCode === 0) {
                            common.showSuccess("Unmark Problem Account successful. Please wait, Data Synchronization may take a few minutes!");
                            domElements.divAddProblemAccount.dialog('close');
                            setTimeout(() => {
                                GetProblemAccountReport();
                            }, 1000);
                        } else {
                            if (response.ErrorMessage != '') {
                                common.showError(response.ErrorMessage, 2000);
                            }
                            BindResultValidatedInputUserName(response.ResultValidated);
                        }
                    },
                    complete: function () {
                        domElements.spanAddProblemAccountUserName.removeClass("loading-block");
                    },
                    error: function (errMsg) {
                        domElements.spanAddProblemAccountUserName.removeClass("loading-block");
                        console.log(errMsg);
                    }
                });
            },
            () => { });
    }

    function RefreshProblemCategories() {
        domElements.cbbCategory.empty();

        categoryGroup.isIncludeMember = domElements.cbbRole.val().some(x => x == 1);
        categoryGroup.isIncludeAgency = domElements.cbbRole.val().some(x => x != 1);

        var categories = {};

        categoryGroup.list
            .filter(x =>
                (categoryGroup.isIncludeMember && x.RoleGroup == 1) ||
                (categoryGroup.isIncludeAgency && x.RoleGroup == 2))
            .forEach(x => {
                if (categories[x.CategoryName]) {
                    categories[x.CategoryName].CategoryIDs += `,${x.CategoryIDs}`;
                }
                else {
                    categories[x.CategoryName] = {
                        CategoryName: x.CategoryName,
                        CategoryIDs: x.CategoryIDs
                    };
                }
            });

        Object.values(categories).forEach(x => {
            domElements.cbbCategory.append("<option selected value='" + x.CategoryIDs + "'>" + x.CategoryName + "</option>");
        });

        domElements.cbbCategory.multiselect("refresh");
    }

    function GetProblemCategories() {
        $.ajax(common.modifyRelativeUrl('ProblemAccountManagement/GetProblemCategories'), {
            type: 'GET',
            success: function (response) {
                if (response) {
                    categoryGroup.list = response;

                    RefreshProblemCategories();
                }
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function GetProblemAccountCategories(setVal) {
        domElements.cbbProblemAccountCategories.empty();

        $.ajax(common.modifyRelativeUrl('ProblemAccountManagement/GetProblemAccountAllCategories'), {
            type: 'GET',
            success: function (response) {

                const mergedCategories = [];

                response.forEach(category => {
                    if (category.RoleGroup === 1) {
                        const agencyCategory = response.find(c => c.CategoryName === category.CategoryName && c.RoleGroup === 2);

                        if (agencyCategory) {
                            mergedCategories.push({
                                CategoryID: category.CategoryID,
                                CategoryName: category.CategoryName,
                                CategoryGroupID: category.CategoryGroupID,
                                IsRobot: category.IsRobot,
                                RelevantCategoryGroup: category.RelevantCategoryGroup,
                                CategoryIDAgency: agencyCategory.CategoryID,
                                CategoryGroupIDAgency: agencyCategory.CategoryGroupID,
                                RelevantCategoryGroupAgency: agencyCategory.RelevantCategoryGroup
                            });
                        }
                    }
                });

                mergedCategories.map(function (x) {
                    domElements.cbbProblemAccountCategories.append("<option value='" + x.CategoryID + "' data-categorygroup='" + x.CategoryGroupID + "' data-isrobot='" + x.IsRobot + "' data-categoryidagency='" + x.CategoryIDAgency + "' data-categorygroupidagency='" + x.CategoryGroupIDAgency + "'>" + x.CategoryName + "</option>");
                });

                domElements.cbbProblemAccountCategories.prop('selectedIndex', 0);
            },
            complete: function () {
                if (setVal) {
                    domElements.cbbProblemAccountCategories.val(setVal).change();
                }
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function ExportToExcel() {
        if (ValidateRangeDateExport()) {
            common.showSuccess('Your filter export is being processed. Please wait, this may take some minutes.', 5000);

            common.showLoading();

            let request = new XMLHttpRequest();
            request.open('POST', common.modifyRelativeUrl('ProblemAccountManagement/ExportToExcel'), true);
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
                    let filenameFormat = "PAM_{0}{1}";
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

            let params = GetParams(true);
            request.send(JSON.stringify(params));
        }
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

    function ShowPopupMarkNew(isNavigation) {
        domElements.grpCategories.removeClass('hide');
        domElements.grpCkboxAffectedDownline.addClass('hide');
        GetProblemAccountCategories(isNavigation ? '52' : null);
        domElements.txtComment.val('');
        domElements.spanProblemAccountActionHidden.val('1'); // Mark

        InitInputTagUsername();

        if (isNavigation) {
            domElements.inputTagUsername.data('tagify').addTags(domElements.hdUserNames.val());
        }

        domElements.divAddProblemAccount.dialog({
            modal: true,
            width: '800px',
            title: 'MARK NEW PROBLEM ACCOUNT',
            resizable: true,
            draggable: true,
            close: function () {
                $(this).dialog('close');
            }
        });
    }

    function Initial() {
        domElements.dpkFromDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true
            })
            .datepicker("setDate", new Date(Date.now() - 12096e5))
            .change(SetRestrictToDate);

        domElements.dpkToDate
            .datepicker({
                dateFormat: "M dd, yy",
                changeMonth: true,
                changeYear: true
            })
            .datepicker("setDate", new Date())
            .change(SetRestrictFromDate);

        domElements.cbbSite.multiselect({
            menuWidth: 280,
            menuHeight: 280,
            selectedText: "# of # selected",
            noneSelectedText: "Select Site",
            enableClickableOptGroups: true,
            includeSelectAllOption: true,
            groupsCollapsable: true,
            optGroupChecked: true
        }).multiselectfilter({
            width: 217,
            placeholder: '',
            label: ''
        });

        domElements.cbbRole.multiselect({
            menuWidth: 280,
            menuHeight: 280,
            selectedText: "# of # selected",
            noneSelectedText: "Select Role"
        }).multiselectfilter({
            width: 240,
            placeholder: '',
            label: ''
        });

        domElements.cbbCategory.multiselect({
            menuWidth: 320,
            menuHeight: 280,
            selectedText: "# of # selected",
            noneSelectedText: "Select Category"
        }).multiselectfilter({
            width: 240,
            placeholder: '',
            label: ''
        });

        GetProblemCategories();

        domElements.cbbRole.on('change', function () { RefreshProblemCategories() });

        domElements.btnGenerate.on('click', function () { GetProblemAccountReport(); });

        domElements.btnExportToExcel.on('click', function () { ExportToExcel(); });

        domElements.btnMarkNewProblemAccount.on('click', function () {
            ShowPopupMarkNew(false);
        });

        domElements.btnUnmarkProblemAccount.on('click', function () {
            domElements.grpCategories.addClass('hide');
            domElements.grpCkboxAffectedDownline.addClass('hide');
            domElements.ckbUnmarkDownlines.prop("checked", false);
            domElements.txtComment.val('');
            domElements.spanProblemAccountActionHidden.val('2'); // Unmark

            InitInputTagUsername();

            domElements.divAddProblemAccount.dialog({
                modal: true,
                width: '800px',
                title: 'UNMARK PROBLEM ACCOUNT',
                resizable: true,
                draggable: true,
                close: function () {
                    $(this).dialog('close');
                }
            });
        });

        domElements.btnSave.on('click', () => SaveProblemAccount());

        domElements.btnCancel.on('click', () => {
            domElements.divAddProblemAccount.dialog('close');
        });

        // Mark PA from other page
        if (domElements.hdUserNames.val()) {
            ShowPopupMarkNew(true);

            setTimeout(() => {
                $(document).mouseup();
            }, 100);
        }
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ProblemAccountManagement);