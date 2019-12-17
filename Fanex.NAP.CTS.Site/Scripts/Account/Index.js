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
            RefreshCollapseExpand();

            var $this = $(this).next();

            if ($this.hasClass('hide')) {
                $this.removeClass('hide');
                $this.parent().addClass("expanded");
            }            
        });
    }

    function RefreshCollapseExpand()
    {
        $("#account-detail-container .report-item .report-cont").addClass('hide');
        $("#account-detail-container .report-item").removeClass('expanded');
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