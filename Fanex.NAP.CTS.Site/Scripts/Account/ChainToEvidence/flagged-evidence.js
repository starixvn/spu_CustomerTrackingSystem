function flagged_evidence() {
    var tblChainToEvidenceFlagged = $('#tblChainToEvidenceFlagged');
    var btnChainToEvidenceFlagged = $('#btnChainToEvidenceFlagged');        

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
                RenderFlaggedGrid(response);    
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

    function RenderFlaggedGrid(data) {
        tblChainToEvidenceFlagged.DataTable().clear();
        if (data.FlaggedEvidenceList.length > 0) {
            tblChainToEvidenceFlagged.DataTable().rows.add(data.FlaggedEvidenceList).draw();
        }        
        tblChainToEvidenceFlagged.DataTable().columns.adjust();
    }

    this.Initial = function (ctsCustId, username, subscriberId, subscriber, evidenceCodes) {       
        InitialFlaggedGrid(ctsCustId, username);
        GetFlaggedEvidences(ctsCustId, evidenceCodes);

        btnChainToEvidenceFlagged.on('click', function () {
            var formPlaceEvidence = new place_evidence();
            formPlaceEvidence.Initial(ctsCustId, username, subscriberId, subscriber);
        });       
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