function customer_category()
{
    var btnMarkCategory = $('#btnmarkcategory');
    var divMarkCategory = $('#divMarkCategory');

    function BindingMarkCategory() {
        $('#btncancelcategory').on('click', function () {
            divMarkCategory.dialog('close');
        });

        $('#btnsavecategory').on('click', function (e) {
            //AddPlaceEvidence(ctsCustId, username, subscriberId);
        });

        $("#ddlcategory").multiselect({
            header: "Select an option",
            noneSelectedText: "Select an Option",
            selectedList: 1
        });

        $("#ddlsports").multiselect({
            selectedList: 5,
            selectedText: "# of # selected",
            noneSelectedText: "Select options"
        });
    }
    function InitialMarkCategory(ctsCustId) {
        $.ajax(common.modifyRelativeUrl('Account/MarkCatergory'), {
            type: 'POST',
            data: {
                ctsCustId: ctsCustId
            },
            beforeSend: function () {
                common.showLoading();
            },
            success: function (response) {
                divMarkCategory.html(response);
                BindingMarkCategory();
            },
            complete: function () {
                divMarkCategory.dialog({
                    modal: true,
                    width: '600px',
                    title: 'MARK CUSTOMER CATEGORY',
                    resizable: true,
                    draggable: true,
                    close: function () { $(this).dialog('close'); }
                });

                common.closeLoading();
            },
            error: function (errMsg) {
                common.showError(errMsg);
            }
        });
    }

    this.Initial = function (ctsCustid, username) {
        btnMarkCategory.click(function () { InitialMarkCategory(ctsCustid);});
    }
}