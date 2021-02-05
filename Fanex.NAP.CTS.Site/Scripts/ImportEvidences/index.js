function ImportEvidences() {
    let dataTable;
    let isOverwriteEvidence = false;
    let maxTags = 200;
    let domElements = {
        hdIsHostPermission: $('#hdIsHostPermission'),
        hdIsOverwritePermission: $('#hdIsOverwritePermission'),
        tabs: $("#tabsDetail"),        
        spanFileName: $('#spanFileName'),
        inputFileUpload: $('#inputFileUpload'),
        btnBrowse: $('#btnBrowse'),
        btnNoteToggle: $('#btnNoteToggle'),
        divNote: $('#divNote'),
        divResult: $('#divResult'),
        divValidation: $('#divValidation'),
        divResultParseFile: $('#divResultParseFile'),
        tableResult: $('#tableResult'),
        btnDownloadTemplate: $('#btnDownloadTemplate'),
        btnImport: $('#btnImport'),
        btnResultValidationClose: $('#btnResultValidationClose'),
        overwriteCheckBox: $('#overwriteCheckbox'),
        containerCheckBox: $('#containerCheckbox'),
        divAddUserName: $('#divAddUserName'),
        spanAddUserName : $('#spanAddUserName'),
        spanAddUserNameMaxTag : $('#spanAddUserNameMaxTag'),
        spanAddChangeHidden : $('#spanAddChangeHidden'),
        inputTagUsername: $('#inputTagUsername'),
        btnRemoveEvidence: $('#btnRemoveEvidence')
    };

    let isHostPermission = domElements.hdIsHostPermission.attr('IsHostPermission') === "True";
    let isOverwritePermission = domElements.hdIsOverwritePermission.attr('IsOverwritePermission') === "True";
    let isEnableOverwriteButton = (isHostPermission && isOverwritePermission);

    function ImportEvidences() {
        let fileName = domElements.inputFileUpload.val();

        if (ValidateInputFile(fileName)) {
            let formdata = new FormData();
            let fileUpload = domElements.inputFileUpload.get(0);
            let files = fileUpload.files;

            formdata.append(files[0].name, files[0]);
            formdata.append("isOverwrite", isOverwriteEvidence);

            $.ajax(common.modifyRelativeUrl('ImportEvidences/ImportFromExcel'), {
                type: 'POST',
                data: formdata,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    common.showLoading();
                    domElements.divResultParseFile.prop('hidden', true);
                    domElements.divResult.prop('hidden', true);
                },
                success: function (response) {
                    ResultCheckAccountEvidence(response);
                    domElements.inputFileUpload.val("");
                    domElements.spanFileName.text("");
                },
                complete: function () {
                    common.closeLoading();
                },
                error: function (errMsg) {
                    console.log(errMsg);
                    common.closeLoading();
                }
            });
        }
    }

    function ValidateInputFile(fileName) {
        let fileExtension = ['xls', 'xlsx'];
        let extension = fileName.replace(/^.*\./, '');

        if (fileName.length === 0) {
            common.showError("Please select a file");
            return false;
        }
        
        if ($.inArray(extension, fileExtension) === -1) {
            common.showError("Please select only excel files.");
            return false;
        }

        return true;
    }

    function ResultCheckAccountEvidence(data) {
        if (data.ErrorCode === 1) {
            HiddenNote();

            domElements.divResult.prop('hidden', false);

            dataTable
                ? domElements.tableResult.DataTable().clear().rows.add(data.ResultCheckAccountEvidenceList).draw()
                : dataTable = GenerateGrid(data.ResultCheckAccountEvidenceList);

        } else if (data.ErrorCode === 2) {
            HiddenNote();

            var html = "<span style='color:red; font-weight: bold'>" + data.ErrorMessage + "</span>";
            domElements.divResultParseFile.prop('hidden', false);
            domElements.divResultParseFile.html(html);
        } else {
            domElements.divValidation.prop('hidden', false);

            domElements.divValidation.dialog({
                appendTo: '.import-evidences',
                modal: true,
                width: '600px',
                title: 'Successful Validation',
                resizable: true,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            });
        }
    }

    function GenerateGrid(data) {
        return domElements.tableResult.DataTable({
            language: {
                lengthMenu: "",
                info: "",
                infoEmpty: "",
                zeroRecords: ""
            },
            serverSide: false,
            paging: false,
            ordering: false,
            searching: false,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3], className: "text-center" },
                { targets: [0], width: "10%" },
                { targets: [1], width: "20%" },
                { targets: [2], width: "20%" }
            ],
            columns: [
                { data: obj => obj.RowId },
                { data: obj => SubString(obj.Username) },
                { data: obj => SubString(obj.EvidenceCode) },
                { data: obj => obj.RowStatus }
            ]
        }).rows.add(data).draw();
    }

    function SubString(text) {
        return text !== '' && text.length > 50
            ? text.substring(0, 47) + '...'
            : text;
    }

    function ToggleNote() {
        let classChevronDown = domElements.btnNoteToggle.hasClass('icon-chevron-down');
        if (classChevronDown) {
            domElements.btnNoteToggle.removeClass('icon-chevron-down').addClass('icon-chevron-right');
            domElements.divNote.prop('hidden', true);
        } else {
            domElements.btnNoteToggle.removeClass('icon-chevron-right').addClass('icon-chevron-down');
            domElements.divNote.prop('hidden', false);
        }        
    }

    function HiddenNote() {
        let classChevronDown = domElements.btnNoteToggle.hasClass('icon-chevron-down');
        if (classChevronDown) {
            domElements.btnNoteToggle.removeClass('icon-chevron-down').addClass('icon-chevron-right');
            domElements.divNote.prop('hidden', true);
        }
    }

    function OverwirteEvidences()
    {
        if (domElements.overwriteCheckBox.prop('checked')) {
            common.showConfirmation("<div class='popup - content'>Are you sure about this? For confirm this action, all evidences from account in the list will be <strong style='color: red'> REMOVED </strong></div>",
                () => {
                    isOverwriteEvidence = true;
                    domElements.overwriteCheckBox.prop('checked', true);},
                () => {
                    isOverwriteEvidence = false;
                    domElements.overwriteCheckBox.prop('checked', false);
                }
            );
        }
        else
            isOverwriteEvidence = false;
    }

    function ValidateAcountsForRemoveEvidence(textTagRemove) {
        let userNames = GetInputUserName(textTagRemove);

        if (userNames !== '') {
            let params = {};
            params.userNames = userNames;

            $.ajax(common.modifyRelativeUrl('ImportEvidences/ValidateAccountsForRemoveEvidences'), {
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
        spanAddUserName.find('tags tag').each(function (_, tag) {
            $(tag).removeClass('valid').addClass('invalid-username');
        });
    }

    function BindResultValidatedAccounts(data) {
        domElements.spanAddUserName.find('tags tag').each(function (_, tag) {
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

        domElements.btnRemoveEvidence.prop('disabled', !isEnableAdd);
    }    

    function InsertAccountsForRemoveEvidences() {
        let userNames = GetInputUserName();

        if (userNames !== '') {
            let params = {};
            params.userNames = userNames;

            $.ajax(common.modifyRelativeUrl('ImportEvidences/InsertAccountsForRemoveEvidences'), {
                type: 'POST',
                data: params,
                beforeSend: function () {
                    domElements.divAddUserName.addClass("loading-block");
                },
                success: function (response) {
                    if (response.ErrorCode === 0) {
                        common.showSuccess("Remove Evidence successful");
                        domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
                        domElements.spanAddUserNameMaxTag.text(maxTags);
                        domElements.spanAddChangeHidden.text("0");
                    } else {
                        BindResultValidatedAccounts(response.ValidatedResult);
                    }
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

        function EnableButtonAdd() {
            let validTags = domElements.spanAddUserName.find('tags tag.valid');
            let invalidDuplicatedTags = domElements.spanAddUserName.find('tags tag.invalid-duplicated');
            let invalidUsernameTags = domElements.spanAddUserName.find('tags tag.invalid-username');
            let notInvalid = invalidDuplicatedTags.length + invalidUsernameTags.length;
            let disabled = notInvalid <= 1 && validTags.length > 1;

            domElements.btnRemoveEvidence.prop('disabled', !disabled);
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

                    ValidateAcountsForRemoveEvidence(currentTag.text().trim());

                    EnableButtonAdd();
                });

                domElements.spanAddUserNameMaxTag.text(maxCountTags - 1);
            } else {
                domElements.inputTagUsername.tagify().data('tagify').removeTags(tagName.tag);
            }

            domElements.btnRemoveEvidence.prop('disabled', true);
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
                    ValidateAcountsForRemoveEvidence("");
                    domElements.spanAddUserName.removeClass("loading-value");
                    domElements.spanAddChangeHidden.text("0");
                }, 500);
            }
        });

        domElements.inputTagUsername.tagify().data('tagify').removeAllTags();
        domElements.spanAddUserName.find('tags').attr('tabindex', "1");
        domElements.btnRemoveEvidence.prop('disabled', true);
    }

    function Initial() {
        domElements.divNote.prop('hidden', false);
        domElements.divResultParseFile.prop('hidden', true);
        domElements.divResult.prop('hidden', true);
        domElements.divValidation.prop('hidden', true);        
        domElements.btnImport.prop('disabled', !isHostPermission);
        domElements.overwriteCheckBox.prop('disabled', !isEnableOverwriteButton);

        domElements.btnDownloadTemplate.on('click', function () {
            common.downloadFile(common.modifyRelativeUrl('ImportEvidences/DownloadTemplateExcel'), '0');
        });

        domElements.inputFileUpload.on('change', function () {
            domElements.spanFileName.text(domElements.inputFileUpload.get(0).files[0].name);
        });

        domElements.btnResultValidationClose.on('click', function () {
            domElements.divValidation.dialog('close');
        });

        domElements.btnNoteToggle.on('click', () => ToggleNote());        

        if (isHostPermission) {
            domElements.btnImport.on('click', () => ImportEvidences());      
        }

        if (isEnableOverwriteButton) {
            domElements.overwriteCheckBox.on('click', () => OverwirteEvidences());
        }
        else {
            domElements.containerCheckBox.addClass('none-pointer');
        }

        domElements.tabs.tabs();

        InitInputTagUsername();

        domElements.btnRemoveEvidence.on('click', function () {
            let msgConfirm = "<div class='popup - content'>Are you sure about this? <br/>For confirm this action, all evidences from account in the list will be <strong style='color: red'> REMOVED </strong></div>";

            common.showConfirmation(msgConfirm,
                () => InsertAccountsForRemoveEvidences());
            }
        );
    }

    $(document).ready(function () {
        Initial();
    });
}

$(ImportEvidences);