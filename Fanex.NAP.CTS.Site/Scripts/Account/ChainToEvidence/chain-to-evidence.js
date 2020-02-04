function chain_to_evidence() {
    var tblChainToEvidenceFlagged = $('#tblChainToEvidenceFlagged');
    var btnChainToEvidenceFlagged = $('#btnChainToEvidenceFlagged');
    
    var cbbChainToEvidenceListEvidence = $('#cbbChainToEvidenceListEvidence');

    function InitialFlaggedGrid(custId) {
        tblChainToEvidenceFlagged.DataTable({
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
                    data: obj => RenderRemoveFlaggedEvidence(), createdCell: (cell, _, row) => {
                        $('.icon-delete', cell).click(() => RemoveFlaggedEvidence(row.EvidenceID, custId));
                    }
                }
            ]
        })
    }

    function RenderRemoveFlaggedEvidence() {
        return '<span class="icon-delete"></span>';
    }

    function RemoveFlaggedEvidence(evidenceId, custId) {

    }

    function GetChainToEvidence(custId) {
        var url = common.modifyRelativeUrl('/Account/GetChainToEvidence');

        $.ajax(url, {
            type: 'POST',
            beforeSend: function () {
                tblChainToEvidenceFlagged.find('tbody').addClass('loading');
            },
            data: {
                custId: custId
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

    this.Initial = function (custId) {
        cbbChainToEvidenceListEvidence.multiselect({
            menuWidth: 330,
            menuHeight: 330,
            selectedText: "# of # selected",
            noneSelectedText: "Select options"
        }).multiselectfilter({
            width: 250,
            placeholder: ''
        });

        InitialFlaggedGrid(custId);
        GetChainToEvidence(custId);

        btnChainToEvidenceFlagged.on('click', function () {
            var formPlaceEvidence = new place_evidence();
            formPlaceEvidence.Initial();
        });

       
    }
}