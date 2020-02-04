function place_evidence() {
    var divPlaceEvidence = $('#divPlaceEvidence');
    var ddlPlaceEvidenceListEvidence = $('#ddlPlaceEvidenceListEvidence');

    this.Initial = function (custId) {
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
    }
}