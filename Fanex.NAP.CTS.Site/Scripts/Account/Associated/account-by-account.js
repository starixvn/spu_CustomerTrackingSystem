function account_by_account() {
    var divAddAssociation = $('#divAddAssociation');
    var btnAddAssociation = $('#btnAddAssociation');
    var btnSearch = $('#account-by-account-btn-search');
    var btnViewOnGraph = $('#account-by-account-btn-on-graph');
    var associatedByAccountTable = $('#account-by-account-table');
    var assusername = $('#txtassusername');
    var asscomment = $('#txtasscomment');
    var btnSaveAssociation = $('#btnsaveassociation');
    var btnCancelAssociation = $('#btncancelassociation');
    var _username;
    var dataTable;
    function RenderTag(obj) {

        var html = "";

        if (obj.AffectedTag == 1) {
            html += "<span class='icon icon-Affected'  title='Affected'></span>";
        }

        if (obj.FlaggedTag != 0) {
            if (obj.FlaggedTag == 2) {
                html += "<span class='icon icon-Flagged' title='Flagged'></span>";
            }
            else {
                html += "<span class='icon icon-Flagged-With-Evidence' title='Flagged with Evidence 100'></span>";
            }
        }

        return html == "" ? "-" : html;
    }

    function RenderUsername(obj) {
        if (obj.Username != null) {
            var url = common.modifyRelativeUrl('Account/Index/' + obj.CTSCustId);

            return '<a class="info-bng" href="' + url + '">' + obj.Username + '</a>';
        }

        return null;
    }

    function RenderGraphImage() {
        return '<span class="icon-path graph" title="Graph View"></span>';
    }

    function RenderGrid() {
        var url = common.modifyRelativeUrl('Account/GetAssociatedByAccount');
        var ctsCustId = $("#hdCTSCustId").val();
        var subscriberId = $("#hdSubscriberId").val();

        return associatedByAccountTable.DataTable({
            serverSide: true,
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
                    previous: "Back"
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
                    data: obj => AssociationStatus(obj.IsActive), createdCell: (cell, _, row) => {
                        $('.toggle', cell).click(function (e) {
                            e.preventDefault(); 
                           
                            ActiveRemove(row, ctsCustId, subscriberId)
                        });
                    }
                },
                {
                    data: obj => obj.AssociationDate
                }
                //,
                //{
                //    data: obj => RenderGraphImage(), createdCell: (cell, _, row) => {
                //        $('.icon-path', cell).click(() => HandleGraphShortestPath(row, ctsCustId));
                //    }
                //}
            ]
        });
    }

    function AssociationStatus(isActive) {
        //return '<label class="switch-status switch-green"><input type="checkbox" class="switch-input" ' + (isActive ? '' : 'checked') + '><span class="switch-label" data-on="Active" data-off="Removed"></span><span class="switch-handle"></span></label>'

        return '<div class="toggle ' + (isActive ? 'toggle-on' : '') +'" id="switch"><div class="toggle-text-off">Removed</div><div class="glow-comp"></div><div class="toggle-button"></div><div class="toggle-text-on">Active</div></div>';

    }

    function ActiveRemove(obj, fromCTSCustId, fromSubscriberId)
    {
        common.showConfirmation('Do you want to update <b>ASSOCIATION STATUS</b> for <b>' + _username +'</b> and <b>' + obj.Username + '</b> ?',
            function () {
                RemoveAssociation(fromSubscriberId, fromCTSCustId, obj.SubscriberId, obj.CTSCustId, !obj.IsActive);
            });

    }

    function HandleGraphShortestPath(row, ctsCustid) {
        var formGraph = new graph();
        formGraph.GenerateShortestPath(ctsCustid, row.CTSCustId);
        formGraph.OpenModal();
    }

    function ResetForm() {
        assusername.val('');
        asscomment.val('');
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
            complete: function () {
                
            },
            error: function (errMsg) {
                // common.showError(errMsg);
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
                if (response.errorCode == 0) {
                    associatedByAccountTable.find('tbody').addClass('loading');
                    common.showSuccess('Added association successfully!', 2000);
                    divAddAssociation.dialog('close');

                    $('#association-cbb-status').val(0);
                    $("#account-by-account-cbb-association-type").val(3);

                    setTimeout(() => {
                        btnSearch.trigger("click");
                    }, 2000);
                }
                else if (response.errorCode == -2) {
                    common.showError('Username "' + params.Username + '" is invalid.');
                }
                else if (response.errorCode == -1) {
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

    function RemoveAssociation(fromSubscriberId, fromCTSCustId, toSubscriberId, toCTSCustId, isActive) {
        var url = common.modifyRelativeUrl('Account/ActiveRemoveAssociation');
        var params = {};
        params.fromSubscriberId = parseInt(fromSubscriberId);
        params.fromCTSCustId = parseInt(fromCTSCustId);
        params.toSubscriberId = toSubscriberId;
        params.toCTSCustId = toCTSCustId;
        params.Remark = "";
        params.isActive = isActive;

        $.ajax(url, {
            type: 'POST',
            data: params,
            beforeSend: function () {
                
            },
            success: function (response) {
                if (response.errorCode == 0) {
                    
                    common.showSuccess('Updated association successful!', 2000);
                }
                else if (response.errorCode == -1) {
             
                    common.showError('This association is already deactivated!');
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

    this.Initial = function (ctsCustid, username, subscriberId, subscriber) {
        _username = username;

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
            ResetForm();

            divAddAssociation.dialog({
                modal: true,
                width: '600px',
                title: 'ADD ASSOCIATION',
                resizable: true,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            })
        });

        btnSaveAssociation.on('click', function () {
            if (assusername.val() == '') {
                common.showError('Username is required.');
                assusername.focus();
            }
            else if (assusername.val() == username) {
                common.showError('The username must be different from ' + username);
                assusername.focus();
            }
            else {
                AddAssociation(ctsCustid, subscriberId, username, subscriber);
            }
        });

        btnCancelAssociation.on('click', function () {
            divAddAssociation.dialog('close');
        });


    }
}



