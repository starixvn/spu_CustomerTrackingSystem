function flagged_evidence() {
    var tblChainToEvidenceFlagged = $('#tblChainToEvidenceFlagged');
    var btnChainToEvidenceFlagged = $('#btnChainToEvidenceFlagged');        
    var divPlaceEvidence = $('#divPlaceEvidence');
    var lblPlaceEvidenceAccount = $('#lblPlaceEvidenceAccount');
    var lblPlaceEvidenceSubscriber = $('#lblPlaceEvidenceSubscriber');
    var ddlPlaceEvidenceListEvidence = $('#ddlPlaceEvidenceListEvidence');
    var btnPlaceEvidenceCancel = $('#btnPlaceEvidenceCancel');
    var btnPlaceEvidenceSubmit = $('#btnPlaceEvidenceSubmit');
    var divPlaceEvidenceDescription = $('#divPlaceEvidenceDescription');
    var txtPlaceEvidenceComment = $('#txtPlaceEvidenceComment');
    var cacheEvidenceCodes = '';

    function InitialFlaggedGrid(ctsCustId, username) {
        tblChainToEvidenceFlagged.DataTable({
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
                { targets: [0], width: "30%", className: "text-left" },
                { targets: [1], width: "20%", className: "text-center" },
                { targets: [2], width: "20%", className: "text-left" },
                { targets: [3], width: "20%", className: "text-left" },
                { targets: [4], width: "20%", className: "text-center" }
            ],
            columns: [
                { data: obj => obj.EvidenceDisplay },
                { data: obj => moment(obj.FlaggedDate).format("MMM DD, YYYY HH:mm:ss") },
                { data: obj => obj.Comment },
                { data: obj => obj.Creator },
                {
                    data: obj => '<span class="icon-delete"></span>', createdCell: (cell, _, row) => {
                        $('.icon-delete', cell).click(() =>
                            common.showConfirmation(`Do you want remove affected evidence ${row.EvidenceDisplay} to ${username}?`, function () {
                                RemoveFlaggedEvidence(row.EvidenceID, ctsCustId)
                            })                            
                        );
                    }
                }
            ],
            createdRow: function (row, obj) {
                $(row).attr('evidence-code', obj.EvidenceCode);
            },
        })
    }   

    function RemoveFlaggedEvidence(evidenceId, ctsCustId) {
        var url = common.modifyRelativeUrl('/Account/RemoveFlaggedEvidence'); 

        $.ajax(url, {
            type: 'POST', 
            beforeSend: function () {
                common.showLoading();
            },
            data: {
                ctsCustId: ctsCustId,
                evidenceId: evidenceId
            },
            success: function (response) {
                common.showSuccess();
            },  
            complete: function () {
                common.closeLoading();
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        })
    }

    function GetFlaggedEvidences(ctsCustId, evidenceCodes) {
        var url = common.modifyRelativeUrl('/Account/GetFlaggedEvidences');

        $.ajax(url, {
            type: 'POST',
            beforeSend: function () {
                tblChainToEvidenceFlagged.find('tbody').addClass('loading');
            },
            data: {
                ctsCustId: ctsCustId,
                evidenceCodes: evidenceCodes
            },
            success: function (response) {                
                tblChainToEvidenceFlagged.DataTable().clear().rows.add(response).draw();                  
                tblChainToEvidenceFlagged.DataTable().columns.adjust();
            },
            complete: function () {
                tblChainToEvidenceFlagged.find('tbody').removeClass('loading');
            },
            error: function (errMsg) {
                console.log(errMsg);
                common.showError(errMsg);
            }
        })
    }

    function ResetPlaceEvidence(username, subscriber) {      
        lblPlaceEvidenceAccount.text(username);
        lblPlaceEvidenceSubscriber.text(subscriber);
        divPlaceEvidenceDescription.text('');
        txtPlaceEvidenceComment.val('');
    }

    function AddPlaceEvidence(ctsCustId, subscriberId) {
        var url = common.modifyRelativeUrl('/Account/PlaceEvidence');
        var params = {};
        params.ctsCustId = ctsCustId;
        params.subscriberId = subscriberId,
        params.evidenceId = ddlPlaceEvidenceListEvidence.find('option:selected').val();
        params.comment = txtPlaceEvidenceComment.val();

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                common.showSuccess('Add the evidence successful!');
                divPlaceEvidence.dialog('close');
            },
            complete: function () {
                GetFlaggedEvidences(ctsCustId, cacheEvidenceCodes);
            },
            error: function (errorMessage) {
                console.log(errorMessage);
                common.showError('Add the evidence fail!');
            }
        })
    }

    this.Initial = function (ctsCustId, username, subscriberId, subscriber, evidenceCodes) {     
        cacheEvidenceCodes = evidenceCodes;

        InitialFlaggedGrid(ctsCustId, username);
        GetFlaggedEvidences(ctsCustId, evidenceCodes);

        btnChainToEvidenceFlagged.on('click', function () {           
            ddlPlaceEvidenceListEvidence.multiselect({
                multiple: false,
                selectedList: 1,
                menuWidth: 300,
                menuHeight: 300,
                create: function () {
                    $('.ui-multiselect-all').parent().parent().remove();
                }
            }).multiselectfilter({
                width: 220,
                placeholder: ''
            });

            ddlPlaceEvidenceListEvidence.bind('change', function (e) {
                var desc = $('#ddlPlaceEvidenceListEvidence :selected').attr('desc');
                if (desc === undefined)
                    desc = '';
                divPlaceEvidenceDescription.html(desc);
            })

            ResetPlaceEvidence(username, subscriber);

            divPlaceEvidence.dialog({
                modal: true,
                width: '650px',
                title: 'ADD EVIDENCE',
                resizable: true,
                draggable: true,
                close: function () { $(this).dialog('close'); }
            })    
        })   

        btnPlaceEvidenceCancel.on('click', function () {
            divPlaceEvidence.dialog('close');
        })

        btnPlaceEvidenceSubmit.on('click', function (e) {
            AddPlaceEvidence(ctsCustId, subscriberId);
        })
    }

    this.Refresh = function (evidenceCode, checked) {
        tblChainToEvidenceFlagged.find('tr').each(function () {
            if ($(this).attr('evidence-code') == evidenceCode) {
                if (checked) {
                    $(this).removeClass('hide');
                } else {
                    $(this).addClass('hide');
                }
            }
        }) 
    }

    this.RefreshAll = function (checked) {
        tblChainToEvidenceFlagged.find('tr').each(function () {
            if (checked) {
                $(this).removeClass('hide');
            } else {
                $(this).addClass('hide');
            }
        })
    }
}