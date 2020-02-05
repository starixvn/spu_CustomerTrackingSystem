function account_exceptions()
{
    var divException = $('#divAddException');
    var tblChainToEvidenceException = $('#tblChainToEvidenceException');
    var btnAddException = $('#btnaddexceptions');
    var btnSaveException = $('#btnsaveexceptions');
    var btnCancelException = $('#btncancelexceptions');

    function RenderGrid() {
        tblChainToEvidenceException.DataTable({
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
            paging: false,
            searching: false,
            scrollX: false,
            scrollCollapse: false,
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0], width: "10%", className: "text-left" },
                { targets: [1], width: "10%", className: "text-center" },
                { targets: [2], width: "20%", className: "text-left" },
                { targets: [3], width: "30%", className: "text-left" },
                { targets: [4], width: "20%", className: "text-center" },
                { targets: [4], width: "10%", className: "text-center" }
            ],
            columns: [
                { data: obj => obj.UserName },
                { data: obj => obj.LoginId },
                { data: obj => moment(obj.ExceptionDate).format("MMM DD, YYYY HH:mm:ss") },
                { data: obj => obj.Comment },
                { data: obj => obj.Creator },
                {
                    data: obj => '<span class="icon-delete"></span>', createdCell: (cell, _, row) => {
                        $('.icon-delete', cell).click(() => RemoveException());
                    }
                }
            ]
        })
    }

    function RemoveException() {

    }

    function AddException(ctsCustId) {
        var url = common.modifyRelativeUrl('/Account/AddException');
        var params = {};
        params.UserId = ctsCustId;        

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                if (response.errorCode == 0) {
                    common.showSuccess('Add the exception successful!');
                    divPlaceEvidence.dialog('close');
                }
                else {
                    common.showError('Add the exception fail!');
                }                
            },
            complete: function () {
                GetException(ctsCustId);
            }           
        })
    }

    this.Initial = function(ctsCustId) {
        RenderGrid();
        GetException(ctsCustId);

        btnAddException.on('click', function () {           
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
            AddException(ctsCustId);
        });

        btnCancelException.on('click', function () {
            divException.dialog('close');
        });
    }

    function GetException(ctsCustId) {
        var url = common.modifyRelativeUrl('/Account/GetExceptions');

        $.ajax(url, {
            type: 'POST',
            beforeSend: function () {
                tblChainToEvidenceException.find('tbody').addClass('loading');
            },
            data: { 
                ctsCustId: ctsCustId
            },
            success: function (response) {
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