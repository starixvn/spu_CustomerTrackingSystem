function AssociationDetection() {
    let dataTable;    
    let maxTags = 200;
    let domElements = {
        hdIsHostPermission: $('#hdIsHostPermission'),
        hdIsOverwritePermission: $('#hdIsOverwritePermission'),       
        divValidation: $('#divValidation'),                
        btnResultValidationClose: $('#btnResultValidationClose'),        
        divAddUserName: $('#divAddUserName'),
        spanAddUserName : $('#spanAddUserName'),
        spanAddUserNameMaxTag: $('#spanAddUserNameMaxTag'),        
        spanAddChangeHidden: $('#spanAddChangeHidden'),
        inputTagUsername: $('#inputTagUsername'),
        btnDetect: $('#btnAssociationDetection'),
        tableResult: $('#tblAssociationDetection')
    };

    function GenerateGrid(data) {

        if ($.fn.DataTable.isDataTable("#tblAssociationDetection")) {
            domElements.tableResult.DataTable().clear().destroy();
        }

        var groupColumn = 0;        
        return domElements.tableResult.DataTable({
            language: {
                lengthMenu: "",
                info: "",
                infoEmpty: "",
                zeroRecords: ""
            },
            info: false,
            ordering: false,
            searching: false,
            scrollX: false,
            scrollCollapse: true,
            paging: false,
            searching: false,
            columnDefs: [
                { "visible": false, "targets": groupColumn },
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4 , 5], className: "text-center" },
                { targets: [0], width: "5%" },
                { targets: [1], width: "20%" },
                { targets: [2], width: "10%" },
                { targets: [3], width: "25%" },
                { targets: [4], width: "25%" },
                { targets: [5], width: "15%" },
            ],
            columns: [
                { data: obj => obj.GroupId },
                { data: obj => obj.UserName },
                { data: obj => obj.Status },
                { data: obj => obj.Evidence },
                {
                    data: obj => common.renderCategoryName(obj.CustomerCategory), createdCell: (cell, _, row) => {
                        common.renderCategoryColor(cell, row);
                    }
                },
                { data: obj => obj.CustomerClass }
            ],
            "drawCallback": function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;

                api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        $(rows).eq(i).before(
                            '<tr class="group"><td colspan="5"> <i class="icon-chevron-down"></i>' + 'Group ' + group + '</td></tr>'
                        );

                        last = group;
                    }
                });
            }
        }).rows.add(data).draw();
    }

    function ValidateUsernames(textTagRemove) {
        let userNames = GetInputUserName(textTagRemove);

        if (userNames !== '') {
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

        let isEnableAdd = data.UserNameInvalidList.length === 0 && data.UserNameDuplicatedList.length === 0 && data.UserNameValidList.length >= 2;

        domElements.btnDetect.prop('disabled', !isEnableAdd);
    }    

    function DetectUserName() {
        let userNames = GetInputUserName();

        if (userNames !== '') {
            let params = {};
            params.userNames = userNames;

            $.ajax(common.modifyRelativeUrl('AssociationDetection/GetAssociationDetection'), {
                type: 'POST',
                data: params,
                beforeSend: function () {
                    domElements.divAddUserName.addClass("loading-block");
                },
                success: function (response) {
                    GenerateGrid(response);
                },
                complete: function () {
                    domElements.divAddUserName.removeClass("loading-block");
                    InitInputTagUsername();
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
            let disabled = notInvalid <= 1 && validTags.length > 3;

            domElements.btnDetect.prop('disabled', !disabled);
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

                    //EnableButtonAdd();
                });

                domElements.spanAddUserNameMaxTag.text(maxCountTags - 1);
            } else {
                domElements.inputTagUsername.tagify().data('tagify').removeTags(tagName.tag);
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

    function Initial() {        
        domElements.divValidation.prop('hidden', true);                
        InitInputTagUsername();
        domElements.btnDetect.on('click', () => DetectUserName());

    }

    $(document).ready(function () {
        Initial();
    });
}

$(AssociationDetection);