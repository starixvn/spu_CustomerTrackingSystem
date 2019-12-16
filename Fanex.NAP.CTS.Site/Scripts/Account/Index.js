function AccountDetail() {
    function LoadAssociationByAccount() {
        $.ajax("GetAssociatedAccountByAccount", {
            type: 'POST',
            success: function (response) {
                $("#associated_account").html(response)
            },
            error: function (errMsg) {

            }
        });
    }

    function CollapseExpand() {
        $("#account-detail-container .report-item h3").on('click', function ()
        {
            CollapseAll();

            var $this = $(this).next();

            if ($this.hasClass('hide')) {
                $this.removeClass('hide');
            }
            else {
                $this.addClass('hide');
            }
        });
    }

    function CollapseAll()
    {
        $("#account-detail-container .report-item .report-cont").addClass('hide');
    }

    function Initial() {
        CollapseExpand();
        LoadAssociationByAccount();
    }

    $(document).ready(function () {
        Initial();
    });
}
$(AccountDetail);