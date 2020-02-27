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
                    $('span.icon-Affected').removeClass('hide');
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
                        $('span.icon-Flagged').removeClass('hide');                       
                    }
                    else {
                        $('span.icon-Flagged-With-Evidence').removeClass('hide');
                    }
                }                
            },
            complete: function () {
                $('span.loading').remove(); 
            },
            error: function (errMsg) {
                common.showError(errMsg);
            }
        });
    }

    function GetCustomerGeneralStatus() {
        $.ajax(common.modifyRelativeUrl('/Account/GetCustomerGeneralStatus'), {
            type: 'POST',
            data: {
                custId: custId               
            },
            success: function (response) {
                if (response != null) {

                    $('#accountStatus').html(response.accountStatus);

                    if (response.customerStatus.ProblemAccountWithBetLimit) {
                        $('span.icon-P-with-Bet-Limit').removeClass('hide');
                    }

                    if (response.customerStatus.ProblemAccountWithoutBetLimit) {
                        $('span.icon-P-without-Bet-Limit').removeClass('hide');
                    }

                    if (response.customerStatus.BetRestrictedAccount) {
                        $('span.icon-Bet-Restrict-Account').removeClass('hide');
                    }

                    if (response.customerStatus.BetRestrictedCurrency) {
                        $('span.icon-Bet-Restricted-Currency').removeClass('hide');
                    }

                    if (response.customerStatus.BetRestrictedSite) {
                        $('span.icon-Bet-Restrict-Site').removeClass('hide');
                    }

                    if (response.customerStatus.BetRestrictedSiteAndCurrency) {
                        $('span.icon-Bet-Restricted-SiteandCurrency').removeClass('hide');
                    }

                    if (response.customerStatus.SmartPunter) {
                        $('span.icon-Smart-Punter-Account').removeClass('hide');
                    }
                    
                    if (response.customerStatus.Danger == 0) {
                        $("#dangerousOriginal").html("None");
                    }
                    else {
                        $("#dangerousOriginal").html(response.customerStatus.Danger);
                        $("#dangerousOriginal").addClass("dangerous-level-" + response.customerStatus.Danger);
                    }

                    if (response.customerStatus.Danger2 == 0) {
                        $("#dangerouseABI").html("None");
                    }
                    else {
                        $("#dangerouseABI").html(response.customerStatus.Danger2);
                        $("#dangerouseABI").addClass("dangerous-level-" + response.customerStatus.Danger2);
                    }

                    if (response.customerStatus.Danger3 == 0) {
                        $("#dangerouseOtherSports").html("None");
                    }
                    else {
                        $("#dangerouseOtherSports").html(response.customerStatus.Danger3);
                        $("#dangerouseOtherSports").addClass("dangerous-level-" + response.customerStatus.Danger3);
                    }

                    if (response.customerStatus.Abnormal == 0) {
                        $("#abnormal").html("None");
                    }
                    else {
                        $("#abnormal").html(response.customerStatus.Abnormal);
                        $("#abnormal").addClass("abnormal-level-" + response.customerStatus.Abnormal);
                    }

                    $('#autoSuspendSoccer').html(response.customerStatus.Autosuspend ? 'Yes' : 'No');
                    $('#autoSuspendOtherSport').html(response.customerStatus.Autosuspend2 ? 'Yes' : 'No');

                }
            },
            complete: function () {
                                 
            },
            error: function (errMsg) {
                common.showError(errMsg);
            }
        });
    }

    function GetCustomerRobotUserStatus() {
        $.ajax(common.modifyRelativeUrl('/Account/GetCustomerRobotUserStatus'), {
            type: 'POST',
            data: {
                custId: custId
            },
            success: function (response) {
                if (response != null) {
                    if (response.IsRobotUser) {
                        $('span.icon-Robot-User').removeClass('hide');
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

    function GetCustomerEsportAutoSuspendedStatus() {
        $.ajax(common.modifyRelativeUrl('/Account/GetCustomerEsportAutoSuspendedStatus'), {
            type: 'POST',
            data: {
                custId: custId
            },
            success: function (response) {
                if (response != null) {
                    $('#autoSuspendESport').html(response.EsportAutoSuspendStatus ? 'Yes' : 'No');
                }
            },
            complete: function () {

            },
            error: function (errMsg) {
                common.showError(errMsg);
            }
        });
    }


    function GetCustomerProfileFraudInfo() {
        $.ajax(common.modifyRelativeUrl('/Account/GetCustomerProfileFraudInfo'), {
            type: 'POST',
            data: {
                custId: custId
            },
            success: function (response) {
                if (response.suspiciouseCustomer != null) {                  

                    if (response.suspiciouseCustomer.IsDelinquent) {
                        $('span.icon-delinquency').removeClass('hide');
                    }

                    if (response.suspiciouseCustomer.IsHedge) {
                        $('span.icon-Hedge-Account').removeClass('hide');
                    }

                    if (response.suspiciouseCustomer.IsDirty) {
                        $('span.icon-dirty').removeClass('hide');
                    }

                    $('div.classification > img.icon-loading').remove();
                }
                else {
                    $('div.classification').html("<span>None</span>");
                }

                if (response.fraudCases != null) {
                    $('#fraudcase').html("<span>" + response.fraudCases + "</span>");
                }
                else {
                    $('#fraudcase').html("<span>None</span>");
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
        GetCustomerRobotUserStatus();
        GetCustomerGeneralStatus();
        GetCustomerTotalAssociation();
        GetCustomerFlaggInfo();
        GetCustomerEsportAutoSuspendedStatus();
        GetCustomerProfileFraudInfo();

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