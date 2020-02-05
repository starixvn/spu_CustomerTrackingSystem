function AccountDetail() {
    var username = $("#hdUsername").val();  
    var custId = $("#hdCustId").val();
    var ctsCustId = $("#hdCTSCustId").val();
    var custSubId = $("#hdCustSubId").val();
    var subscriber = $("#hdSubscriber").val();

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
                associationByAccount.Initial(custId, ctsCustId, username, accountStatus, associationType, isDirect);
            },
            error: function (errMsg) {
                common.showError(errMsg);
            }
        });
    }

    function LoadChainToEvidence() {
        $.ajax(common.modifyRelativeUrl('/Account/ChainToEvidence'), {
            type: 'POST',
            success: function (response) {
                $("#chains_evidence").html(response);
            },
            complete: function () {
                var flaggedEvidence = new flagged_evidence();
                flaggedEvidence.Initial(ctsCustId, username, custSubId, subscriber);

                var affectedRetractEvidence = new affected_retract_evidence();
                affectedRetractEvidence.Initial(ctsCustId);

                var exceptions = new account_exceptions();
                exceptions.Initial(ctsCustId);
            },
            error: function (errMsg) {
                common.showError(errMsg);
            }
        });
    }

    function CollapseExpandItem() {
        $("#account-detail-container .report-item h3").on('click', function () {
            
            var $this = $(this).next();

            if ($this.hasClass('hide')) {
                CollapseAll();
                $this.removeClass('hide');
                $this.parent().addClass("expanded");
            }
            else {
                $this.parent().removeClass('expanded');
                $this.addClass("hide");
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
        LoadChainToEvidence();
    }

    $(document).ready(function () {
        Initial();
    });
}
$(AccountDetail);