function place_evidence() {
    var divPlaceEvidence = $('#divPlaceEvidence');
    var ddlPlaceEvidenceListEvidence = $('#ddlPlaceEvidenceListEvidence');
    var btnPlaceEvidenceCancel = $('#btnPlaceEvidenceCancel');
    var btnPlaceEvidenceSubmit = $('#btnPlaceEvidenceSubmit');
    var txtPlaceEvidenceComment = $('#txtPlaceEvidenceComment');
  
    function Summit(custId, subscriberId) {
        var url = common.modifyRelativeUrl('/Account/PlaceEvidence');
        var params = {};
        params.custId = 1;
        params.subscriberId = 1,
        params.evidenceId = ddlPlaceEvidenceListEvidence.find('option:selected').val();
        params.comment = txtPlaceEvidenceComment.val();

        $.ajax(url, {
            type: 'POST',
            data: params,
            success: function (response) {
                common.showSuccess('Add the evidence successful!');
                divPlaceEvidence.dialog('close');
            },
            error: function (errorMessage) {
                console.log(errorMessage);
                common.showError('Add the evidence fail!');
            }
        })
    }

    this.Initial = function (custId, subscriberId) {
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

        divPlaceEvidence.dialog({
            modal: true,
            width: '650px',
            title: 'ADD EVIDENCE',
            resizable: true,
            draggable: true,
            close: function () { $(this).dialog('close'); }
        })

        btnPlaceEvidenceCancel.on('click', function () {
            divPlaceEvidence.dialog('close');
        })

        btnPlaceEvidenceSubmit.on('click', function () {
            Summit(custId, subscriberId);
        })
    }
}