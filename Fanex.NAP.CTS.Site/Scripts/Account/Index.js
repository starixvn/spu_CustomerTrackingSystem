function AccountDetail() {
    function LoadAssociationByAccount() {
        $.ajax(common.modifyRelativeUrl('/Account/GetAssociatedAccountByAccount'), {
            type: 'POST',
            success: function (response) {
                $("#associated_account").html(response)
            },
            error: function (errMsg) {

            }
        });
    }

    function CollapseExpandItem() {
        $("#account-detail-container .report-item h3").on('click', function ()
        {
            CollapseAll();

            var $this = $(this).next();

            if ($this.hasClass('hide')) {
                $this.removeClass('hide');
                $this.parent().addClass("expanded");
            }            
        });
    }

    function CollapseExpandAll() {
        $("#account-detail-header").on('click', function () {
            var $this = $(this).find("span");

            if ($this.hasClass("expand-all")) {
                $this.removeClass("expand-all").addClass("collapse-all");
                ExpandAll();
            }
            else {
                $this.removeClass("collapse-all").addClass("expand-all");

                CollapseAll();
            }
        });
    }

    function ExpandAll() {
        $("#account-detail-container .report-item .report-cont").removeClass('hide');
        $("#account-detail-container .report-item").addClass('expanded');
    }

    function CollapseAll()
    {
        $("#account-detail-container .report-item .report-cont").addClass('hide');
        $("#account-detail-container .report-item").removeClass('expanded');
    }


    function Initial() {
        CollapseExpandAll();
        CollapseExpandItem();
        LoadAssociationByAccount();
        
    }

    $(document).ready(function () {
        Initial();
    });
}
$(AccountDetail);