function AccountDetail() {
    var username = $("#hdUsername").val();
    var custId = $("#hdCustId").val();
    var ctsCustId = $("#hdCTSCustId").val();
    let roleID = $("#hdRoleID").val();
    var subscriberId = $("#hdSubscriberId").val();
    var subscriber = $("#hdSubscriber").val();
    var subscriberSourceID = $("#hdSubscriberSourceID").val();
    var isHostPermission = $("#hdIsHostPermission").attr("IsHostPermission");
    var isBettingStatisticPermission = $("#hdIsBettingStatisticPermission").attr("IsBettingStatisticPermission");
    var isAccountInfoPermission = $("#hdIsAccountInfoPermission").attr("IsAccountInfoPermission");
    var isAccountDetailTabPermission = $("#hdIsAccountDetailTabPermission").attr("IsAccountDetailTabPermission");
    var isViewIPTransactionPermission = $("#hdIsViewIPTransactionPermission").attr("IsViewIPTransactionPermission");
    var currentUserId = $("#hdCurrentUserId").val();
    var custSubId = $("#hdCustSubId").val();
    var isPreviousStatisticOpen = false;
    var currentTabIndex = 1;

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
        spanBot: $('#spanBot'),
        spanInvalidBrowserInfo: $('#spanInvalidBrowserInfo'),
        spanDesktopMobileUsage: $('#spanDesktopMobileUsage'),
        spanIPDevice: $('#spanIPDevice'),
        spanTotalLogin: $('#spanTotalLogin'),
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
        customerClassTab: $("#customer-class-tab"),
        tranHistoryTab: $("#transaction-history-tab"),
        robotClassificationTab: $("#robot-classification-tab"),
        robotBettingTab: $("#robot-betting-tab"),
        statisticTitle: $("#statistic-title"),
        statisticIcon: $("#statistic-title .icon"),
        statisticDetail: $("#statistic-detail"),
        wrapperStatistic: $("#wrapper-statistic")
    };

    function LoadAssociationByAccount() {
        const isAccountDetailTab = isAccountDetailTabPermission === "True";

        if (isAccountDetailTab) {
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
                    //LoadLastestRobotClassification();
                    LoadRobotBetting();
                },
                error: function (errMsg) {
                    console.log(errMsg);
                }
            });
        }
    }

    function LoadChainToEvidence() {
        const isAccountDetailTab = isAccountDetailTabPermission === "True";

        if (isAccountDetailTab) {
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
    }

    function LoadCustomerCategory() {
        const isAccountInfo = isAccountInfoPermission === "True";
        const isAccountDetailTab = isAccountDetailTabPermission === "True";

        if (isAccountInfo || isAccountDetailTab) {
            $.ajax(common.modifyRelativeUrl('Account/CustomerCategory'), {
                type: 'POST',
                async: true,
                success: function (response) {
                    domElements.tabCustomerCategory.html(response);
                },
                complete: function () {
                    var customerCategory = new customer_category();
                    customerCategory.Initial(custId, ctsCustId, subscriberId, username, isHostPermission, custSubId);
                    customerCategory.ReloadStatusRobotUser(custId, roleID);

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
    }

    //function LoadLastestRobotClassification() {
    //    $.ajax(common.modifyRelativeUrl('Account/RobotClassification'), {
    //        type: 'POST',
    //        success: function (response) {
    //            domElements.tabRobotClassification.html(response);
    //        },
    //        complete: function () {
    //            var robotClassification = new robot_classification();
    //            robotClassification.Initial(custId);
    //        },
    //        error: function (errMsg) {
    //            console.log(errMsg);
    //        }
    //    });
    //}

    function LoadTransactionHistory() {
        let isViewIP = (isViewIPTransactionPermission === "False" || isViewIPTransactionPermission === undefined) ? false : true;

        $.ajax(common.modifyRelativeUrl('Account/TransactionHistory'), {
            type: 'POST',
            success: function (response) {
                domElements.tabTransactionHistory.html(response);
            },
            complete: function () {
                var transHistory = new transaction_history();
                transHistory.Initial(ctsCustId, subscriberId, subscriberSourceID, isViewIP);
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }

    function LoadRobotBetting() {
        const isAccountDetailTab = isAccountDetailTabPermission === "True";

        if (isAccountDetailTab) {
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
                    domElements.spanBot.html(FormatNumber(response.BotTrans, response.BotTransPercentage, 1));
                    domElements.spanInvalidBrowserInfo.html(FormatNumber(response.InvalidBrowser, response.InvalidBrowserPercentage, 1));
                    domElements.spanDesktopMobileUsage.html(FormatNumber(response.DesktopUsage, response.MobileUsage, 2));
                    domElements.spanIPDevice.html(FormatNumber(response.IPCount, response.DeviceCount, 2));
                    domElements.spanTotalLogin.html(FormatNumber(response.LoginCount, null, 3));
                }
            },
            error: function (errMsg) {
                console.log(errMsg);
            }
        });
    }


    function FormatNumber(number1, number2, type) {
        if (type == 1 && number1 != null) {
            return FormatNumberUnit(number1) +  " (" + number2 + "%)";
        }

        if (type == 2 && number1 != null && number2 != null) {
            return FormatNumberUnit(number1) + " / " + FormatNumberUnit(number2);
        }

        if (type == 3 && number1 != null) {
            return FormatNumberUnit(number1);
        }

        return "-";
    }

    function FormatNumberUnit(x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }

    function GetCustProfileDates() {
        $.ajax(common.modifyRelativeUrl('Account/GetCustProfileDates'), {
            type: 'POST',  
            traditional: true,
            async: true,            
            data: {
                ctsCustId: ctsCustId,
                roleID: roleID
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

    function TrackingEvidenceFuction() {
        if (currentTabIndex != 2 && domElements.evidenceTab.attr("aria-selected")) {
            TrackingFunction(2);
            currentTabIndex = 2;
        }
    }

    function TrackingCustClassFuction() {
        if (currentTabIndex != 3 && domElements.customerClassTab.attr("aria-selected")) {
            TrackingFunction(3);
            currentTabIndex = 3;
        }
    }

    function TrackingTransFuction() {
        if (currentTabIndex != 4 && domElements.tranHistoryTab.attr("aria-selected")) {
            TrackingFunction(4);
            currentTabIndex = 4;
        }
    }

    function TrackingRobotClassFuction() {
        if (currentTabIndex != 5 && domElements.robotClassificationTab.attr("aria-selected")) {
            TrackingFunction(5);
            currentTabIndex = 5;
        }
    }

    function TrackingRobotBettingFuction() {
        if (currentTabIndex != 6 && domElements.robotBettingTab.attr("aria-selected")) {
            TrackingFunction(6);
            currentTabIndex = 6;
        }
    }
 
    function TrackingFunction(functionId) {
        var functionName = null;

        switch (functionId) {
            case 2:
                {
                    functionName = "TrackingEvidence";
                    break;
                }
            case 3:
                {
                    functionName = "TrackingCustomerCategory";
                    break;
                }
            case 4:
                {
                    functionName = "TrackingTransactionHistory";
                    break;
                }
            case 5:
                {
                    functionName = "TrackingRobotClassification";
                    break;
                }
            case 6:
                {
                    functionName = "TrackingRobotBetting";
                    break;
                }
        }

        if (functionName != null) {
            $.ajax(common.modifyRelativeUrl('Account/' + functionName), {
                type: 'GET'
            });
        }
    }

    function DisplaySection() {
        const isAccountInfo = isAccountInfoPermission === "True";
        const isStatistic = isBettingStatisticPermission === "True";
        const isAccountDetailTab = isAccountDetailTabPermission === "True";

        if (isAccountInfo) {
            $(".wrapper-content").removeClass("hide");
            $("#divHeaderInfo").css("visibility", "visible");
            $("#spanContentInfoLoading").hide();
        }

        if (isStatistic) {
            $("#wrapper-statistic").removeClass("hide");
        }

        if (isAccountDetailTab) {
            $(".content-card").removeClass("hide");
        }

        if (!isAccountInfo) {
            $("#wrapper-content .icon-loading").hide();
            $("#spanContentInfoLoading").hide();
            $("#divHeaderInfo").hide();
        }

        if (!isStatistic) {
            $("#wrapper-statistic .icon-loading").hide();
        }
        if (!isAccountDetailTab) {
            $(".content-card .icon-loading").hide();
            $(".content-card img[src*='ajax-loader.gif']").hide();
        }
    }

    $(document).ready(function () {   
        DisplaySection();
        DisplayStatistic();          

        Initial();    

        domElements.evidenceTab.on('click', () => TrackingEvidenceFuction());
        domElements.customerClassTab.on('click', () => TrackingCustClassFuction());
        domElements.tranHistoryTab.on('click', () => TrackingTransFuction());
        domElements.robotClassificationTab.on('click', () => TrackingRobotClassFuction());
        domElements.robotBettingTab.on('click', () => TrackingRobotBettingFuction());
    });
}

$(AccountDetail);  