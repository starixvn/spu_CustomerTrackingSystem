function account_exceptions() {
    var divException = $('#divAddException');
    var tblChainToEvidenceException = $('#tblChainToEvidenceException');
    var btnAddException = $('#btnaddexceptions');
    var btnSaveException = $('#btnsaveexceptions');
    var btnCancelException = $('#btncancelexceptions');
    var username = $('#txtusername');
    var comment = $('#txtcomment');
    var _ctsCustId = 0;
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
                { targets: [0], width: "15%", className: "text-center" },
                { targets: [1], width: "15%", className: "text-center" },
                { targets: [2], width: "15%", className: "text-center" },
                { targets: [3], width: "30%", className: "text-left" },
                { targets: [4], width: "15%", className: "text-center" },
                { targets: [5], width: "10%", className: "text-center" }
            ],
            columns: [
                { data: obj => obj.UserName },
                { data: obj => obj.LoginId },
                { data: obj => moment(obj.ExceptionDate).format("MMM DD, YYYY HH:mm:ss") },
                { data: obj => obj.Comment },
                { data: obj => obj.Creator },
                {
                    data: obj => '<span class="icon-delete"></span>', createdCell: (cell, _, row) => {
                        $('.icon-delete', cell).click(() =>
                        {
                            ConfirmRemoveException(row.ExceptionCTSCustId, row.UserName);
                        });
                    }
                }
            ]
        })
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
                var html = `The following evidence will be affected between 2 accounts ${username}? <br/>`;
                for (var i = 0; i < response.length; i++) {
                    html += response[i].EvidenceDisplay + '<br/>';
                }
                common.showConfirmation(html, function () {
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
            }
        });
    }

    function AddException() {
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
                    common.showError('User "' + username.val() + '" dose not exist.');
                }
                else {
                    common.showError('Add the exception fail!');
                }
            },
            complete: function () {
                GetException();
            }
        });
    }

    function ResetForm() {
        username.val('');
        comment.val('');
    }

    this.Initial = function (ctsCustId) {
        _ctsCustId = ctsCustId;
        RenderGrid();
        GetException();

        btnAddException.on('click', function () {
            ResetForm();

            divException.dialog({
                modal: true,
                width: '650px',
                title: 'ADD EXCEPTIONS',
                resizable: true,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            })
        });

        btnSaveException.on('click', function () {
            AddException();
        });

        btnCancelException.on('click', function () {
            divException.dialog('close');
        });
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
}