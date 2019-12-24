function AccountDetail() {
    var ctsCustId = $("#hdCTSCustId").val();
    var username = $("#hdUsername").val();  

    function LoadAssociationByAccount() {
        $.ajax(common.modifyRelativeUrl('/Account/AssociatedByAccount'), {
            type: 'POST',
            data: {
                username: username
            },
            success: function (response) {
                $("#associated_account").html(response);
            },
            complete: function () {
                var isDirect = $("#radio03-01").is(':checked');
                var accountStatus = $('#account-by-account-cbb-status').val().join(',');
                var associationType = $("#account-by-account-cbb-association-type").val();

                var associationByAccount = new account_by_account();
                associationByAccount.Initial(ctsCustId, accountStatus, associationType, isDirect);
            },
            error: function (errMsg) {
                common.showError(errMsg);
            }
        });
    }

    function CollapseExpandItem() {
        $("#account-detail-container .report-item h3").on('click', function () {
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

    function CollapseAll() {
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