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
    let inputTagUsername = $('#inputTagUsername');
    let maxCountTags = 2;

    function RenderTag(obj) {

        var html = "";

        if (obj.AffectedTag === 1) {
            html += "<span class='icon icon-Affected'  title='Affected'></span>";
        }

        if (obj.FlaggedTag !== 0) {
            if (obj.FlaggedTag === 2) {
                html += "<span class='icon icon-Flagged' title='Flagged'></span>";
            }
            else {
                html += "<span class='icon icon-Flagged-With-Evidence' title='Flagged with Evidence 100'></span>";
            }
        }

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
                        if (isHostPermission === "True") {
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
        html += '<div class="toggle-text-off' + (isHostPermission === "False" ? ' disabled"' : '') + '"> Unlinked</div >';
        html += '<div class="glow-comp"></div> <div class="toggle-button ' + (isHostPermission === "False" ? ' disabled"' : '') + '"></div>';
        html += '<div class="toggle-text-on' + (isHostPermission === "False" ? ' disabled"' : '') + '"> Linked</div>';
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

    function AddAssociation(ctsCustId, subscriberId, rootUsername, subscriber) {
        var url = common.modifyRelativeUrl('Account/AddAssociation');
        var params = {};
        params.Username = assusername.val();
        params.RootUsername = rootUsername;
        params.Comment = asscomment.val();
        params.FromSubscriberId = subscriberId;
        params.FromCTSCustId = ctsCustId;

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.errorCode === 0) {
                    associatedByAccountTable.find('tbody').addClass('loading');
                    common.showSuccess('Added association successfully!', 2000);
                    divAddAssociation.dialog('close');

                    $('#association-cbb-status').val(0);
                    $("#account-by-account-cbb-association-type").val(3);

                    setTimeout(() => {
                        btnSearch.trigger("click");
                    }, 2000);
                }
                else if (response.errorCode === -2) {
                    common.showError('Username "' + params.Username + '" is invalid.');
                }
                else if (response.errorCode === -1 || response.errorCode === -100) {
                    common.showError('Username already existed in "' + params.Username + '" association list.');
                }
                else {
                    common.showError('Added association fail!');
                }
            },
            complete: function () {
                GetTotalAssociation(ctsCustId);
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
                    spanAddAssociationUserName.addClass("loading");
                },
                success: function (response) {
                    BindResultValidatedManualAssociationMultiple(response);
                },
                complete: function () {
                    spanAddAssociationUserName.removeClass("loading");
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
                },
                success: function (response) {
                    if (response.ErrorCode === 0) {
                        setTimeout(() => {
                            common.showSuccess("Add association successful");
                            divAddAssociation.dialog('close');                            
                        }, 2000);
                    } else {
                        BindResultValidatedManualAssociationMultiple(response.ResultValidated);
                    }
                },
                complete: function () {
                    btnSearch.trigger("click");
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

    function OnChangeTags(tagName, cssClass) {
        if (maxCountTags > 0) {
            let $tag = $(tagName.tag);

            $tag.attr('data-user-name', $tag.text());
            $tag.addClass(cssClass);
        } else {
            var tagify = inputTagUsername.tagify().data('tagify');
            tagify.removeTags(tagName.tag);            
        }

        maxCountTags = maxCountTags > 0
            ? maxCountTags - 1
            : 0;
    }

    function InitInputTagUsername() {
        inputTagUsername.tagify({
            pattern: null,
            keepInvalidTags: true,
            skipInvalid: false,
            callbacks: {},
            addTagOnBlur: true,
            duplicates: false,
            editTags: 0,
            whitelist: [],
            blacklist: [],
            maxTags: maxCountTags,
            enforceWhitelist: false,
            dropdown: {
                position: "input",
                enabled: 0
            }
        })            
            .on('add', function (_, tagName) {
                OnChangeTags(tagName, 'valid');
                isValidated = false;
            })
            .on('invalid', function (_, tagName) {
                OnChangeTags(tagName, 'invalid-duplicated');
            });
    
        spanAddAssociationUserName.find('tags').attr('tabindex', "1");        
        btnSaveAssociation.prop('disabled', true);        
    }

    function DisableButton(hostPermission) {
        isHostPermission = hostPermission;
        if (isHostPermission === "False") {
            btnAddAssociation.prop('disabled', true);
        }
    }

    this.Initial = function (ctsCustid, username, subscriberId, subscriber, hostPermission) {
        _username = username;
        let isValidated = true;

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

        InitInputTagUsername();

        btnAddAssociation.on('click', function () {
            asscomment.val('');   
            maxCountTags = 50;

            var tagify = inputTagUsername.tagify().data('tagify');
            tagify.removeAllTags();

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
        inputTagUsername.on('blur', () => {
            if (!isValidated) {
                ValidateManualAssociationMultiple(ctsCustid);
                isValidated = true;
            }
        });
    };
}