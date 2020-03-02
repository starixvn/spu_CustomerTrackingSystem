function account_exceptions() {
    var divException = $('#divAddException');
    var tblChainToEvidenceException = $('#tblChainToEvidenceException');
    var btnAddException = $('#btnaddexceptions');
    var btnSaveException = $('#btnsaveexceptions');
    var btnCancelException = $('#btncancelexceptions');
    var username = $('#txtusername');
    var comment = $('#txtcomment');
    var _ctsCustId = 0;
    var _rootUsername = '';
    var _subscriberId = 0;

    function RenderGrid() {
        tblChainToEvidenceException.DataTable({
            language: {
                zeroRecords: "There's no data"
            },
            info: false,
            ordering: false,
            paging: false,
            searching: false,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0], width: "15%", className: "text-left" },
                { targets: [1], width: "15%", className: "text-left" },
                { targets: [2], width: "15%", className: "text-center" },
                { targets: [3], width: "30%", className: "text-left" },
                { targets: [4], width: "15%", className: "text-center" },
                { targets: [5], width: "10%", className: "text-center" }
            ],
            columns: [
                { data: obj => RenderUsername(obj) },
                { data: obj => obj.LoginId },
                { data: obj => obj.ExceptionDate },
                { data: obj => obj.Comment },
                { data: obj => obj.Creator },
                {
                    data: obj => '<span class="icon-delete" title="Remove Exception"></span>', createdCell: (cell, _, row) => {
                        $('.icon-delete', cell).click(() =>
                        {
                            ConfirmRemoveException(row.ExceptionCTSCustId, row.UserName);
                        });
                    }
                }
            ]
        })
    }

    function RenderUsername(obj) {
        if (obj.UserName != null) {
            var url = common.modifyRelativeUrl('/Account/Index/' + obj.ExceptionCTSCustId);

            return '<a class="info-bng" href="' + url + '">' + obj.UserName + '</a>';
        }

        return null;        
    }

    function ConfirmRemoveException(exceptionCTSCustId, username) {
        var url = common.modifyRelativeUrl('/Account/GetAffectedEvidenceListBetweenCusts');
        var params = {};
        params.FromCTSCustId = _ctsCustId;
        params.ToCTSCustId = exceptionCTSCustId;        
        
        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                var html = 'Account: <b>' + username + '</b><br/>';
                html += 'From: <b>' + _rootUsername + $.parseHTML("&#x27;")[0]['wholeText'] + 's Exception List </b><br/>';
                html += `The following evidence will be affected between 2 account: <br/>`;
                html += '<div class="list-evidence"><ul>';
                for (var i = 0; i < response.length; i++) {
                    html += '<li> - ' + response[i].EvidenceDisplay + '</li>';
                }
                html += '</ul></div>';

                var title = `Remove Exceptions List`;                                

                common.showFormConfirmation(html, title, function () {
                    RemoveException(exceptionCTSCustId);
                });
            }
        });
    }

    function RemoveException(exceptionCTSCustId) {
        var url = common.modifyRelativeUrl('/Account/RemoveException');
        var params = {};
        params.FromCTSCustId = _ctsCustId;
        params.ToCTSCustId = exceptionCTSCustId;
        params.SubscriberId = _subscriberId;

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.errorCode == 0) {
                    common.showSuccess('Remove the exception successful!');
                }
                else {
                    common.showError('Remove the exception fail!');
                }
            },
            complete: function () {
                GetException();
                var accountEvidence = new account_evidence();
                accountEvidence.ReloadAccountEvidence(_ctsCustId, _rootUsername);
            }
        });
    }

    function AddException() {

        if (_rootUsername == username.val().trim()) {
            common.showError('Username/Login ID "' + username.val() + '" is invalid.');

            return;
        }

        var url = common.modifyRelativeUrl('/Account/AddException');
        var params = {};
        params.FromCTSCustId = _ctsCustId;
        params.ToUserName = username.val();
        params.Comment = comment.val();

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.errorCode == 0) {
                    common.showSuccess('Add the exception successful!');
                    divException.dialog('close');
                }
                else if (response.errorCode == 1) {
                    common.showError('Username/Login ID "' + params.ToUserName + '" is invalid.');
                }
                else if (response.errorCode == 1062) {
                    common.showError('Username/Login ID already exists in "' + params.ToUserName + '" exceptions list.');
                }
                else {
                    common.showError('Add the exception fail!');
                }
            },
            complete: function () {
                GetException();
                var accountEvidence = new account_evidence();
                accountEvidence.ReloadAccountEvidence(_ctsCustId, _rootUsername);
            }
        });
    }

    function ResetForm() {
        username.val('');
        comment.val('');
    }

    function GetException() {
        var url = common.modifyRelativeUrl('/Account/GetExceptions');

        $.ajax(url, {
            type: 'POST',
            beforeSend: function () {
                tblChainToEvidenceException.find('tbody').addClass('loading');
            },
            data: {
                ctsCustId: _ctsCustId
            },
            success: function (response) {
                tblChainToEvidenceException.DataTable().clear();
                tblChainToEvidenceException.DataTable().rows.add(response).draw();
                tblChainToEvidenceException.DataTable().columns.adjust();
            },
            complete: function () {
                tblChainToEvidenceException.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        })
    }

    this.Initial = function (ctsCustId, rootUsername, subscriberId) {
        _ctsCustId = ctsCustId;
        _rootUsername = rootUsername;
        _subscriberId = subscriberId;

        RenderGrid();
        GetException();

        btnAddException.on('click', function () {
            ResetForm();

            divException.dialog({
                modal: true,
                width: '600px',               
                title: 'ADD EXCEPTIONS',                
                resizable: true,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            })
        });

        btnSaveException.on('click', function () {
            if (username.val() == '') {
                common.showError('Username/Login ID is require.');
                username.focus();
            } else {
                AddException();
            }            
        });

        btnCancelException.on('click', function () {
            divException.dialog('close');
        });
    }    
}