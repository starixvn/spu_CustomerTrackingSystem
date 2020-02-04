function place_evidence() {
    var divPlaceEvidence = $('#divPlaceEvidence');
    var lblPlaceEvidenceAccount = $('#lblPlaceEvidenceAccount');
    var lblPlaceEvidenceSubscriber = $('#lblPlaceEvidenceSubscriber');
    var ddlPlaceEvidenceListEvidence = $('#ddlPlaceEvidenceListEvidence');
    var btnPlaceEvidenceCancel = $('#btnPlaceEvidenceCancel');
    var btnPlaceEvidenceSubmit = $('#btnPlaceEvidenceSubmit');
    var divPlaceEvidenceDescription = $('#divPlaceEvidenceDescription');
    var txtPlaceEvidenceComment = $('#txtPlaceEvidenceComment');

    function Reset(username, subscriber) {
        btnPlaceEvidenceCancel.prop('onclick', null).off('click');
        btnPlaceEvidenceSubmit.prop('onclick', null).off('click');
        ddlPlaceEvidenceListEvidence.prop('change', null).off('change');

        lblPlaceEvidenceAccount.text(username);
        lblPlaceEvidenceSubscriber.text(subscriber);
        divPlaceEvidenceDescription.text('');
        txtPlaceEvidenceComment.val('');
    }

    function Summit(ctsCustId, subscriberId) {
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
            error: function (errorMessage) {
                console.log(errorMessage);
                common.showError('Add the evidence fail!');
            }
        })
    }

    this.Initial = function (ctsCustId, username, subscriberId, subscriber) {
        Reset(username, subscriber);

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

        btnPlaceEvidenceSubmit.on('click', function (e) {
            Summit(ctsCustId, subscriberId);            
        })
    }
}