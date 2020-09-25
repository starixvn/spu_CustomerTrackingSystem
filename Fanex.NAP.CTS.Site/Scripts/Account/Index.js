function AccountDetail() {
    var username = $("#hdUsername").val();
    var custId = $("#hdCustId").val();
    var ctsCustId = $("#hdCTSCustId").val();
    var subscriberId = $("#hdSubscriberId").val();
    var subscriber = $("#hdSubscriber").val();   
    var isHostPermission = $("#hdIsHostPermission").attr("IsHostPermission");
    var isBettingStatisticPermission = $("#hdIsBettingStatisticPermission").attr("IsBettingStatisticPermission");
    var currentUserId = $("#hdCurrentUserId").val();
    var isPreviousStatisticOpen = false;

    let domElements = {        
        tabAssociatedAccount: $("#associated_account"),
        tabChainEvidence: $("#chains_evidence"),
        tabCustomerCategory: $("#customer_category"),
        tabTransactionHistory: $("#trans_history"),        
        spanTotalAssociated: $('#spanTotalAssociated'),
        spanTotalEvidences: $('#spanTotalEvidences'),
        spanDangerousOriginal: $("#dangerousOriginal"),
        spanDangerouseABI: $("#dangerouseABI"),
        spanDangerouseOtherSports: $("#dangerouseOtherSports"),
        spanFraudcase: $('#fraudcase'),
        spanAccountStatus: $('#spanAccountStatus'),        
        spanAutoSuspendSoccer: $('#autoSuspendSoccer'),
        spanAutoSuspendOtherSport: $('#autoSuspendOtherSport'),
        spanAutoSuspendESport: $('#autoSuspendESport'),
        associatedAccountTab: $("#associated_account-tab"),
        evidenceTab: $("#evidence-tab"),
        statisticTitle: $("#statistic-title"),
        statisticIcon: $("#statistic-title .icon"),
        statisticDetail: $("#statistic-detail"),
        wrapperStatistic: $("#wrapper-statistic")
    };

    function LoadAssociationByAccount() {
        $.ajax(common.modifyRelativeUrl('Account/AssociatedByAccount'), {
            type: 'POST',
            data: {
                username: username
            },
            success: function (response) {
                domElements.tabAssociatedAccount.html(response);
            },
            complete: function () {
                var associationByAccount = new account_by_account();
                associationByAccount.Initial(ctsCustId, username, subscriberId, subscriber, isHostPermission);
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function LoadChainToEvidence() {
        $.ajax(common.modifyRelativeUrl('Account/ChainToEvidence'), {
            type: 'POST',
            success: function (response) {
                domElements.tabChainEvidence.html(response);
            },
            complete: function () {
                var accountEvidence = new account_evidence();
                accountEvidence.Initial(ctsCustId, username, subscriberId, subscriber, custId, isHostPermission);

                var exceptions = new account_exceptions();
                exceptions.Initial(ctsCustId, username, subscriberId, isHostPermission);
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function LoadCustomerCategory() {
        $.ajax(common.modifyRelativeUrl('Account/CustomerCategory'), {
            type: 'POST',
            success: function (response) {
                domElements.tabCustomerCategory.html(response);
            },
            complete: function () {
                var customerCategory = new customer_category();
                customerCategory.Initial(custId, ctsCustId, subscriberId, username, isHostPermission);
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function LoadTransactionHistory() {
        $.ajax(common.modifyRelativeUrl('Account/TransactionHistory'), {
            type: 'POST',
            success: function (response) {
                domElements.tabTransactionHistory.html(response);
            },
            complete: function () {
                var transHistory = new transaction_history();
                transHistory.Initial(ctsCustId, subscriberId);
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }  

    function GetCustomerTotalAssociation() {
        $.ajax(common.modifyRelativeUrl('Account/GetCustomerTotalAssociationByCTSCustId'), {
            type: 'POST',
            data: {
                ctsCustId: ctsCustId
            },
            success: function (response) {
                totalAssAccount = response.TotalAccounts;
                domElements.spanTotalAssociated.html(response.TotalAccounts + " / " + response.TotalDevices);               
            },
            complete: function () {
                GetCustomerEvidences();
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function GetCustomerEvidences() {
        $.ajax(common.modifyRelativeUrl('Account/GetCustomerEvidenceByCTSCustId'), {
            type: 'POST',
            data: {
                ctsCustId: ctsCustId
            },
            success: function (response) {
                domElements.spanTotalEvidences.html(response.evidences);

                totalEvidence = response.numberOfItems;

                if (response.numberOfItems === 0) {
                    domElements.spanTotalEvidences.contents().unwrap();
                }
            },
            complete: function () {
                LoadAssociationByAccount();
                LoadChainToEvidence();
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function DangerouseIcon(response) {
        if (response.GeneralStatusInfo.Danger === 0)
            domElements.spanDangerousOriginal.html("None");
        else
            domElements.spanDangerousOriginal.html(response.GeneralStatusInfo.Danger);

        if (response.GeneralStatusInfo.Danger2 === 0)
            domElements.spanDangerouseABI.html("None");
        else
            domElements.spanDangerouseABI.html(response.GeneralStatusInfo.Danger2);

        if (response.GeneralStatusInfo.Danger3 === 0)
            domElements.spanDangerouseOtherSports.html("None");
        else
            domElements.spanDangerouseOtherSports.html(response.GeneralStatusInfo.Danger3);
    }

    function FrauseCaseList(response) {
        if (response.FraudCases !== '')
            domElements.spanFraudcase.html(response.FraudCases);
        else
            domElements.spanFraudcase.html("None");
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
                $('div.classification').html("<span>None</span>");
                HandleCustomerStatus(response);
                FrauseCaseList(response);
                HandleRobotUserStatus(response.RobotUserStatus);
                HandleFlagedAffectedEvidence(response.FlagedAffectedEvidence);
                HandleEsportAutoSuspendedStatus(response.EsportAutoSuspendStatus);
            },
            complete: function () {
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function GetBettingStatistic() {
        let chartTurnoverBySport = new turnover_by_sport();
        let chartTurnoverByBettype = new turnover_by_bettype();
        let chartWinLost = new win_lost();

        chartTurnoverBySport.Initial(custId);
        chartWinLost.Initial(custId);
        chartTurnoverByBettype.Initial(custId);
    }

    function CollapseExpandStatistic() {
        $("#wrapper-statistic .statistic-title").on('click', function () {
            var $this = $(this).next();

            if ($(this).hasClass("expanded")) {
                $(this).removeClass("expanded");
                domElements.statisticIcon.removeClass("icon-open");
                if (!$this.hasClass('hide')) {
                    $this.addClass('hide');
                }
                setStorageStatisticUserBehavior(currentUserId, 'isStatisticOpen=false');
            } else {
                $(this).addClass("expanded");
                domElements.statisticIcon.addClass("icon-open");
                if ($this.hasClass('hide')) {
                    $this.removeClass('hide');
                }
                setStorageStatisticUserBehavior(currentUserId, 'isStatisticOpen=true');
            }
        });
    }

    function HandleCustomerStatus(data) {       
        if (data !== null) {
            DangerouseIcon(data);
            domElements.spanAccountStatus.html(data.StatusLabel);
            domElements.spanAutoSuspendSoccer.html(data.GeneralStatusInfo.Autosuspend ? 'Yes' : 'No');
            domElements.spanAutoSuspendOtherSport.html(data.GeneralStatusInfo.Autosuspend2 ? 'Yes' : 'No');
        }
    }

    function HandleRobotUserStatus(data) {
        if (data !== null && data.IsRobotUser) 
            $('span.icon-Robot').removeClass('hide');
    }

    function HandleFlagedAffectedEvidence(data) {
        $('span.loading').remove();

        if (data.Flagged)
            $('span.icon-flag').removeClass('hide');
        if (data.Affected)
            $('span.icon-error').removeClass('hide');
    }

    function HandleEsportAutoSuspendedStatus(data) {
        if (data !== null) {
            domElements.spanAutoSuspendESport.html(data.EsportAutoSuspendStatus ? 'Yes' : 'No');
        }
    }

    function getStorageStatisticUserBehavior (key) {
        if (typeof Storage !== "undefined") {
            return localStorage.getItem(key);
        }
        return '';
    };

    function setStorageStatisticUserBehavior (key, value) {
        if (typeof Storage !== "undefined") {
            localStorage.setItem(key, value);
        }
    };

    function Initial() {
        CollapseExpandStatistic();
        GetAccountProfile();
        GetCustomerTotalAssociation();
        LoadTransactionHistory();
        LoadCustomerCategory();

        domElements.spanTotalAssociated.on('click', function () {
            domElements.associatedAccountTab.trigger("click");
        });

        domElements.spanTotalEvidences.on('click', function () {
            domElements.evidenceTab.trigger("click");
        });

        $("#tabs-account-detail").tabs();
    }

    function DisplayStatistic() {
        if (isBettingStatisticPermission === "False" || isBettingStatisticPermission === undefined) {
            return;
        }

        var userBehavior = getStorageStatisticUserBehavior(currentUserId);

        domElements.wrapperStatistic.removeClass('hide');  

        if (userBehavior === null) {
            isPreviousStatisticOpen = true;
        }
        else {
            isPreviousStatisticOpen = (userBehavior === 'isStatisticOpen=true') ? true : false;
        }
     
        if (isPreviousStatisticOpen === true) {
            domElements.statisticDetail.removeClass("hide");
            domElements.statisticIcon.addClass("icon-open");

            if (!domElements.statisticTitle.hasClass("expanded")) {
                domElements.statisticTitle.addClass("expanded");
            }
        }
        else 
        {
            domElements.statisticDetail.addClass("hide");
            domElements.statisticIcon.removeClass("icon-open");
        }

        GetBettingStatistic();
           
    }

    $(document).ready(function () {
        DisplayStatistic();
        Initial();
    });
}
$(AccountDetail);