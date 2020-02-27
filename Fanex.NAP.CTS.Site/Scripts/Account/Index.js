function AccountDetail() {
    var username = $("#hdUsername").val();
    var custId = $("#hdCustId").val();
    var ctsCustId = $("#hdCTSCustId").val();
    var subscriberId = $("#hdSubcriberId").val();
    var subscriber = $("#hdSubscriber").val();

    var lnkAssociation = $("#lnkAssociation");
    var lnkEvidence = $("#lnkEvidence");

    var totalAssAccount = 0;
    var totalEvidence = 0

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
                var accountEvidence = new account_evidence();
                accountEvidence.Initial(ctsCustId, username, subscriberId, subscriber);

                var exceptions = new account_exceptions();
                exceptions.Initial(ctsCustId, username, subscriberId);
            },
            error: function (errMsg) {
                common.showError(errMsg);
            }
        });
    }

    function GetCustomerTotalAssociation() {
        $.ajax(common.modifyRelativeUrl('/Account/GetCustomerTotalAssociationByCTSCustId'), {
            type: 'POST',
            data: {
                ctsCustId: ctsCustId                
            },
            success: function (response) {

                totalAssAccount = response.TotalAccounts;

                $('#totalAssAccount').html(response.TotalAccounts);
                $('#totalAssDevice').html(response.TotalDevices);
                $('#totalAssIP').html(response.TotalIps);
            },
            complete: function () {
                GetCustomerEvidences();
            },
            error: function (errMsg) {
                common.showError(errMsg);
            }
        });
    }

    function GetCustomerEvidences() {
        $.ajax(common.modifyRelativeUrl('/Account/GetCustomerEvidenceByCTSCustId'), {
            type: 'POST',
            data: {
                ctsCustId: ctsCustId
            },
            success: function (response) {
                $('#evidences').html(response.evidences);
                totalEvidence = response.numberOfItems;

                if (response.numberOfItems == 0) {
                    $('#lnkEvidence').contents().unwrap();
                }
            },
            complete: function () {
                if (totalAssAccount > 0 && totalEvidence > 0) {
                    $('#affected').removeClass('hide');
                }                
            },
            error: function (errMsg) {
                common.showError(errMsg);
            }
        });
    }

    function GetCustomerFlaggInfo() {
        $.ajax(common.modifyRelativeUrl('/Account/GetCustomerFlagInfo'), {
            type: 'POST',
            data: {
                ctsCustId: ctsCustId,
                subscriberId: subscriberId
            },
            success: function (response)
            {
                if (response.Flagged != 0)
                {
                    if (response.Flagged == 2) {
                        $('#flagged100').removeClass('hide');
                       // $('#flagged').remove();
                    }
                    else {
                        $('#flagged').removeClass('hide');
                        //$('#flagged100').remove();
                    }
                }                
            },
            complete: function () {

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
        GetCustomerTotalAssociation();
        GetCustomerFlaggInfo();
        LoadAssociationByAccount();
        LoadChainToEvidence();

        lnkAssociation.on('click', function () {
            $("#header_associated_account").trigger("click");
        });

        lnkEvidence.on('click', function () {
            $("#header_chains_evidence").trigger("click");
        });
    }

    $(document).ready(function () {
        Initial();
    });
}
$(AccountDetail);