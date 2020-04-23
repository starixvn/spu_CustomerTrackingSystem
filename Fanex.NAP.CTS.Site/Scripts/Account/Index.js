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
        $.ajax(common.modifyRelativeUrl('Account/AssociatedByAccount'), {
            type: 'POST',
            data: {
                username: username
            },
            success: function(response) {
                $("#associated_account").html(response);
            },
            complete: function() {
                var associationByAccount = new account_by_account();
                associationByAccount.Initial(ctsCustId, username);
            },
            error: function(errMsg) {
               // common.showError(errMsg);
            }
        });
    }

    function LoadChainToEvidence() {
        $.ajax(common.modifyRelativeUrl('Account/ChainToEvidence'), {
            type: 'POST',
            success: function(response) {
                $("#chains_evidence").html(response);
            },
            complete: function() {
                var accountEvidence = new account_evidence();
                accountEvidence.Initial(ctsCustId, username, subscriberId, subscriber);

                var exceptions = new account_exceptions();
                exceptions.Initial(ctsCustId, username, subscriberId);
            },
            error: function(errMsg) {
               // common.showError(errMsg);
            }
        });
    }

    function LoadTransactionHistory() {
        $.ajax(common.modifyRelativeUrl('Account/TransactionHistory'), {
            type: 'POST',
            success: function (response) {
                $("#trans_history").html(response);
            },
            complete: function () {
                var transHistory = new transaction_history();
                transHistory.Initial(ctsCustId, subscriberId);
            },
            error: function (errMsg) {
                common.showError(errMsg);
            }
        });
    }

    function LoadCustomerCategory() {
        $.ajax(common.modifyRelativeUrl('Account/CustomerCategory'), {
            type: 'POST',            
            success: function (response) {
                $("#customer_category").html(response);
            },
            complete: function () {
                var customerCategory = new customer_category();
                customerCategory.Initial(ctsCustId, subscriberId, username);
            },
            error: function (errMsg) {
                common.showError(errMsg);
            }
        });
    }

    function GetCustomerTotalAssociation() {
        $.ajax(common.modifyRelativeUrl('Account/GetCustomerTotalAssociationByCTSCustId'), {
            type: 'POST',
            data: {
                ctsCustId: ctsCustId
            },
            success: function(response) {

                totalAssAccount = response.TotalAccounts;

                $('#totalAssAccount').html(response.TotalAccounts);
                $('#totalAssDevice').html(response.TotalDevices);
                $('#totalAssIP').html(response.TotalIps);
            },
            complete: function () {
                GetCustomerEvidences();
            },
            error: function(errMsg) {
               // common.showError(errMsg);
            }
        });
    }

    function GetCustomerEvidences() {
        $.ajax(common.modifyRelativeUrl('Account/GetCustomerEvidenceByCTSCustId'), {
            type: 'POST',
            data: {
                ctsCustId: ctsCustId
            },
            success: function(response) {
                $('#evidences').html(response.evidences);
                totalEvidence = response.numberOfItems;

                if (response.numberOfItems == 0) {
                    $('#lnkEvidence').contents().unwrap();
                }
            },
            complete: function() {
                if (totalAssAccount > 0 && totalEvidence > 0) {
                    $('span.icon-Affected').removeClass('hide');
                }

                LoadAssociationByAccount();
                LoadChainToEvidence();
            },
            error: function(errMsg) {
               // common.showError(errMsg);
            }
        });
    }

    function BetLimitAndAccountStatusIcon(response) {
        $('#accountStatus').html(response.StatusLabel);

        if (response.GeneralStatusInfo.ProblemAccountWithBetLimit) {
            $('span.icon-P-with-Bet-Limit').removeClass('hide');
        }

        if (response.GeneralStatusInfo.ProblemAccountWithoutBetLimit) {
            $('span.icon-P-without-Bet-Limit').removeClass('hide');
        }
    }

    function BetRestrictAndSmartPunterIcon(response) {
        if (response.GeneralStatusInfo.BetRestrictedAccount) {
            $('span.icon-Bet-Restrict-Account').removeClass('hide');
        }

        if (response.GeneralStatusInfo.BetRestrictedCurrency) {
            $('span.icon-Bet-Restricted-Currency').removeClass('hide');
        }

        if (response.GeneralStatusInfo.BetRestrictedSite) {
            $('span.icon-Bet-Restrict-Site').removeClass('hide');
        }

        if (response.GeneralStatusInfo.BetRestrictedSiteAndCurrency) {
            $('span.icon-Bet-Restricted-SiteandCurrency').removeClass('hide');
        }

        if (response.GeneralStatusInfo.SmartPunter) {
            $('span.icon-Smart-Punter-Account').removeClass('hide');
        }
    }

    function DangerouseIcon(response) {
        if (response.GeneralStatusInfo.Danger == 0) {
            $("#dangerousOriginal").html("None");
        }
        else {
            $("#dangerousOriginal").html(response.GeneralStatusInfo.Danger);
            $("#dangerousOriginal").addClass("dangerous-level-" + response.GeneralStatusInfo.Danger);
        }

        if (response.GeneralStatusInfo.Danger2 == 0) {
            $("#dangerouseABI").html("None");
        }
        else {
            $("#dangerouseABI").html(response.GeneralStatusInfo.Danger2);
            $("#dangerouseABI").addClass("dangerous-level-" + response.GeneralStatusInfo.Danger2);
        }

        if (response.GeneralStatusInfo.Danger3 == 0) {
            $("#dangerouseOtherSports").html("None");
        }
        else {
            $("#dangerouseOtherSports").html(response.GeneralStatusInfo.Danger3);
            $("#dangerouseOtherSports").addClass("dangerous-level-" + response.GeneralStatusInfo.Danger3);
        }
    }

    function ClassificationIcon(response) {
        if (response.SuspiciousInfo != null && (response.SuspiciousInfo.IsDelinquent || response.SuspiciousInfo.IsHedge || response.SuspiciousInfo.IsDirty)) {

            if (response.SuspiciousInfo.IsDelinquent) {
                $('span.icon-delinquency').removeClass('hide');
            }

            if (response.SuspiciousInfo.IsHedge) {
                $('span.icon-Hedge-Account').removeClass('hide');
            }

            if (response.SuspiciousInfo.IsDirty) {
                $('span.icon-dirty').removeClass('hide');
            }

            $('div.classification > img.icon-loading').remove();
        }
        else {
            $('div.classification').html("<span>None</span>");
        }
    }

    function FrauseCaseList(response) {
        if (response.FraudCases != '') {
            $('#fraudcase').html("<span>" + response.FraudCases + "</span>");
        }
        else {
            $('#fraudcase').html("<span>None</span>");
        }
    }

    function GetAccountProfile() {
        $.ajax(common.modifyRelativeUrl('Account/GetAccountProfile'), {
            type: 'POST',
            data: {
                custId: custId,
                ctsCustId: ctsCustId,
                subscriberId: subscriberId
            },
            success: function (response) {
                HandleCustomerStatus(response);
                ClassificationIcon(response);
                FrauseCaseList(response);
                HandleRobotUserStatus(response.RobotUserStatus);
                HandleFlagInfo(response.FlagInfo);
                HandleEsportAutoSuspendedStatus(response.EsportAutoSuspendStatus);
            },
            complete: function () {

            },
            error: function (errMsg) {
               // common.showError(errMsg);
            }
        });
    }

    function CollapseExpandItem() {
        $("#account-detail-container .report-item h3").on('click', function() {

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
        $("#account-detail-header").on('click', function() {
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

    function HandleCustomerStatus(data) {
        if (data != null) {
            BetLimitAndAccountStatusIcon(data);
            BetRestrictAndSmartPunterIcon(data);
            DangerouseIcon(data);

            if (data.GeneralStatusInfo.Abnormal == 0) {
                $("#abnormal").html("None");
            }
            else {
                $("#abnormal").html(data.GeneralStatusInfo.Abnormal);
                $("#abnormal").addClass("abnormal-level-" + data.GeneralStatusInfo.Abnormal);
            }

            $('#autoSuspendSoccer').html(data.GeneralStatusInfo.Autosuspend ? 'Yes' : 'No');
            $('#autoSuspendOtherSport').html(data.GeneralStatusInfo.Autosuspend2 ? 'Yes' : 'No');
        }
    }

    function HandleRobotUserStatus(data) {
        if (data != null) {
            if (data.IsRobotUser) {
                $('span.icon-Robot-User').removeClass('hide');
            }
        }
    }

    function HandleFlagInfo(data) {
        if (data.Flagged != 0) {
            if (data.Flagged == 2) {
                $('span.icon-Flagged').removeClass('hide');
            }
            else {
                $('span.icon-Flagged-With-Evidence').removeClass('hide');
            }
        }

        $('span.loading').remove();
    }

    function HandleEsportAutoSuspendedStatus(data) {
        if (data != null) {
            $('#autoSuspendESport').html(data.EsportAutoSuspendStatus ? 'Yes' : 'No');
        }
    }

    function Initial() {

        CollapseExpandAll();
        CollapseExpandItem();
        GetAccountProfile();
        GetCustomerTotalAssociation();
        LoadTransactionHistory();
        LoadCustomerCategory();

        lnkAssociation.on('click', function() {
            $("#header_associated_account").trigger("click");
        });

        lnkEvidence.on('click', function() {
            $("#header_chains_evidence").trigger("click");
        });
    }

    $(document).ready(function() {
        Initial();
    });
}
$(AccountDetail);