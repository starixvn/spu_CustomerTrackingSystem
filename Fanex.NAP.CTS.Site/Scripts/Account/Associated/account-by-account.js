function account_by_account() {
    var divAddAssociation = $('#divAddAssociation');
    var btnAddAssociation = $('#btnAddAssociation');
    var btnSearch = $('#account-by-account-btn-search');
    var btnViewOnGraph = $('#account-by-account-btn-on-graph');
    var associatedByAccountTable = $('#account-by-account-table');    
    var asscomment = $('#txtasscomment');
    var btnSaveAssociation = $('#btnsaveassociation');
    var btnCancelAssociation = $('#btncancelassociation');
    var _username;
    var dataTable;
    let isHostPermission;
    let spanAddAssociationUserName = $('#spanAddAssociationUserName');
    let spanAddAssociationUserNameMaxTag = $('#spanAddAssociationUserNameMaxTag');
    let spanAddAssociationUserNameMaxTagHidden = $('#spanAddAssociationUserNameMaxTagHidden');
    let inputTagUsername = $('#inputTagUsername'); 
    let maxTags = 50;

    function RenderTag(obj) {
        let html = "";

        if (obj.AffectedTag === 1)
            html += "<span class='icon icon-Affected'  title='Affected'></span>";       

        if (obj.FlaggedTag === 2)
            html += "<span class='icon icon-Flagged' title='Flagged'></span>";        

        return html === "" ? "-" : html;
    }

    function RenderUsername(obj) {
        if (obj.Username !== null) {
            var url = common.modifyRelativeUrl('Account/Index/' + obj.CTSCustId);

            return '<a class="info-bng" href="' + url + '">' + obj.Username + '</a>';
        }

        return null;
    }

    function RenderGrid() {
        var url = common.modifyRelativeUrl('Account/GetAssociatedByAccount');
        var ctsCustId = $("#hdCTSCustId").val();
        var subscriberId = $("#hdSubscriberId").val();

        return associatedByAccountTable.DataTable({
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
                        return $("#radio03-01").is(':checked');
                    }
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
                { targets: [1], width: "20%" },
                { targets: [2], width: "20%" },
                { targets: [3], width: "10%" },
                { targets: [4], width: "10%" },
                { targets: [5], width: "10%" },
                { targets: [6], width: "10%" },
                { targets: [7], width: "15%" }
            ],
            columns: [
                { data: obj => RenderTag(obj) },
                { data: obj => RenderUsername(obj, ctsCustId) },
                { data: obj => obj.LoginId },
                { data: obj => obj.Status },
                { data: obj => obj.Subscriber },
                { data: obj => obj.AssociationTypeName },
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
            ]
        });
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
        common.showConfirmation('Do you want to update Association Status between <b>' + _username + '</b> and <b>' + obj.Username + '</b> ?',
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

                $('#totalAssAccount').html(response.TotalAccounts);
                $('#totalAssDevice').html(response.TotalDevices);
                $('#totalAssIP').html(response.TotalIps);
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
                    btnSearch.trigger("click");
                }, 2000);
            }
        });
    }

    function ValidateManualAssociationMultiple(fromCTSCustId) {
        let userNameList = GetInputUserName();

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

    function BindResultValidatedManualAssociationMultiple(data) {
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
                        }, 1000);                        
                    } else {
                        BindResultValidatedManualAssociationMultiple(response.ResultValidated);
                    }
                },
                complete: function () {
                    divAddAssociation.removeClass("loading-block");
                    btnSearch.trigger("click");
                    GetTotalAssociation(fromCTSCustId);
                },
                error: function (errMsg) {
                    console.log(errMsg);
                }
            });
        }
    }

    function GetInputUserName() {
        let userNameList = [];
        let tags = spanAddAssociationUserName.find('tags tag');

        tags.map(function (_, ele) {
            userNameList.push($(ele).text().trim());
        });

        return userNameList.join();
    }       

    function InitInputTagUsername(fromCTSCustid) {
        spanAddAssociationUserNameMaxTag.text(maxTags);
        spanAddAssociationUserNameMaxTagHidden.text(maxTags);

        function RemoveTag() {
            let maxCountTags = parseInt(spanAddAssociationUserNameMaxTag.text());
            if (maxCountTags < maxTags)
                spanAddAssociationUserNameMaxTag.text(maxCountTags + 1);

            let invalidDuplicatedTags = spanAddAssociationUserName.find('tags tag.invalid-duplicated');
            let invalidUsernameTags = spanAddAssociationUserName.find('tags tag.invalid-username');
            let notInvalid = invalidDuplicatedTags.length + invalidUsernameTags.length;
            if (notInvalid <= 1)
                btnSaveAssociation.prop('disabled', false);
            else
                btnSaveAssociation.prop('disabled', true);
        }

        function OnChangeTags(tagName, cssClass) {
            let maxCountTags = parseInt(spanAddAssociationUserNameMaxTag.text());

            if (maxCountTags > 0) {
                let $tag = $(tagName.tag);

                $tag.attr('data-user-name', $tag.text());
                $tag.addClass(cssClass);
                let btnRemove = $tag.find('x');

                btnRemove.on('click', () => RemoveTag());

                spanAddAssociationUserNameMaxTag.text(maxCountTags - 1);
            } else {
                var tagify = inputTagUsername.tagify().data('tagify');
                tagify.removeTags(tagName.tag);
            }
            btnSaveAssociation.prop('disabled', true);
        }
        
        inputTagUsername.tagify({
            pattern: null,
            delimiters: /\r?\n|\r| |,/,
            keepInvalidTags: true,
            skipInvalid: false,
            callbacks: {},
            addTagOnBlur: false,
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
            })
            .off('invalid').on('invalid', function (_, tagName) {
                OnChangeTags(tagName, 'invalid-duplicated');
            })
            .off('blur').on('blur', function (_, tagName) {
                let maxTag = spanAddAssociationUserNameMaxTag.text();
                let maxTagHidden = spanAddAssociationUserNameMaxTagHidden.text();
                let isValidated = maxTag === maxTagHidden;

                if (!isValidated) {
                    ValidateManualAssociationMultiple(fromCTSCustid);
                    spanAddAssociationUserNameMaxTagHidden.text(maxTag);
                }
            });

        inputTagUsername.off('keydown').on('keydown', function () {
            var key = event.keyCode || event.charCode;

            if (key === 8)
                RemoveTag();
        });

        inputTagUsername.tagify().data('tagify').removeAllTags();
        spanAddAssociationUserName.find('tags').attr('tabindex', "1");
        btnSaveAssociation.prop('disabled', true);
    }

    function DisableButton(hostPermission) {
        isHostPermission = hostPermission === "True";

        if (!isHostPermission)
            btnAddAssociation.prop('disabled', true);
    }

    this.Initial = function (ctsCustid, username, subscriberId, subscriber, hostPermission) {
        _username = username;        

        DisableButton(hostPermission);

        $("#account-by-account-cbb-status").multiselect();

        dataTable = RenderGrid();

        btnSearch.on('click', function () {
            $(this).prop('disabled', true);
            if (dataTable) {
                dataTable.ajax.reload(null, true);
            }
        });

        btnViewOnGraph.on('click', function () {
            var info = dataTable.page.info();
            var formGraph = new graph();
            formGraph.GenerateFullPath(ctsCustid, username, $('#association-cbb-status').val(), info.start, info.length);
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