function AccountDetail() {
    var username = $("#hdUsername").val();
    var custId = $("#hdCustId").val();
    var ctsCustId = $("#hdCTSCustId").val();
    var subscriberId = $("#hdSubscriberId").val();
    var subscriber = $("#hdSubscriber").val();
    var isHostPermission = $("#hdIsHostPermission").attr("IsHostPermission");
    var isBettingStatisticPermission = $("#hdIsBettingStatisticPermission").attr("IsBettingStatisticPermission");
    var isViewIPTransactionPermission = $("#hdIsViewIPTransactionPermission").attr("IsViewIPTransactionPermission");
    var currentUserId = $("#hdCurrentUserId").val();
    var custSubId = $("#hdCustSubId").val();
    var isPreviousStatisticOpen = false;

    let domElements = {
        tabAssociatedAccount: $("#associated_account"),
        tabChainEvidence: $("#chains_evidence"),
        tabCustomerCategory: $("#customer_category"),
        tabTransactionHistory: $("#trans_history"),
        tabRobotClassification: $("#robot_classification"),
        tabRobotBetting: $('#robot_betting'),     
        spanAssAccByBettingPattern: $('#spanAssAccByBettingPattern'),
        spanAssAccByManually: $('#spanAssAccByManually'),
        spanAssAccByDevice: $('#spanAssAccByDevice'),
        spanAssAccByIP: $('#spanAssAccByIP'),
        spanClass: $('#spanClass'),
        spanLblFlagEvidence: $('#spanLblFlagEvidence'),
        spanFlagEvidence: $('#spanFlagEvidence'),
        spanLblAffectedEvidence: $('#spanLblAffectedEvidence'),
        spanAffectedEvidence: $('#spanAffectedEvidence'),        
        spanLicVIP: $('#spanLicVIP'),       
        spanDesktopUsage: $('#spanDesktopUsage'),
        spanMobileUsage: $('#spanMobileUsage'),
        spanOthersUsage: $('#spanOthersUsage'),
        spanLoginCount: $('#spanLoginCount'),
        spanIPDeviceCount: $('#spanIPDeviceCount'),
        spanLblNextCategoryScanDate: $('#spanLblNextCategoryScanDate'),
        spanNextCategoryScanDate: $('#spanNextCategoryScanDate'),
        spanLblLastTicketDate: $('#spanLblLastTicketDate'),
        spanLastTicketDate: $('#spanLastTicketDate'),
        spanLblLastLoginDate: $('#spanLblLastLoginDate'),
        spanLastLoginDate: $('#spanLastLoginDate'),
        spanLblCreatedDate: $('#spanLblCreatedDate'),
        spanCreatedDate: $('#spanCreatedDate'),
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
            async: true,
            data: {
                username: username
            },
            success: function (response) {
                domElements.tabAssociatedAccount.html(response);
            },
            complete: function () {
                var associationByAccount = new account_by_account();
                associationByAccount.Initial(ctsCustId, username, subscriberId, subscriber, isHostPermission);

                LoadChainToEvidence();                
                LoadTransactionHistory();
                LoadLastestRobotClassification();
                LoadRobotBetting();
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
            async: true,
            success: function (response) {
                domElements.tabCustomerCategory.html(response);
            },
            complete: function () {
                var customerCategory = new customer_category();
                customerCategory.Initial(custId, ctsCustId, subscriberId, username, isHostPermission, custSubId);

                GetCustProfileDates();

                GetCustProfileLoginInfo();

                GetCustProfileClassification();

                GetCustProfileAssociationAccount();
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function LoadLastestRobotClassification() {
        $.ajax(common.modifyRelativeUrl('Account/RobotClassification'), {
            type: 'POST',
            success: function (response) {
                domElements.tabRobotClassification.html(response);
            },
            complete: function () {
                var robotClassification = new robot_classification();
                robotClassification.Initial(custId);
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function LoadTransactionHistory() {
        let isViewIP = (isViewIPTransactionPermission === "False" || isViewIPTransactionPermission === undefined) ? false : true;

        $.ajax(common.modifyRelativeUrl('Account/TransactionHistory'), {
            type: 'POST',
            success: function (response) {
                domElements.tabTransactionHistory.html(response);
            },
            complete: function () {
                var transHistory = new transaction_history();
                transHistory.Initial(ctsCustId, subscriberId, isViewIP);
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function LoadRobotBetting() {
        $.ajax(common.modifyRelativeUrl('Account/RobotBetting'), {
            type: 'POST',
            success: function (response) {
                domElements.tabRobotBetting.html(response);
            },
            complete: function () {
                var robotBetting = new robot_betting();
                robotBetting.Initial(custId);
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }  

    function GetCustProfileAssociationAccount() {
        $.ajax(common.modifyRelativeUrl('Account/GetCustProfileAssociationAccount'), {
            type: 'POST', 
            traditional: true,
            async: true,
            data: {
                ctsCustId: ctsCustId                 
            },
            success: function (response) {
                if (response !== null) {
                    domElements.spanAssAccByBettingPattern.html(response.AI);
                    domElements.spanAssAccByManually.html(response.Manual);
                    domElements.spanAssAccByDevice.html(response.Device);
                    domElements.spanAssAccByIP.html(response.IP);
                }
            },           
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function GetCustProfileClassification() {
        $.ajax(common.modifyRelativeUrl('Account/GetCustProfileClassification'), {
            type: 'POST',  
            traditional: true,
            async: true,
            data: {
                ctsCustId: ctsCustId,
                custId: custId
            },
            success: function (response) {
                if (response !== null) {
                    domElements.spanClass.html(response.Class);
                    domElements.spanLicVIP.html(response.LicVIP);
                    domElements.spanFlagEvidence.html(response.FlagEvidenceCodes);
                    domElements.spanAffectedEvidence.html(response.AffectedEvidenceCodes); 

                    domElements.spanLblFlagEvidence.css("height", domElements.spanFlagEvidence.css("height"));
                    domElements.spanLblAffectedEvidence.css("height", domElements.spanAffectedEvidence.css("height"));
                }
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }    

    function GetCustProfileLoginInfo() {
        $.ajax(common.modifyRelativeUrl('Account/GetCustProfileLoginInfo'), {
            type: 'POST', 
            traditional: true,
            async: true,
            data: {
                ctsCustId: ctsCustId
            },
            success: function (response) {
                if (response !== null) {
                    domElements.spanDesktopUsage.html(response.DesktopUsage + " (" + response.DesktopUsagePercent +"%)");
                    domElements.spanMobileUsage.html(response.MobileUsage + " (" + response.MobileUsagePercent + "%)");
                    domElements.spanOthersUsage.html(response.OthersUsage + " (" + response.OthersUsagePercent + "%)");
                    domElements.spanLoginCount.html(response.LoginCount);
                    domElements.spanIPDeviceCount.html(response.IPCount + " / " + response.DeviceCount);
                }
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function GetCustProfileDates() {
        $.ajax(common.modifyRelativeUrl('Account/GetCustProfileDates'), {
            type: 'POST',  
            traditional: true,
            async: true,            
            data: {
                ctsCustId: ctsCustId
            },
            success: function (response) {
                if (response !== null) {
                    domElements.spanNextCategoryScanDate.html(response.NextCategoryScanString);
                    domElements.spanLastTicketDate.html(response.LastTicketString);
                    domElements.spanLastLoginDate.html(response.LastLoginString);
                    domElements.spanCreatedDate.html(response.CreatedString);

                    domElements.spanLblNextCategoryScanDate.css("height", domElements.spanNextCategoryScanDate.css("height"));
                    domElements.spanLblLastTicketDate.css("height", domElements.spanLastTicketDate.css("height"));
                    domElements.spanLblLastLoginDate.css("height", domElements.spanLastLoginDate.css("height"));
                    domElements.spanLblCreatedDate.css("height", domElements.spanCreatedDate.css("height"));
                }
            },     
            complete: function () {
                LoadAssociationByAccount();
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

    function getStorageStatisticUserBehavior(key) {
        if (typeof Storage !== "undefined") {
            return localStorage.getItem(key);
        }
        return '';
    };

    function setStorageStatisticUserBehavior(key, value) {
        if (typeof Storage !== "undefined") {
            localStorage.setItem(key, value);
        }
    };    

    function Initial() {

        LoadCustomerCategory();                 
                                  
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
        else {
            domElements.statisticDetail.addClass("hide");
            domElements.statisticIcon.removeClass("icon-open");
        }

        GetBettingStatistic();
        CollapseExpandStatistic();
    }

    $(document).ready(function () {                        
        DisplayStatistic();          

        Initial();    
    });
}

$(AccountDetail);