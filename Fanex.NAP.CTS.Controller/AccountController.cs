namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.Helpers;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.Account.RobotBetting;
    using Fanex.NAP.CTS.Model.Association;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassification.ProblemAccount;
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.ProblemAccountAgency;
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.RescanCustomerClassificationAgency;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using Fanex.NAP.CTS.Model.Transaction;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Implement.CustomerClassificationAgency;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Interface.CustomerClassificationAgency;
    using Fanex.NAP.CTS.ViewModel.Account;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using Fanex.NAP.CTS.ViewModel.Account.BettingStatistic;
    using Fanex.NAP.CTS.ViewModel.Account.ChainToEvidence;
    using Fanex.NAP.CTS.ViewModel.Account.CustomerCategory;
    using Fanex.NAP.CTS.ViewModel.Account.CustomerCategoryBySport;
    using Fanex.NAP.CTS.ViewModel.Account.CustomerInfo;
    using Fanex.NAP.CTS.ViewModel.Account.Evidence;
    using Fanex.NAP.CTS.ViewModel.Account.RobotBetting;
    using Fanex.NAP.CTS.ViewModel.Account.TransactionHistory;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly ITransactionService _transService;
        private readonly ICustomerCategoryService _customerCategoryService;
        private readonly ICustomerCategoryAgencyService _customerCategoryAgencyService;
        private readonly ICustomerEvidenceService _customerEvidenceService;
        private readonly IRescanCustomerClassificationService _rescanClassificationService;
        private readonly IRescanCustomerClassificationAgencyService _rescanClassificationAgencyService;
        private readonly ICustomerClassificationService _customerClassificationService;
        private readonly ICommonService _commonService;
        private readonly IRobotClassificationService _robotClassificationService;
        private readonly IRobotBettingService _robotBettingService;
        private readonly IAntiFraudCenterService _antiFraudCenterService;
        private readonly IProblemAccountManagementService _pamService;
        private readonly IProblemAccountClassificationService _problemAccountClassificationService;
        private readonly IProblemAccountClassificationAgencyService _problemAccountClassificationAgencyService;
        private readonly IDataLogTrackingServices dataLogTrackingServices;

        public override string BaseControllerName { get => string.Empty; }

        public AccountController()
        {
            _accountService = new AccountService();
            _transService = new TransactionService();
            _customerCategoryService = new CustomerCategoryService();
            _customerEvidenceService = new CustomerEvidenceService();
            _rescanClassificationService = new RescanCustomerClassificationService();
            _customerClassificationService = new CustomerClassificationService();
            _commonService = new CommonService();
            _robotClassificationService = new RobotClassificationService();
            _robotBettingService = new RobotBettingService();
            _antiFraudCenterService = new AntiFraudCenterService();
            _pamService = new ProblemAccountManagementService();
            _problemAccountClassificationService = new ProblemAccountClassificationService();
            this.dataLogTrackingServices = new DataLogTrackingServices();
            _rescanClassificationAgencyService = new RescanCustomerClassificationAgencyService();
            _problemAccountClassificationAgencyService = new ProblemAccountClassificationAgencyService();
            _customerCategoryAgencyService = new CustomerCategoryAgencyService();
        }

        #region Account Info
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileRpt)]
        public ActionResult Index(long Id)
        {
            IUserService userService = new UserService();

            var userId = CurrentSession.UserId;
            var siteId = NapSettings.Settings.SiteId;

            var hasAccountInfoPermission = userService.IsHavingPermission(userId, siteId, FSFunctionKeys.AccountInfo, NAP.Core.Business.Constant.Policy.Read);
            var hasBettingStatisticsPermission = userService.IsHavingPermission(userId, siteId, FSFunctionKeys.BettingStatistics, NAP.Core.Business.Constant.Policy.Read);
            var hasAccountDetailTabPermission = userService.IsHavingPermission(userId, siteId, FSFunctionKeys.AccountDetailTab, NAP.Core.Business.Constant.Policy.Read);
            var hasAccountInfoHostPermission = userService.IsHavingPermission(userId, siteId, FSFunctionKeys.AccountInfo, NAP.Core.Business.Constant.Policy.Host);
            var hasAccountDetailTabHostPermission = userService.IsHavingPermission(userId, siteId, FSFunctionKeys.AccountDetailTab, NAP.Core.Business.Constant.Policy.Host);
            var hasViewIPTransactionPermission = userService.IsHavingPermission(userId, siteId, FSFunctionKeys.ViewIP, NAP.Core.Business.Constant.Policy.Read);

            if (!hasAccountInfoPermission && !hasBettingStatisticsPermission && !hasAccountDetailTabPermission )
            {
                return Redirect("~/Errors/403");
            }

            if (Id > 0)
            {
                var customerInfoFromCTS = _accountService.GetCustomerInfoByCTSCustId(Id);

                if (customerInfoFromCTS != null)
                {
                    var customerGeneralStatusViewModel = new CustomerGeneralStatusViewModel()
                    {
                        CTSCustId = Id,
                        IsHostPermission = hasAccountDetailTabHostPermission,
                        IsAccountInfoPermission = hasAccountInfoPermission, 
                        IsBettingStatisticPermission = CheckBettingStatisticPermission(userService, customerInfoFromCTS.RoleID),
                        IsAccountDetailTabPermission = hasAccountDetailTabPermission, 
                        IsViewIPTransactionPermission = hasViewIPTransactionPermission,
                        IsShowPlusIcon = hasAccountInfoHostPermission,
                        CurrentUserId = userId
                    };

                    customerGeneralStatusViewModel.BindingGeneralDataFromCTS(customerInfoFromCTS);

                    Parallel.Invoke(() =>
                    {
                        try
                        {
                            var custDangerous = _accountService.GetCustomerInfoDangerous(customerInfoFromCTS.CustID);

                            customerGeneralStatusViewModel.BindingDangerous(custDangerous);
                        }
                        catch (Exception ex)
                        {
                            ex.LogError();
                            throw;
                        }
                    },
                    () =>
                    {
                        try
                        {
                            var evidences = _accountService.GetCustomerFlagInfo(Id);

                            customerGeneralStatusViewModel.BindingEvidences(evidences);
                        }
                        catch (Exception ex)
                        {
                            ex.LogError();
                            throw;
                        }
                    });

                    return View(customerGeneralStatusViewModel);
                }
                else
                {
                    LogExtension.LogInfo($"Account Index customerInfoFromCTS {Id} is null");
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileRpt)]
        public ActionResult SSOSearch(long? custId)
        {
            if (custId != null)
            {
                var customerInfo = _accountService.GetCustInfoByCustID(custId.ToString());

                if (customerInfo != null && customerInfo.Any())
                {
                    return RedirectToAction("Index", "Account", new { Id = customerInfo.FirstOrDefault().CTSCustID });
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult CustomerCategory()
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);

            if (!hasPermission)
                return new EmptyResult();

            return PartialView("CustomerCategory/CustomerCategory");
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountInfo, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetStatusRobotClassification(int custID, ROLE_CUSTOMER roleID)
        {
            var result = _robotClassificationService.GetStatusRobotClassification(custID, roleID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountInfo, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetCustomerEvidenceByCTSCustId(long ctsCustId)
        {
            var evidences = _accountService.GetCustomerEvidenceByCTSCustId(ctsCustId);
            var result = "None";
            var numOfItem = 0;

            if (evidences != null && evidences.Any())
            {
                numOfItem = evidences.Count;

                result = string.Join(", ", evidences.Select(x => x.EvidenceCode).ToArray());
            }

            return Json(new { evidences = result, numberOfItems = numOfItem }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountInfo, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetCustomerFlagInfo(long ctsCustId)
        {
            var result = _accountService.GetCustomerFlagInfo(ctsCustId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountInfo, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetCustomerProfileFraudInfo(long custId)
        {
            var result = _accountService.GetCustomerProfileFraudInfo(custId);

            var fraudCaseName = string.Empty;

            if (result.FraudCaseList != null && result.FraudCaseList.Any())
            {
                fraudCaseName = string.Join(", ", result.FraudCaseList.Select(x => x.FraudCaseName));
            }

            return Json(new { suspiciouseCustomer = result.SuspiciouseCustomer, fraudCases = fraudCaseName }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetLastestCustomerClassification(long custId)
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);
            var customerClassification = string.Empty;

            if (!hasPermission)
            {
                return Json(new
                {
                    customerClassification = string.Empty
                }, JsonRequestBehavior.AllowGet);
            }

            var result = _customerClassificationService.GetLatestCCAndDangerLevel(new List<long>() { custId });

            if (result != null && result.Any())
            {
                var cc = result.Where(x => x.CustomerClass != -1).FirstOrDefault();

                if (cc != null)
                {
                    customerClassification = cc.CustomerClassName + " (" + cc.CustomerClass.ToString() + ")";
                }
            }

            return Json(new { customerClassification = customerClassification }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountInfo, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetCustProfileAssociationAccount(long ctsCustId)
        {
            var data = _accountService.GetCustomerInfoAssociationAccount(ctsCustId);

            if (data != null)
            {
                var custProfileAssAccount = new CustomerInfoAssociationAccountViewModel
                {
                    AI = data.AI,
                    Manual = data.Manual,
                    Device = data.Device,
                    IP = data.IP
                };

                return Json(custProfileAssAccount, JsonRequestBehavior.AllowGet);
            }

            return Json(new CustomerInfoAssociationAccountViewModel(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountInfo, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetCustProfileClassification(long ctsCustId, long custId)
        {
            var custProfileEvidence = new CustomerInfoClassificationViewModel();

            Parallel.Invoke(() =>
            {
                try
                {
                    var evidences = _accountService.GetCustomerInfoEvidence(ctsCustId);

                    if (evidences != null && evidences.Any())
                    {
                        var flagEvidenceCodes = evidences.Where(x => x.IsFlag).Select(x => x.EvidenceCode);
                        var affectedEvidenceCodes = evidences.Where(x => !x.IsFlag).Select(x => x.EvidenceCode);

                        custProfileEvidence.FlagEvidenceCodes = flagEvidenceCodes.Any() ? string.Join(", ", flagEvidenceCodes) : "None";
                        custProfileEvidence.AffectedEvidenceCodes = affectedEvidenceCodes.Any() ? string.Join(", ", affectedEvidenceCodes) : "None";
                    }
                    else
                    {
                        custProfileEvidence.FlagEvidenceCodes = "None";
                        custProfileEvidence.AffectedEvidenceCodes = "None";
                    }
                }
                catch (Exception ex)
                {
                    ex.LogError();
                    throw;
                }
            },
            () =>
            {
                try
                {
                    var customerClass = _customerClassificationService.GetLatestCCAndDangerLevel(new List<long>() { custId });

                    if (customerClass != null && customerClass.Any())
                    {
                        var cc = customerClass.Where(x => x.CustomerClass != -1).FirstOrDefault();

                        custProfileEvidence.Class = cc != null
                            ? cc.CustomerClassName + " (" + cc.CustomerClass.ToString() + ")"
                            : string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    ex.LogError();
                    throw;
                }
            });

            return Json(custProfileEvidence, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountInfo, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetCustProfileLoginInfo(long ctsCustId)
        {
            var fromDate = DateTime.Now.AddDays(-90);
            var toDate = DateTime.Now;

            var data = _accountService.GetCustomerInfoLoginStats(ctsCustId, fromDate, toDate);

            if (data != null)
            {
                var custProfileLoginStats = new CustomerInfoLoginStatsViewModel
                {
                    BotTrans = data.BotTrans,
                    BotTransPercentage = data.BotTransPercentage.ToString("0.00"),
                    InvalidBrowser = data.InvalidBrowser,
                    InvalidBrowserPercentage = data.InvalidBrowserPercentage.ToString("0.00"),
                    DesktopUsage = data.DesktopUsage,
                    MobileUsage = data.MobileUsage,
                    IPCount = data.IP,
                    DeviceCount = data.Device,
                    LoginCount = data.Login,
                };

                return Json(custProfileLoginStats, JsonRequestBehavior.AllowGet);
            }

            return Json(new CustomerInfoLoginStatsViewModel(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountInfo, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetCustProfileDates(long ctsCustId, ROLE_CUSTOMER roleID)
        {
            var data = _accountService.GetCustProfileDates(ctsCustId, roleID);

            if (data != null)
            {
                var custProfileDates = new CustomerInfoDateViewModel
                {
                    NextCategoryScan = data.NextCategoryScan,
                    LastTicket = data.LastTicket,
                    LastLogin = data.LastLogin,
                    Created = data.Created
                };

                return Json(custProfileDates, JsonRequestBehavior.AllowGet);
            }

            return Json(new CustomerInfoDateViewModel(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileGetCustomerCategoryBySportRpt)]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountInfo, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetCustomerCategoryBySport(long ctsCustID)
        {
            var custs = Enumerable.Empty<GetCustomerCategoryBySportViewModel>();

            var data = _accountService.GetCustomerCategoryBySport(ctsCustID);

            if (data != null && data.Any())
            {
                custs = data.Select(x => new GetCustomerCategoryBySportViewModel(x)).OrderBy(x => CustomerCategorySportOrder.TryGetValue((CustomerCategorySportType)x.SportID, out var order) ? order : 999);
            }

            return Json(new { data = custs }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileGetCustomerCategoryHistoryBySportRpt)]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetCustomerCategoryHistoryBySport(long ctsCustID, int sportID)
        {
            var custs = Enumerable.Empty<GetCustomerCategoryHistoryBySportViewModel>();

            var data = _accountService.GetCustomerCategoryHistoryBySport(ctsCustID, sportID);

            if (data != null && data.Any())
            {
                custs = data.Select(x => new GetCustomerCategoryHistoryBySportViewModel(x));
            }

            return Json(new { data = custs }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Betting Statistic
        public ActionResult GetPerformanceBySport(long custId, int getTop)
        {
            var totalSportBettingStatistic = new SportBettingStatisticViewModel();
            var bestSportBettingStatistic = new SportBettingStatisticViewModel();
            var pieChartSportStatisticSessions = new List<SportStatisticPieChartViewModel>();

            var dataBySport = _accountService.GetPerformanceBySport(custId, getTop)?.OrderBy(c => c.Ranking);

            if (dataBySport != null && dataBySport.Any())
            {
                try
                {
                    var tasks = new Task[]
                    {
                        Task.Run(() =>
                        {
                            totalSportBettingStatistic.TurnOver = dataBySport.Sum(c => c.TurnOver);
                            totalSportBettingStatistic.WinLoss = dataBySport.Sum(c => c.WinLoss);
                            totalSportBettingStatistic.BetCount = dataBySport.Sum(c => c.BetCount);
                        }),
                        Task.Run(() =>
                        {
                            var tempBestSport = dataBySport.FirstOrDefault(c => c.Ranking == dataBySport.Min(x => x.Ranking));
                            bestSportBettingStatistic.SportID = tempBestSport.SportID;
                            bestSportBettingStatistic.SportName = tempBestSport.SportName;
                            bestSportBettingStatistic.TurnOver = tempBestSport.TurnOver;
                            bestSportBettingStatistic.WinLoss = tempBestSport.WinLoss;
                            bestSportBettingStatistic.BetCount = tempBestSport.BetCount;
                        }),
                        Task.Run(() =>
                        {
                             CalculatePieChartTurnOverPercentage(dataBySport, pieChartSportStatisticSessions);
                        })
                    };

                    Task.WhenAll(tasks).Wait();
                }
                catch (Exception ex)
                {
                    ex.LogError("CTS - Profile Page Get Performance By Sport");
                }
            }

            return Json(new
            {
                totalSportBettingStatistic,
                bestSportBettingStatistic,
                pieChartSportStatisticSessions
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPerformanceByBettype(long custId, int getTop)
        {
            var result = _accountService.GetPerformanceByBettype(custId, getTop)?.OrderBy(c => c.Ranking);
            var liveIndicatorList = new List<LiveIndicatorViewModel>();
            var betTypeCategories = new List<string>();

            CalculateLiveIndicatorPercentage(result, liveIndicatorList, betTypeCategories);

            return Json(new
            {
                betTypeCategories,
                liveIndicatorList
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPerformanceDaily(long custId)
        {
            var result = _accountService.GetPerformanceDaily(custId).OrderBy(c => c.RptDate);
            var turnOverAreaData = new List<decimal>();
            var winLossAreaDate = new List<decimal>();
            var days = new List<string>();

            if (result != null && result.Any())
            {
                result.ToList().ForEach(c =>
                {
                    days.Add(c.RptDate.ToString("MM - dd"));
                    turnOverAreaData.Add(Math.Round(c.Turnover));
                    winLossAreaDate.Add(Math.Round(c.Winloss));
                });
            }

            return Json(new
            {
                turnOverAreaData,
                winLossAreaDate,
                days
            }, JsonRequestBehavior.AllowGet);
        }

        private void CalculatePieChartTurnOverPercentage(IEnumerable<PerformanceBySportModel> result, List<SportStatisticPieChartViewModel> pieChartSportStatisticSessions)
        {
            result.ToList().ForEach(c =>
            {
                if ((c.SportID == -99 && c.BetCount != 0) || c.SportID != -99)
                {
                    pieChartSportStatisticSessions.Add(new SportStatisticPieChartViewModel
                    {
                        name = c.SportName,
                        selected = true,
                        y = c.TurnOver,
                        turnOver = Math.Round(c.TurnOver)
                    });
                }
            });
        }

        private void CalculateLiveIndicatorPercentage(IEnumerable<PerformanceByBettypeModel> result, List<LiveIndicatorViewModel> liveIndicatorList, List<string> betTypeCategories)
        {
            if (result == null || !result.Any())
            {
                return;
            }

            decimal maxTurnOverOfBetType = Math.Round((result.GroupBy(c => new { c.BettypeId })
                                              .Select(x => new { BettypeId = x.Key, TotalTurnOver = x.Sum(w => w.Turnover) }))
                                              .Max(c => c.TotalTurnOver));

            liveIndicatorList.Add(new LiveIndicatorViewModel { name = "None", data = new List<decimal>() });
            liveIndicatorList.Add(new LiveIndicatorViewModel { name = "Live", data = new List<decimal>() });
            liveIndicatorList.Add(new LiveIndicatorViewModel { name = "Non-Live", data = new List<decimal>() });

            result.GroupBy(c => c.Ranking).ToList().ForEach(c =>
            {
                decimal live = (c.FirstOrDefault(x => x.LiveIndicator == true)?.Turnover).GetValueOrDefault();
                decimal nonelive = (c.FirstOrDefault(x => x.LiveIndicator == false)?.Turnover).GetValueOrDefault();

                liveIndicatorList.ElementAt(1).data.Add(Math.Round(live));
                liveIndicatorList.ElementAt(2).data.Add(Math.Round(nonelive));
                liveIndicatorList.ElementAt(0).data.Add(maxTurnOverOfBetType - nonelive - live);
                betTypeCategories.Add(c.FirstOrDefault().BettypeName);
            });

            int numberOfBettype = result.GroupBy(c => c.Ranking).Count();
            if (numberOfBettype < 4)
            {
                int loop = 4 - numberOfBettype;
                for (int i = 0; i < loop; i++)
                {
                    liveIndicatorList.ElementAt(0).data.Add(maxTurnOverOfBetType);
                    liveIndicatorList.ElementAt(1).data.Add(0);
                    liveIndicatorList.ElementAt(2).data.Add(0);
                    betTypeCategories.Add("-");
                }
            }
        }

        private bool CheckBettingStatisticPermission(IUserService userService, int roleID)
        {
            if (roleID == Consts.AGENT_ROLE || roleID == Consts.MASTER_ROLE || roleID == Consts.SUPER_ROLE)
            {
                return false;
            }
            
            return userService.IsHavingPermission(CurrentSession.UserId, NapSettings.Settings.SiteId, FSFunctionKeys.BettingStatistics, NAP.Core.Business.Constant.Policy.Read);
        }

        #endregion Betting Statistic

        #region Account Detail Tab

        #region Associated By Account
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult AssociatedByAccount(string username)
        {
            var getAssociationStatus = _accountService.GetCustomerStaticStatus(3, true);

            getAssociationStatus.Insert(0, new CustomerStaticStatusModel { ItemId = 0, ItemNameDisplay = "All" });

            var getCustomerStaticAssType = _accountService.GetCustomerStaticStatus(2, true);

            getCustomerStaticAssType.Insert(0, new CustomerStaticStatusModel { ItemId = 0, ItemNameDisplay = "All" });

            var accountStatus = _accountService.GetAccountStaticStatuses(1, true);

            var subscribers = _commonService.GetSubscriberGroups();

            var roles = _commonService.GetRoles();

            var associatedByAccountViewModel = new AssociatedByAccountViewModel
            {
                AssociationStatus = getAssociationStatus,
                AssociationTypeList = getCustomerStaticAssType,
                AccountStatusList = accountStatus,
                SubscribersList = subscribers.ToList(),
                RoleList = roles.ToList(),
                Username = username
            };

            return PartialView("Associated/AccountByAccount", associatedByAccountViewModel);
        }

        [HttpGet]
        public ActionResult GetAllCategories()
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);
            var categoryList = new List<CategoryGroupViewModel>();

            if (!hasPermission) 
                return Json(categoryList, JsonRequestBehavior.AllowGet);

            var data = _accountService.GetAllCategory();

            if (data != null && data.Any())
            {
                categoryList = data
                    .GroupBy(g => new { g.CategoryGroup })
                    .Select(x => new { Group = x.Key, CategoryList = x.ToList() })
                    .Select(x => new CategoryGroupViewModel
                    {
                        CategoryGroupName = x.Group.CategoryGroup,
                        CategoryList = x.CategoryList.Select(y => new CategoryViewModel
                        {
                            CategoryIDs = y.CategoryIDs,
                            CategoryName = y.CategoryName,
                            RoleGroup = y.RoleGroup
                        }).OrderBy(y => y.CategoryName)
                    }).ToList();
            }

            return Json(categoryList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileAssAccountRpt, "isSkipTracking")]
        public ActionResult GetAssociatedByAccount(AssociatedFilterModel model, bool? isSkipTracking)
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);
            var totalItem = 0;
            var result = new List<CustomerAssociationByAccountDetailViewModel>();

            if (!hasPermission || model.CTSCustId == 0)
            {
                return Json(new
                {
                    draw = model.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = result
                }, JsonRequestBehavior.AllowGet);
            }

            var graphAssociationPage = _accountService.GetAssociatedAccount(model);

            if (graphAssociationPage != null && graphAssociationPage.AssociationList != null && graphAssociationPage.AssociationList.Any())
            {
                totalItem = graphAssociationPage.TotalItems;
                var customerAssociation = graphAssociationPage.AssociationList;
                var ctsCustIdList = customerAssociation.Select(x => new { CTSCustId = x.CTSCustID, SubscriberId = x.SubscriberID });

                customerAssociation = GetCustClassification(
                    string.Join(",", customerAssociation.Select(c => c.CustId).Distinct()),
                    customerAssociation.ToList()
                );

                var flaggedAffectedTagList = _accountService.GetDirectAssociatedAccountTagFlagged(JsonConvert.SerializeObject(ctsCustIdList));

                result = customerAssociation.Select(x =>
                {
                    var newX = new CustomerAssociationByAccountDetailViewModel(x);

                    var flaggedAffected = flaggedAffectedTagList
                        .FirstOrDefault(y => y.SubscriberID == newX.SubscriberId && y.CTSCustID == newX.CTSCustId);

                    if (flaggedAffected != null)
                    {
                        newX.IsFlaggedTag = flaggedAffected.IsFlagged;
                        newX.IsAffectedTag = flaggedAffected.IsAffected;
                    }

                    return newX;
                }).ToList();
            }

            return Json(new
            {
                draw = model.draw,
                recordsTotal = graphAssociationPage?.TotalItems ?? 0,
                recordsFiltered = totalItem,
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        private List<AssociatedAccountModel> GetCustClassification(string custIds, List<AssociatedAccountModel> assModel)
        {
            var custClassification = new List<LatestCCDangerLevelModel>();

            if (!string.IsNullOrEmpty(custIds))
            {
                custClassification = _customerClassificationService.GetLatestCCAndDangerLevel(custIds)?.ToList();
            }

            var result = assModel.GroupJoin(custClassification,
                                 a => a.CustId,
                                 b => b.CustID,
                                 (a, b) => new { a, b })
                                 .SelectMany(
                                      group => group.b.DefaultIfEmpty(),
                                      (group, temp) => new AssociatedAccountModel
                                      {
                                          CTSCustID = group.a.CTSCustID,
                                          CustId = group.a.CustId,
                                          CustSubId = group.a.CustSubId,
                                          SubscriberID = group.a.SubscriberID,
                                          SubscriberName = group.a.SubscriberName,
                                          Username = group.a.Username,
                                          Username2 = group.a.Username2,
                                          AssociationType = group.a.AssociationType,
                                          AssociationTypeName = group.a.AssociationTypeName,
                                          AssociationDate = group.a.AssociationDate,
                                          SharedDevices = group.a.SharedDevices,
                                          SharedIPs = group.a.SharedIPs,
                                          StatusName = group.a.StatusName,
                                          StatusId = group.a.StatusId,
                                          AssociationStatus = group.a.AssociationStatus,
                                          ParentIDs = group.a.ParentIDs,
                                          IsProbation = group.a.IsProbation,
                                          CategoryIDs = group.a.CategoryIDs,
                                          CategoryNames = group.a.CategoryNames,
                                          CustomerClasses = (temp != null && group.a.CustSubId == 0) ? temp.CustomerClass.ToString() : "-",
                                          GroupIDs = group.a.GroupIDs,
                                          OriginGroupIDs = group.a.OriginGroupIDs,
                                          ColorTypeIDs = group.a.ColorTypeIDs
                                      }).ToList();

            return result;
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileAssAccountViewOnGraph)]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetDirectAssociationGraph(AssociatedFilterModel model, string displayName)
        {
            var graphCustomer = new GraphCustomerItemViewModel();

            var graphAssociationPaging = _accountService.GetAssociatedAccount(model);

            if (graphAssociationPaging != null && graphAssociationPaging.AssociationList != null && graphAssociationPaging.AssociationList.Any())
            {
                var graphCustomerList = graphAssociationPaging.AssociationList
                    .Select(x => new GraphCustomerItemViewModel
                    {
                        CTSCustId = x.CTSCustID,
                        ItemCode = x.CTSCustID.ToString() + "_" + DateTime.Now.Ticks.ToString(),
                        DisplayName = !string.IsNullOrWhiteSpace(x.Username) ? x.Username : x.Username2,
                        AssociationStatus = model.AssociationStatus,
                        AssociationType = model.AssociationType,
                        AccountStatusIds = model.AccountStatusIds,
                        CategoryIds = model.CategoryIds,
                        SubscriberIds = model.SubscriberIds,
                        Start = model.start,
                        Length = model.length
                    }).ToList();

                graphCustomer.children = graphCustomerList.ToArray();
            }

            graphCustomer.CTSCustId = model.CTSCustId;
            graphCustomer.DisplayName = displayName;
            graphCustomer.AssociationStatus = model.AssociationStatus;
            graphCustomer.AssociationType = model.AssociationType;
            graphCustomer.AccountStatusIds = model.AccountStatusIds;
            graphCustomer.CategoryIds = model.CategoryIds;
            graphCustomer.SubscriberIds = model.SubscriberIds;
            graphCustomer.Start = model.start;
            graphCustomer.Length = model.length;

            return Json(graphCustomer, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult ValidateManualAssociationMultiple(long fromCTSCustId, string toUserNameList)
        {
            var viewModel = new ValidateManualAssociationMultipleViewModel();
            bool isException = false;
            var resultValidated = _accountService.ValidateManualAssociationMultiple(fromCTSCustId, toUserNameList, out isException);

            return resultValidated.Any()
                ? Json(GetInvalidOrDuplicated(resultValidated, isException), JsonRequestBehavior.AllowGet)
                : Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        private ValidateManualAssociationMultipleViewModel GetInvalidOrDuplicated(IEnumerable<ValidateManualAssociationMultipleModel> list, bool isException)
        {
            return new ValidateManualAssociationMultipleViewModel
            {
                IsException = isException,
                UserNameInvalidList = list.Where(x => x.CTSCustID.Equals(0))
                                          .Select(x => x.UserName),
                UserNameDuplicatedList = list.Where(x => x.IsDuplicated)
                                             .Select(x => x.UserName)
            };
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileAssAccountInsert)]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult InsertManualAssociationMultiple(int fromSubscriberId, long fromCTSCustId, string toUserNameList, string remark)
        {
            var resultInsert = new InsertManualAssociationMultipleViewModel { ErrorCode = -1 };
            bool isException = false;

            var resultValidated = _accountService.ValidateManualAssociationMultiple(fromCTSCustId, toUserNameList, out isException);

            if (resultValidated.Any())
            {
                var invalidOrDuplicatedList = GetInvalidOrDuplicated(resultValidated, isException);

                resultInsert.ResultValidated = invalidOrDuplicatedList;

                var isNotAllowInsert = invalidOrDuplicatedList.UserNameInvalidList.Any() || invalidOrDuplicatedList.UserNameDuplicatedList.Any();

                return isNotAllowInsert
                    ? Json(resultInsert, JsonRequestBehavior.AllowGet)
                    : InsertManualAssociationMultiple(resultValidated, fromSubscriberId, fromCTSCustId, remark);
            }

            return Json(resultInsert, JsonRequestBehavior.AllowGet);
        }

        private ActionResult InsertManualAssociationMultiple(IEnumerable<ValidateManualAssociationMultipleModel> list, int fromSubscriberId, long fromCTSCustId, string remark)
        {
            var associationList = new List<InsertManualAssociationMultipleModel>();
            var userId = CurrentSession.UserId;

            foreach (var association in list)
            {
                associationList.Add(new InsertManualAssociationMultipleModel
                {
                    FromSubscriberID = fromSubscriberId,
                    FromCTSCustID = fromCTSCustId,
                    ToSubscriberID = association.SubscriberID,
                    ToCTSCustID = association.CTSCustID,
                    Remark = remark,
                    CreatedBy = userId
                });
            }

            var associationListJson = JsonConvert.SerializeObject(associationList);

            var dbInsert = _accountService.InsertManualAssociationMultiple(associationListJson);

            var resultInsert = new InsertManualAssociationMultipleViewModel()
            {
                ErrorCode = dbInsert.ErrorCode,
                ErrorMessage = dbInsert.ErrorMsg
            };

            return Json(resultInsert, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileAssAccountLinkedUnlinked)]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult ActiveRemoveAssociation(int fromSubscriberId, long fromCTSCustId, int toSubscriberId, long toCTSCustId, string remark, bool isLinked)
        {
            if (fromSubscriberId <= 0 || fromCTSCustId <= 0 || toSubscriberId <= 0 || toCTSCustId <= 0)
            {
                return Json(new
                {
                    errorCode = -100,
                    errorMessage = "Paramater is invalid."
                }, JsonRequestBehavior.AllowGet);
            }

            var dbResult = _accountService.ActiveRemoveAssociation(fromSubscriberId, fromCTSCustId, toSubscriberId, toCTSCustId, remark, CurrentSession.UserId, isLinked);

            return Json(new
            {
                errorCode = dbResult.ErrorCode,
                errorMessage = dbResult.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileAssAccountDetailRpt)]
        public ActionResult GetMatchesAssociationByAI(long custID1, long custID2, string groupIDs, string originGroupIDs)
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);
            var data = new MatchesAssociationByAIViewModel(originGroupIDs);

            if (!hasPermission)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var para1 = (custID1 <= custID2) ? custID1 : custID2;
                var para2 = (custID1 > custID2) ? custID1 : custID2;

                var groupBettingAssociation = _accountService.GetGBAssociationFromAI(para1, para2, originGroupIDs);

                if (groupBettingAssociation.ErrorCode == 0 && groupBettingAssociation.MatchesInfo != null && groupBettingAssociation.MatchesInfo.Any())
                {
                    var matchIDs = groupBettingAssociation.MatchesInfo.Select(c => c.MatchID).Distinct();

                    var matchesInfo = _accountService.GetBettingPatternMatchesInfo(string.Join(",", matchIDs), custID1 + "," + custID2);

                    if (matchesInfo != null && matchesInfo.Any())
                    {
                        data = new MatchesAssociationByAIViewModel(originGroupIDs)
                        {
                            ErrorMessage = groupBettingAssociation.ErrorMessage,
                            MatchesInfo = matchesInfo.OrderByDescending(c => c.EventDate)
                                                     .ThenByDescending(c => c.MatchID)
                                                     .Take(50)
                                                     .Select(c => new MatchesInfoViewModel
                                                     {
                                                         MatchDesc = c.MatchID
                                                                    + " - " + c.HomeName + " vs " + c.AwayName
                                                                    + " - " + c.EventDate.ToString(Consts.FormatDateToString_MMMddyyyy)
                                                                    + " - " + c.LeagueName,
                                                         MatchID = c.MatchID,
                                                         EventDateString = c.EventDate.ToString(Consts.FormatDateToString_yyyyMMddHHmmss),
                                                         CustID1 = para1,
                                                         CustID2 = para2,
                                                         GroupIDs = groupIDs,
                                                         OrginGroupIDs = originGroupIDs
                                                     })
                        };
                    }
                }

                if (groupBettingAssociation.ErrorCode != 0)
                {
                    LogExtension.LogError($"Association Detail throws exception - SPU_AIML_GB_GetAssociationInfo: {groupBettingAssociation.ErrorMessage} - {JsonConvert.SerializeObject(new { para1, para2, originGroupIDs })}");
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetMatchesAssociationDetailByAI(MatchesAssociationDetailViewModel para)
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);
            var data = new List<MatchesDetailInfoViewModel>();

            if (!hasPermission || para == null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var groupBettingAssociation = _accountService.GetGBAssociationFromAI(para.CustID1, para.CustID2, para.OrginGroupIDs);

                if (groupBettingAssociation.ErrorCode == 0 && groupBettingAssociation.MatchesInfo.Any())
                {
                    var originGroupByMatchID = groupBettingAssociation.MatchesInfo.Where(c => c.MatchID == para.MatchId);

                    if (originGroupByMatchID != null && originGroupByMatchID.Any())
                    {
                        var specialGBs = originGroupByMatchID.Where(c => c.GroupID == 0).FirstOrDefault();
                        var generalGBs = originGroupByMatchID.Where(c => c.GroupID != 0);
                        var sgbData = Enumerable.Empty<MatchesDetailInfoViewModel>();
                        var ggbData = Enumerable.Empty<MatchesDetailInfoViewModel>();

                        Parallel.Invoke(() =>
                        {
                            if (specialGBs != null)
                            {
                                var sgbTicketDetails = _accountService.GetTicketDetails(para, para.CustID1 + "," + para.CustID2, specialGBs.AdditionalInfo);

                                sgbData = _accountService.MergeCustGroupTicketInfo(null, sgbTicketDetails, false);
                            }
                        },
                        () =>
                        {
                            if (generalGBs != null && generalGBs.Any())
                            {
                                var groupInfo = _accountService.GetCustGroupAssociation(para.GroupIDs);

                                if (groupInfo.Any())
                                {
                                    var matchGroupInfo = _accountService.MergeCustMatchInfo(generalGBs, groupInfo);

                                    var custIds = string.Join(",", matchGroupInfo.Select(c => c.CustID).Distinct());

                                    var ggbTicketDetails = _accountService.GetTicketDetails(para, custIds, null);

                                    if (ggbTicketDetails != null && ggbTicketDetails.Any())
                                    {
                                        ggbData = _accountService.MergeCustGroupTicketInfo(matchGroupInfo.Distinct(x => new { x.CustID, x.GroupID }), ggbTicketDetails, true);
                                    }
                                }
                            }
                        });

                        data.AddRange(sgbData);
                        data.AddRange(ggbData);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileAssAccountDetailRpt)]
        public ActionResult GetCoupleInformationsByAI(long custID1, long custID2)
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);
            var data = new CoupleDetailParameterViewModel();

            if (!hasPermission)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var para1 = (custID1 <= custID2) ? custID1 : custID2;
                var para2 = (custID1 > custID2) ? custID1 : custID2;

                var firstAssociation = _accountService.GetAIFristAssociationDate(para1, para2, AccountAssociation_ByAIPattern.Pattern_3G5S);

                if (firstAssociation != null && firstAssociation.Any())
                {
                    data.FirstCreatedDate = firstAssociation.FirstOrDefault();

                    var coupleInfo = _accountService.GetCoupleInfos(para1, para2);

                    if (coupleInfo.CoupleInfo.Any())
                    {
                        data.CoupleInfo.Add(new CoupleDetailParameterModel
                        {
                            CoupleInfoDetails = coupleInfo.CoupleInfoDetails,
                            CustID1 = custID1,
                            CustID2 = custID2,
                            PatternType = (int)AccountAssociationByAI_3G5SPattern.Stake
                        });

                        data.CoupleInfo.Add(new CoupleDetailParameterModel
                        {
                            CoupleInfoDetails = coupleInfo.CoupleInfoDetails,
                            CustID1 = custID1,
                            CustID2 = custID2,
                            PatternType = (int)AccountAssociationByAI_3G5SPattern.IP
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetStakeIPPatternDetailByAI(CoupleDetailParameterModel para)
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);
            var data = new List<StakeIpPatternDetailViewModel>();

            if (!hasPermission || para == null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            try
            {
                StakeIpPatternDetailModel patternDetail = new StakeIpPatternDetailModel()
                {
                    CustId1 = para.CustID1,
                    CustId2 = para.CustID2,
                    LastGroupCustID = 1,
                    LastGroupID = 1
                };

                if (para.PatternType == (int)AccountAssociationByAI_3G5SPattern.Stake)
                {
                    patternDetail.StakeFraudDetail = _accountService.GetCoupleFraudDetails(para.CoupleInfoDetails);
                }
                else
                {
                    patternDetail.IpFraudDetail = _accountService.GetCoupleIPFraudDetails(para.CoupleInfoDetails);
                }

                patternDetail.TicketDetails = Get5G3STicketDetail(patternDetail.StakeFraudDetail, patternDetail.IpFraudDetail);

                data.AddRange(GetStakePatternDetail(patternDetail));
                data.AddRange(GetIpPatternDetail(patternDetail));
            }
            catch (Exception ex)
            {
                ex.LogError();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<TicketDetailsModel> Get5G3STicketDetail(IEnumerable<CoupleFraudDetailModel> coupleFraudDetails, IEnumerable<CoupleIPFraudDetailModel> coupleIPFraudDetails)
        {
            var unionAllFraudTransList = new List<string>();

            if (coupleFraudDetails != null && coupleFraudDetails.Any())
            {
                coupleFraudDetails.ToList().ForEach(c =>
                {
                    unionAllFraudTransList.AddRange(c.FraudTransListSperation);
                });
            }

            if (coupleIPFraudDetails != null && coupleIPFraudDetails.Any())
            {
                coupleIPFraudDetails.ToList().ForEach(c =>
                {
                    unionAllFraudTransList.AddRange(c.FraudTransListSperation);
                });
            }

            var fraudTransList = unionAllFraudTransList.Any() ? string.Join(",", unionAllFraudTransList.Distinct()) : string.Empty;

            return string.IsNullOrEmpty(fraudTransList) ? Enumerable.Empty<TicketDetailsModel>() : _accountService.GetAIAssociationTicketDetails(fraudTransList, null, null);
        }

        private IEnumerable<StakeIpPatternDetailViewModel> GetStakePatternDetail(StakeIpPatternDetailModel stakeIpPatternDetailModel)
        {
            var result = new List<StakeIpPatternDetailViewModel>();

            if (!stakeIpPatternDetailModel.StakeFraudDetail.Any())
            {
                return result;
            }

            var groupStateIds = stakeIpPatternDetailModel.StakeFraudDetail.GroupBy(g => g.StateID).Select(s => new { StateId = s.Key, BetCount = s.Max(m => m.BetCount) });

            foreach (var groupStateId in groupStateIds.OrderByDescending(desc => desc.BetCount))
            {
                var coupleFraudDetailByStateIds = stakeIpPatternDetailModel.StakeFraudDetail.Where(p => p.StateID == groupStateId.StateId).OrderByDescending(desc => desc.EventDate).ThenByDescending(desc => desc.MatchID);

                foreach (var coupleFraudDetail in coupleFraudDetailByStateIds)
                {
                    var resultCust1 = GetAI5g3sStakeAssDetail(stakeIpPatternDetailModel, coupleFraudDetail, true).OrderBy(asc => asc.Stake).ThenByDescending(asc => asc.TransDate);
                    if (resultCust1.Any())
                    {
                        result.AddRange(resultCust1.Select(s =>
                        {
                            s.IsStakePattern = true;
                            s.TicketsPerMatch = coupleFraudDetail.BetCount;
                            s.TotalStake = coupleFraudDetail.SumStake;
                            return s;
                        }));
                        stakeIpPatternDetailModel.LastGroupCustID++;
                    }
                }

                foreach (var coupleFraudDetail in coupleFraudDetailByStateIds)
                {
                    var resultCust2 = GetAI5g3sStakeAssDetail(stakeIpPatternDetailModel, coupleFraudDetail, false).OrderBy(asc => asc.Stake).ThenByDescending(asc => asc.TransDate);
                    if (resultCust2.Any())
                    {
                        result.AddRange(resultCust2.Select(s =>
                        {
                            s.IsStakePattern = true;
                            s.TicketsPerMatch = coupleFraudDetail.BetCount;
                            s.TotalStake = coupleFraudDetail.SumStake;
                            return s;
                        }));
                        stakeIpPatternDetailModel.LastGroupCustID++;
                    }
                }

                stakeIpPatternDetailModel.LastGroupID++;
            }

            return result;
        }

        private IEnumerable<StakeIpPatternDetailViewModel> GetAI5g3sStakeAssDetail<T>(StakeIpPatternDetailModel stakeIpPatternDetail, T coupleFraudDetail, bool isCustId1) where T : FraudDetailModel
        {
            var custId = isCustId1 ? stakeIpPatternDetail.CustId1 : stakeIpPatternDetail.CustId2;
            var transIds = coupleFraudDetail.FraudTransListSperation.ToArray();
            var ticketDetails = stakeIpPatternDetail.TicketDetails.Where(p => coupleFraudDetail.MatchID == p.MatchID && custId == p.CustID && transIds.Contains(p.TransId.ToString()));

            var result = GetAI5G3SAssDetail(stakeIpPatternDetail, ticketDetails, AccountAssociationByAI_3G5SPattern.Stake);

            return result ?? Enumerable.Empty<StakeIpPatternDetailViewModel>();
        }

        private IEnumerable<StakeIpPatternDetailViewModel> GetAI5G3SAssDetail(StakeIpPatternDetailModel stakeIpPatternDetail, IEnumerable<TicketDetailsModel> ticketDetails, AccountAssociationByAI_3G5SPattern patternType)
        {
            var result = new List<StakeIpPatternDetailViewModel>();

            if (ticketDetails.Any())
            {
                var countTicketDetails = ticketDetails.Count();

                foreach (var ticketDetail in ticketDetails)
                {
                    result.Add(new StakeIpPatternDetailViewModel()
                    {
                        GroupCustID = stakeIpPatternDetail.LastGroupCustID,
                        GroupID = stakeIpPatternDetail.LastGroupID,
                        CountGroupCustID = countTicketDetails,
                        MatchID = ticketDetail.MatchID,
                        UserName = ticketDetail.UserName,
                        TransId = ticketDetail.TransId.ToString(),
                        IP = ticketDetail.IP,
                        Odds = ticketDetail.Odds,
                        Stake = ticketDetail.Stake,
                        StakeString = String.Format("{0:#.##}", ticketDetail.Stake),
                        //StakeRM = String.Format("{0:#.##}", Math.Truncate(ticketDetail.StakeRM)),
                        BetTeam = ticketDetail.BetTeam,
                        BetType = ticketDetail.TypeName,
                        Currency = ticketDetail.CurrencyName,
                        LiveScore = ticketDetail.LiveHomeScore != null && ticketDetail.LiveAwayScore != null ? $"{ticketDetail.LiveHomeScore}-{ticketDetail.LiveAwayScore}" : "",
                        TransDateString = ticketDetail.TransDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                        TransDate = ticketDetail.TransDate,
                        PatternType = (int)patternType,
                        EventDate = ticketDetail.EventDate
                    });
                }
            }

            return result;
        }

        private IEnumerable<StakeIpPatternDetailViewModel> GetIpPatternDetail(StakeIpPatternDetailModel stakeIpPatternDetailModel)
        {
            var result = new List<StakeIpPatternDetailViewModel>();

            if (!stakeIpPatternDetailModel.IpFraudDetail.Any() || !stakeIpPatternDetailModel.TicketDetails.Any())
            {
                return result;
            }

            var groupIPs = stakeIpPatternDetailModel.TicketDetails.GroupBy(g => g.IP, (key, group) => new { IP = key, Tickets = group });

            foreach (var groupIP in groupIPs)
            {
                var ticketCustId1 = groupIP.Tickets.Where(c => c.CustID == stakeIpPatternDetailModel.CustId1);
                var resultCust1 = GetAI5G3SAssDetail(stakeIpPatternDetailModel, ticketCustId1, AccountAssociationByAI_3G5SPattern.IP);

                if (resultCust1.Any())
                {
                    result.AddRange(resultCust1.OrderByDescending(desc => desc.EventDate).ThenByDescending(desc => desc.MatchID).ThenByDescending(desc => desc.TransDate));
                    stakeIpPatternDetailModel.LastGroupCustID++;
                }

                var ticketCustId2 = groupIP.Tickets.Where(c => c.CustID == stakeIpPatternDetailModel.CustId2);
                var resultCust2 = GetAI5G3SAssDetail(stakeIpPatternDetailModel, ticketCustId2, AccountAssociationByAI_3G5SPattern.IP);

                if (resultCust2.Any())
                {
                    result.AddRange(resultCust2.OrderByDescending(desc => desc.EventDate).ThenByDescending(desc => desc.MatchID).ThenByDescending(desc => desc.TransDate));
                    stakeIpPatternDetailModel.LastGroupCustID++;
                }

                stakeIpPatternDetailModel.LastGroupID++;
            }

            return result;
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileAssAccountDetailRpt)]
        public ActionResult GetAssociationByOTGB(long custID1, long custID2)
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);
            var data = new AssociationOTGBViewModel();

            if (!hasPermission)
            {
                return Json(new
                {
                    errorCode = -1,
                    errorMessage = "Permission denied",
                }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                data = _accountService.GetAssociationByOTGB(custID1, custID2, AccountAssociationByOTGB.CheckExist);
            }
            catch (Exception ex)
            {
                ex.LogError();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAssociationDetailByOTGB(long custID1, long custID2, AccountAssociationByOTGB fraudType)
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);
            var data = Enumerable.Empty<AssByOTGBDetailViewModel>();

            if (!hasPermission)
            {
                return Json(new
                {
                    errorCode = -1,
                    errorMessage = "Permission denied",
                }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var association = _accountService.GetAssociationByOTGB(custID1, custID2, fraudType);

                if (association != null && association.AssociationOTGBRoun1.Any())
                {
                    var tranInfo = association.AssociationOTGBRoun1.Distinct();
                    var ticketDetails = _accountService.GetAIAssociationTicketDetails(JsonConvert.SerializeObject(tranInfo), 1000, 50);

                    data = ProcessTicketDetailOTGB(ticketDetails, fraudType);
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<AssByOTGBDetailViewModel> ProcessTicketDetailOTGB(IEnumerable<TicketDetailsModel> tickets, AccountAssociationByOTGB fraudType)
        {
            var result = new List<AssByOTGBDetailViewModel>();

            if (tickets.Any())
            {
                var groupTickets = tickets.GroupBy(c => new { c.MatchID, c.EventDate })
                                          .OrderByDescending(c => c.Key.EventDate).ThenByDescending(c => c.Key.MatchID);
                int groupId = 0;

                foreach (var group in groupTickets)
                {
                    if ((fraudType == AccountAssociationByOTGB.Round1 || fraudType == AccountAssociationByOTGB.Round3) && group.Count() < 2)
                    {
                        continue;
                    }

                    group.OrderByDescending(c => c.TransDate).ThenByDescending(c => c.TransId).ToList().ForEach(c =>
                    {
                        result.Add(new AssByOTGBDetailViewModel
                        {
                            UserName = c.UserName,
                            Currency = c.CurrencyName,
                            BetTeam = c.BetTeam,
                            BetType = c.BetType,
                            EventDate = c.EventDate,
                            IP = c.IP,
                            LiveAwayScore = c.LiveAwayScore ?? 0,
                            LiveHomeScore = c.LiveHomeScore ?? 0,
                            Stake = c.Stake,
                            Odds = c.Odds,
                            MatchID = c.MatchID,
                            TransDate = c.TransDate,
                            TransId = c.TransId.ToString(),
                            GroupID = groupId
                        });
                    });

                    groupId++;
                }
                ;
            }

            return result;
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileAssAccountDetailRpt)]
        public ActionResult GetAssByIPType(long custID1, long custID2)
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);

            if (!hasPermission)
            {
                return Json(new
                {
                    errorCode = -1,
                    errorMessage = "Permission denied",
                }, JsonRequestBehavior.AllowGet);
            }

            var data = _accountService.GetAssociationByIPType(custID1, custID2).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileAssAccountDetailRpt)]
        public ActionResult GetIPMatchBettype(long custId1, long custId2)
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);

            var data = new AssociationByIPInfoViewModel();
            var para1 = custId1;
            var para2 = custId2;

            if (!hasPermission)
            {
                data.ErrorMessage = "Permission denied.";
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            if (custId1 > custId2)
            {
                para1 = custId2;
                para2 = custId1;
            }

            var info = _accountService.GetAssociationByIPInfo(para1, para2);

            if (info.ItemList != null && info.ItemList.Any())
            {
                var matchIds = string.Join(",", info.ItemList.Select(x => x.MatchID).Distinct());

                var matchInfos = _accountService.GetMatchInfosByIDs(matchIds);

                data.ErrorMessage = info.ErrorMessage;

                data.IPMatchBettypeList = info.ItemList.Select(x =>
                {
                    var homeName = CachedRef.GetTeamName(x.HomeID);
                    var awayName = CachedRef.GetTeamName(x.AwayID);
                    var leagueName = CachedRef.GetLeagueName(x.LeagueID);
                    var betTypeList = new List<string>();
                    var ip = x.IP != null ? x.IP.Replace(",", ", ") : "-";
                    var eventDateStr = "-";

                    if (x.BettypeList != null)
                    {
                        var arrBetType = x.BettypeList.Split(',');

                        foreach (var item in arrBetType)
                        {
                            int.TryParse(item, out int betTypeId);

                            betTypeList.Add(CachedRef.GetBettypeName(betTypeId));
                        }
                    }

                    var match = matchInfos.Where(y => y.MatchID == x.MatchID).FirstOrDefault();

                    if (match != null && match.EventDate != DateTime.MinValue)
                        eventDateStr = match.EventDate.ToString(Consts.FormatDateToString_MMMddyyyy);

                    return new AssByIPMatchBettypeViewModel
                    {
                        MatchID = x.MatchID,
                        EventName = homeName != string.Empty ? (homeName + " - vs - " + awayName) : "-",
                        LeagueName = leagueName != string.Empty ? leagueName : "-",
                        EventDateString = eventDateStr,
                        BetTypes = string.Join(", ", betTypeList),
                        IP = ip
                    };
                });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileAssAccountDetailRpt)]
        public ActionResult GetIPTicketStake(long custID1, long custID2)
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);

            var result = Enumerable.Empty<AssByIPTicketStakeViewModel>();

            if (!hasPermission)
            {
                return Json(new
                {
                    result = Enumerable.Empty<AssByIPTicketStakeViewModel>(),
                    errorCode = -1,
                    errorMessage = "Permission denied"
                }, JsonRequestBehavior.AllowGet);
            }

            var data = _accountService.GetAssociationBy5GIPDetail(custID1, custID2);

            if (data != null && data.Any())
            {
                var transIDs = string.Join(",", data.Select(x => x.TransID).Distinct());

                var mainTickets = _commonService.GetMainTicketDetails(transIDs, null, null);

                if (mainTickets != null && mainTickets.Any())
                {
                    result = RearrangeAssByIPTicketStake(mainTickets);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<AssByIPTicketStakeViewModel> RearrangeAssByIPTicketStake(IEnumerable<MainTicketDetailModel> mainTickets)
        {
            var assList = new List<AssByIPTicketStakeViewModel>();

            var groups = mainTickets.GroupBy(x => x.IP);

            foreach (var group in groups)
            {
                var items = group.Select(x => new AssByIPTicketStakeItemViewModel(x))
                        .OrderByDescending(x => x.EventDate)
                        .ThenByDescending(x => x.MatchID)
                        .ThenByDescending(x => x.TransDate)
                        .ThenByDescending(x => x.TransID);

                var maxEventDateInGroup = items.Max(x => x.EventDate);

                assList.Add(new AssByIPTicketStakeViewModel
                {
                    IP = group.Key,
                    MaxEventDateInGroup = maxEventDateInGroup,
                    Items = items
                });
            }

            return assList.OrderByDescending(x => x.MaxEventDateInGroup);
        }
        #endregion Associated By Account

        #region Chain To Evidence
        //[NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult ChainToEvidence()
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);

            if (!hasPermission)
                return new EmptyResult();

            return PartialView("ChainToEvidence/ChainToEvidence");
        }

        [HttpGet]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult GetEvidenceList()
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);

            if (!hasPermission)
                return new EmptyResult();

            var result = _accountService.GetEvidenceList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileChainstoEvidencesRpt)]
        public ActionResult TrackingEvidence()
        {
            return null;
        }

        [HttpPost]
        public ActionResult GetAccountEvidence(Int64 ctsCustId)
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);
            var result = new AccountEvidenceViewModel();

            if (!hasPermission) return Json(result, JsonRequestBehavior.AllowGet);

            var evidenceCodeList = _accountService.GetEvidenceList().Select(x => x.EvidenceCode);
            var evidenceCodes = string.Join(",", evidenceCodeList);

            var tasks = new List<Task>();
            var evidenceList = new List<EvidenceViewModel>();
            var flaggedEvidences = new List<FlaggedEvidenceModel>();
            tasks.Add(Task.Run(() => flaggedEvidences = _accountService.GetFlaggedEvidences(ctsCustId, evidenceCodes).ToList()));

            var affectedRetractEvidences = new AffectedRetractEvidenceModel();
            tasks.Add(Task.Run(() => affectedRetractEvidences = _accountService.GetAffectedRetractEvidences(ctsCustId, evidenceCodes)));

            Task.WhenAll(tasks.ToArray()).Wait();

            if (flaggedEvidences != null && flaggedEvidences.Any())
            {
                result.FlaggedEvidenceList = flaggedEvidences.OrderBy(x => x.EvidenceID).Select(x => new FlaggedEvidenceViewModel(x)).ToList();

                evidenceList.AddRange(result.FlaggedEvidenceList.Select(x => new EvidenceViewModel(x)));
            }

            if (affectedRetractEvidences.AffectedEvidenceList != null && affectedRetractEvidences.AffectedEvidenceList.Any())
            {
                result.AffectedRetractEvidenceList.AddRange(affectedRetractEvidences.AffectedEvidenceList.OrderBy(x => x.EvidenceID).Select(x => new AffectedRetractEvidenceViewModel(x, false)));
            }

            if (affectedRetractEvidences.RetractEvidenceList != null && affectedRetractEvidences.RetractEvidenceList.Any())
            {
                result.AffectedRetractEvidenceList.AddRange(affectedRetractEvidences.RetractEvidenceList.OrderBy(x => x.EvidenceID).Select(x => new AffectedRetractEvidenceViewModel(x, true)));
            }

            evidenceList.AddRange(result.AffectedRetractEvidenceList.Select(x => new EvidenceViewModel(x)));

            result.EvidenceList.AddRange(evidenceList.Distinct(x => x.EvidenceCode).OrderBy(x => x.EvidenceID));

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetExceptions(Int64 ctsCustId)
        {
            var result = new List<AccountExceptionsViewModel>();

            var exceptions = _accountService.GetAccountExceptions(ctsCustId);

            if (exceptions != null && exceptions.Any())
            {
                result = exceptions.Select(x => new AccountExceptionsViewModel
                {
                    ExceptionCTSCustId = x.ExceptionCTSCustId,
                    UserName = string.IsNullOrEmpty(x.UserName) ? x.LoginId : x.UserName,
                    LoginId = string.IsNullOrEmpty(x.LoginId) ? x.UserName : x.LoginId,
                    Comment = x.Comment,
                    Creator = x.CreatedBy,
                    CTSCustId = x.CTSCustId,
                    CustId = x.CustId,
                    ExceptionDate = x.ExceptionDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss)
                }).ToList();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileChainstoEvidenceInsertExceptions)]
        public ActionResult AddException(UpdateAccountExceptionModel model)
        {
            model.UserId = CurrentSession.UserId;

            var result = _accountService.AddAccountException(model);

            if (result.ErrorCode == 0)
            {
                string ctsCustIdsToString = model.FromCTSCustId.ToString();

                var toCTSCustId = (UInt64)result.OutParam["toCTSCustId"];

                if (toCTSCustId != 0)
                {
                    ctsCustIdsToString += "," + toCTSCustId.ToString();
                }

                var custIdsFlaggedEvidences = _customerEvidenceService.CheckCleanCustFlaggedEvidence(ctsCustIdsToString);

                if (custIdsFlaggedEvidences != null && custIdsFlaggedEvidences.Any())
                {
                    _problemAccountClassificationService.UpdateBetLimitOfUnflaggedCustomer(custIdsFlaggedEvidences.Select(c => (long)c.CustID));
                }
            }

            return Json(new { errorCode = result.ErrorCode }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileChainstoEvidenceRemoveExceptions)]
        public ActionResult RemoveException(UpdateAccountExceptionModel model)
        {
            model.UserId = CurrentSession.UserId;

            var result = _accountService.RemoveAccountException(model);

            if (result == 0)
            {
                var custExistEvidence = new List<CTSCustomerEvidenceViewModel>();
                var custNotExistEvidence = new List<CTSCustomerEvidenceViewModel>();

                var tasks = new List<Task>
                {
                    Task.Run(() =>
                    {
                        custExistEvidence.Add(new CTSCustomerEvidenceViewModel { CTSCustID = model.FromCTSCustId, CustID = _accountService.GetCustomerInfoByCTSCustId(model.FromCTSCustId).CustID });
                        custExistEvidence.Add(new CTSCustomerEvidenceViewModel { CTSCustID = model.ToCTSCustId, CustID = _accountService.GetCustomerInfoByCTSCustId(model.ToCTSCustId).CustID });
                    }),
                    Task.Run(() =>
                    {
                         string ctsCustIdsToString = model.FromCTSCustId.ToString() + "," + model.ToCTSCustId.ToString();
                         custNotExistEvidence = _customerEvidenceService.CheckCleanCustFlaggedEvidence(ctsCustIdsToString)?.ToList();
                    })
                };

                Task.WhenAll(tasks.ToArray()).Wait();

                if (custNotExistEvidence != null && custNotExistEvidence.Any())
                {
                    custExistEvidence.RemoveAll(x => custNotExistEvidence.Contains(x, new CustomerInfoEqualityComparer()));
                }

                if (custExistEvidence.Any())
                {
                    _problemAccountClassificationService.UpdateBetLimitOfFlaggedCustomer(custExistEvidence.Select(c => (long)c.CustID));
                }
            }

            return Json(new
            {
                errorCode = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileChainstoEvidenceRemoveAffEvidence)]
        public ActionResult RetractAffectedEvidence(RemoveEvidenceModel model)
        {
            model.UserId = CurrentSession.UserId;

            var result = _accountService.RetractAffectedEvidence(model);

            if (result == 0)
            {
                var custIdsFlaggedEvidences = _customerEvidenceService.CheckCleanCustFlaggedEvidence(model.CTSCustId.ToString());

                if (custIdsFlaggedEvidences != null && custIdsFlaggedEvidences.Any())
                {
                    _problemAccountClassificationService.UpdateBetLimitOfUnflaggedCustomer(custIdsFlaggedEvidences.Select(c => (long)c.CustID));
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RestoreAffectedEvidence(RemoveEvidenceModel model)
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);

            model.UserId = CurrentSession.UserId;

            if (!hasPermission)
            {
                return Json(new
                {
                    result = -1, 
                    errorMessage = "Permission denied"
                }, JsonRequestBehavior.AllowGet);
            }

            var result = _accountService.RestoreAffectedEvidence(model);

            if (result == 0)
            {
                var custNotExistFlaggedEvidences = _customerEvidenceService.CheckCleanCustFlaggedEvidence(model.CTSCustId.ToString());

                if (custNotExistFlaggedEvidences != null && !custNotExistFlaggedEvidences.Any())
                {
                    var custId = _accountService.GetCustomerInfoByCTSCustId(model.CTSCustId).CustID;

                    _problemAccountClassificationService.UpdateBetLimitOfFlaggedCustomer(new List<long> { custId });
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetAffectedEvidenceListBetweenCusts(Int64 fromCTSCustId, Int64 toCTSCustId)
        {
            var result = new List<AffectedEvidenceListBetweenCustsViewModel>();
            var evidences = _accountService.GetAffectedEvidenceListBetweenCusts(fromCTSCustId, toCTSCustId);
            if (evidences != null && evidences.Any())
            {
                result = evidences.OrderBy(x => x.EvidenceCode).Select(x => new AffectedEvidenceListBetweenCustsViewModel
                {
                    EvidenceDisplay = x.EvidenceCode + " " + x.EvidenceName
                }).ToList();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion Chain To Evidence

        #region Transaciton History
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult TransactionHistory()
        {
            return PartialView("TransactionHistory/TransactionHistory");
        }

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileTransactionHistory)]
        public ActionResult TrackingTransactionHistory()
        {
            return null;
        }

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileTransactionHistory, "isSkipTracking")]
        public ActionResult GetTransactionHistory(DataTableAjaxPostModel model, bool? IsSkipTracking)
        {
            IUserService userService = new UserService();
            var isViewIPPermission = userService.IsHavingPermission(CurrentSession.UserId, NapSettings.Settings.SiteId, FSFunctionKeys.ViewIP, NAP.Core.Business.Constant.Policy.Read);

            var take = model.length;
            var skip = model.start;
            var totalItem = 0;

            var data = _transService.GetTransactionHistory(
                model.CTSCustId,
                model.DateRange,
                take,
                skip,
                out totalItem);

            var result = data.Select(x => new TransactionHistoryViewModel
            {
                TransId = x.TransId.ToString(),
                Action = x.Action,
                ActionResult = x.ActionResult,
                BrowserName = x.Browser,
                DeviceId = x.FirstDeviceCode,
                Flagged = string.IsNullOrEmpty(x.Flagged) ? "-" : x.Flagged,
                Ip = isViewIPPermission ? x.Ip : string.Empty,
                IpId = 0,
                OSName = x.OS,
                TransTime = x.TransTime.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                Url = x.UrlDetails
            });

            return Json(new
            {
                draw = model.draw,
                recordsTotal = totalItem,
                recordsFiltered = totalItem,
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion Transaciton History

        #region Customer Category
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassification)]
        public ActionResult TrackingCustomerCategory()
        {
            return null;
        }

        [HttpPost]
        public ActionResult GetCustomerClassification(long ctsCustID, ROLE_CUSTOMER roleID)
        {
            IUserService userService = new UserService();
            var isVVip = false;
            var isProblemAccount = false;
            var isMultiplePA = false;
            var isExcludedTagAssPA = false;
            var categoryList = new List<GetCustomerCategoryViewModel>();
            var classificationHistoryList = new List<CustomerClassificationHistoryViewModel>();
            var usnBackGroundColor = EnumExtensions.GetStringSlow(CategoryColorType.Default);
            var hasPermission = HasAccountInfoAndDetailPermission(userService);

            if (!hasPermission)
                return Json(new
                {
                    isExcludedTagAssPA,
                    isVVip,
                    isProblemAccount,
                    isMultiplePA,
                    categoryList,
                    usnBackGroundColor,
                    data = classificationHistoryList,
                }, JsonRequestBehavior.AllowGet);

            var classification = _accountService.GetCustomerClassification(ctsCustID, roleID, out isExcludedTagAssPA);

            if (classification != null && classification.Any())
            {
                categoryList = classification.Select(x => new GetCustomerCategoryViewModel(x)).ToList();

                isVVip = categoryList.Any(x => x.IsVVIP);
                isProblemAccount = categoryList.Any(x => x.IsProblemAccount);
                isMultiplePA = categoryList.Any(x => x.IsMultiplePA);
                int minPriority = classification.Min(c => c.CategoryPriority);
                usnBackGroundColor = EnumExtensions.GetStringSlow((CategoryColorType)classification.FirstOrDefault(x => x.CategoryPriority == minPriority).ColorTypeID);
            }

            var classificationHistory = _accountService.GetCustomerClassificationHistory(ctsCustID, roleID);

            if (classificationHistory != null && classificationHistory.Any())
            {
                classificationHistoryList = classificationHistory.Select(x => new CustomerClassificationHistoryViewModel(x)).ToList();
            }

            return Json(new
            {
                isExcludedTagAssPA,
                isVVip,
                isProblemAccount,
                isMultiplePA,
                categoryList,
                usnBackGroundColor,
                data = classificationHistoryList,
            }, JsonRequestBehavior.AllowGet);
        }

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassificationNormalCategory)]
        [HttpPost]
        public ActionResult ChangeExcludedTagAssPA(int custId, long ctsCustId, int subscriberId, bool isExcludedTagAssPA, bool isProblemAccount, int custSubId, int roleID)
        {
            if (roleID > (int)RoleGroupType.Member)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var result = _accountService.ChangeExcludedTagAssPA(ctsCustId, subscriberId, isExcludedTagAssPA, CurrentSession.UserId);

                if (result.ErrorCode == 0
                    && !isProblemAccount
                    && custSubId == 0)
                {
                    _rescanClassificationService.RescanCustomerClassification(new RescanCustClassificationModel
                    {
                        CustIds = new List<long> { custId },
                        FunctionId = FunctionId.ExcludeTaggingAssPAWeb,
                        ServiceName = "ExcludeTaggingAssociationWithPA"
                    });
                }

                return Json(new
                {
                    errorCode = result.ErrorCode,
                    errorMessage = result.ErrorMsg
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogExtension.LogError($"CTS - ExcludeTaggingAssociationWithPA exception: {ex.Message}", ex);

                return Json(new { errorCode = -1, errorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult MarkCustomerCategory(long ctsCustID, ROLE_CUSTOMER roleID, string userName, bool isRobotUser)
        {
            var problemAccounts = _pamService.GetProblemAccountCategories(ViewCategoryQueryType.ProfilePage_PA);

            problemAccounts = problemAccounts.Where(x => x.MapRole(roleID)).OrderBy(x => x.Priorty);

            var result = new MarkCustomerCategoryViewModel
            {
                RoleID = (int)roleID,
                UserName = userName,
                IsRobotUser = isRobotUser,
                ProblemAccountList = problemAccounts
            };

            var classifications = _accountService.GetCustomerClassification(ctsCustID, roleID, out bool isExcludedTagAssPA);

            if (classifications != null && classifications.Any())
            {
                result.CategoryList = classifications.Select(x => new GetCustomerCategoryViewModel(x));
            }

            return PartialView("CustomerCategory/MarkCustomerCategory", result);
        }

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassificationMarkCategory)]
        public ActionResult SaveMarkVVIP(SaveMarkCustomerCategoryViewModel model)
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);
            var result = new DBResult();

            if (!hasPermission)
                return Json(new
                {
                    errorCode = result.ErrorCode,
                    errorMessage = result.ErrorMsg
                }, JsonRequestBehavior.AllowGet);

            var categoryModel = new CustomerCategoryModel
            {
                CustId = model.CustID,
                CTSCustId = model.CTSCustID,
                SubscriberId = model.SubscriberID,
                RoleId = model.RoleID,
                IsAffectToAllDownlines = model.IsAffectAllDownlines,
                Remark = model.Remark,
                IsVVip = model.IsVVip,
                CreatedBy = CurrentSession.UserId,
                ActionType = ActionTypeQueueCustCategory.Insert
            };

            categoryModel.CategoryId = _pamService.GetVvipCategoryID(model.RoleID);

            var isSuccess = MarkDirectVVIP(categoryModel);

            if (isSuccess)
            {
                if (categoryModel.RoleId != ROLE_CUSTOMER.MEMBER && categoryModel.IsAffectToAllDownlines)
                {
                    result = _accountService.AddQueueAffectDownlinesCustomerCategory(categoryModel);
                }
            }
            else
            {
                result = new DBResult { ErrorCode = -1, ErrorMsg = "Mark VVIP failed" };
            }

            return Json(new
            {
                errorCode = result.ErrorCode,
                errorMessage = result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        private bool MarkDirectVVIP(CustomerCategoryModel categoryModel)
        {
            try
            {
                var latestClassifications = MarkVVIP(categoryModel);

                if (latestClassifications.Any())
                {
                    // Update CC in table custinfo to MainDB
                    var roleGroupType = categoryModel.RoleId == ROLE_CUSTOMER.MEMBER ? RoleGroupType.Member : RoleGroupType.Agency;

                    _customerClassificationService.RetryPushCCToMainDB(latestClassifications, roleGroupType, 2);

                    Task.Run(() =>
                    {
                        _antiFraudCenterService.DeleteAFCAccounts(latestClassifications.Select(x => x.CustId), FromFlow.CTSWEB);
                    });
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
                return false;
            }

            return true;
        }

        private IEnumerable<LatestClassificationModel> MarkVVIP(CustomerCategoryModel categoryModel)
        {
            if (categoryModel.RoleId == ROLE_CUSTOMER.MEMBER)
            {
                var memberMarkResult = _customerCategoryService.MarkVVIPCustomerCategory(categoryModel);

                return memberMarkResult.CustInfos.Select(x => new LatestClassificationModel
                {
                    CustId = x.CustID,
                    CustomerClass = x.CustomerClass,
                    Category = x.CategoryID
                });
            }
            else
            {
                var agencyMarkResult = _customerCategoryAgencyService.MarkVVIPCustomerCategory(categoryModel);

                return agencyMarkResult.CustInfos.Select(x => new LatestClassificationModel
                {
                    CustId = x.CustID,
                    CustomerClass = x.CustomerClass,
                    Category = x.CategoryID
                });
            }
        }

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassificationMarkCategory)]
        public ActionResult SaveMarkCustomerCategory(SaveMarkCustomerCategoryViewModel model)
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);
            var result = new DBResult();

            if (!hasPermission)
                return Json(new
                {
                    errorCode = result.ErrorCode,
                    errorMessage = result.ErrorMsg
                }, JsonRequestBehavior.AllowGet);

            var pas = new List<InsertProblemAccountModel>()
            {
                new InsertProblemAccountModel
                {
                    CustID = model.CustID,
                    CTSCustID = model.CTSCustID,
                    RoleID = (int)model.RoleID,
                    SubscriberID = model.SubscriberID,
                    IsLicensee = model.IsLicensee,
                    CategoryID = model.CategoryID,
                    Remark = model.Remark,
                    IsFromAI = false,
                    IsMarkedDirectly = true,
                    IsFromCTS = true,
                    CreatedBy = CurrentSession.UserId
                }
            };

            result = model.RoleID == ROLE_CUSTOMER.MEMBER
                ? _problemAccountClassificationService.InsertProblemAccountPackage(pas, CCInputFlow.InsertPACategory, "ProfileAddProblemAccount", FromFlow.CTSWEB, !model.IsRobot)
                : _problemAccountClassificationAgencyService.InsertProblemAccountAgencyPackage(pas, AgencyCCInputFlow.InsertPACategory, "ProfileAddProblemAccount", FromFlow.CTSWEB, !model.IsRobot);

            return Json(new
            {
                errorCode = result.ErrorCode,
                errorMessage = result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassificationRemoveCategory)]
        public ActionResult UnmarkCustomerCategory(int roleID, string userName, bool isVVIP, bool isLicensee)
        {
            var model = new UnmarkCustomerCategoryViewModel
            {
                RoleID = roleID,
                UserName = userName,
                IsVVip = isVVIP,
                IsLicensee = isLicensee
            };

            return PartialView("CustomerCategory/UnmarkCustomerCategory", model);
        }

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassificationRemoveCategory)]
        public ActionResult SaveUnmarkVVIP(CustomerCategoryModel model)
        {
            try
            {
                IUserService userService = new UserService();
                var hasPermission = HasAccountInfoAndDetailPermission(userService);
                var result = new DBResult();

                if (hasPermission)
                {
                    model.CreatedBy = CurrentSession.UserId;
                    model.ActionType = ActionTypeQueueCustCategory.Remove;

                    result.ErrorMsg = model.RoleId == ROLE_CUSTOMER.MEMBER ? UnmarkAndRescanVVIP(model) : UnmarkAndRescanVVIPAgency(model);
                }

                return Json(new
                {
                    errorCode = hasPermission ? (string.IsNullOrEmpty(result.ErrorMsg) ? 0 : -1) : -1,
                    errorMessage = result.ErrorMsg
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogExtension.LogError($"CTS - Unmark VVIP exception: {ex.Message}", ex);

                dataLogTrackingServices.LogErrorData(ex, "WEB-UnmarkVVIP");

                return Json(new { errorCode = -1, errorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private string UnmarkAndRescanVVIPAgency(CustomerCategoryModel model)
        {
            var resultUmmark = _customerCategoryAgencyService.UnmarkVVIPCustomerCategory(model);

            if (resultUmmark.CustInfos.IsNotNullOrEmpty())
            {
                if (model.IsAffectToAllDownlines)
                {
                    _accountService.AddQueueAffectDownlinesCustomerCategory(model);
                }

                if (model.CustId != 0)
                {
                    _rescanClassificationAgencyService.RescanCustomerClassificationAgency(new RescanCustClassificationAgencyModel
                    {
                        CustIds = new List<long> { model.CustId },
                        IsToQueue = !model.IsAffectToAllDownlines, // add to queue for case SMA and not checking affect to downline to affect PA for downline                          
                        IsUpdateDangerToMainDB = false, // sma not update danger
                        FunctionId = FunctionId.UnmarkCustomerCategory,
                        ServiceName = "RemoveCustomerCategoryAgency"
                    });
                }
            }

            return resultUmmark.ErrorMessage;
        }

        private string UnmarkAndRescanVVIP(CustomerCategoryModel model)
        {
            var resultUmmark = _customerCategoryService.UnmarkVVIPCustomerCategory(model);

            if (resultUmmark.CustInfos.IsNotNullOrEmpty())
            {
                _rescanClassificationService.RescanCustomerClassification(new RescanCustClassificationModel
                {
                    CustIds = new List<long> { model.CustId },
                    IsToQueue = false,
                    IsUpdateDangerToMainDB = true,
                    FunctionId = FunctionId.UnmarkCustomerCategory,
                    ServiceName = "RemoveCustomerCategory"
                });
            }

            return resultUmmark.ErrorMessage;
        }

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassificationRemoveCategory)]
        public ActionResult SaveUnmarkCustomerCategory(SaveUnmarkCustomerCategoryViewModel model)
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);
            string messageResult = null;

            if (hasPermission)
            {
                messageResult = model.RoleID == (int)ROLE_CUSTOMER.MEMBER ? UnmarkAndRescanProblemAccount(model) : UnmarkAndRescanProblemAccountAgency(model);
            }

            return Json(new
            {
                errorCode = hasPermission ? (string.IsNullOrEmpty(messageResult) ? 0 : -1) : -1,
                errorMessage = messageResult
            }, JsonRequestBehavior.AllowGet);
        }

        private string UnmarkAndRescanProblemAccountAgency(SaveUnmarkCustomerCategoryViewModel model)
        {
            var resultUnmark = _problemAccountClassificationAgencyService.UnmarkProblemAccountAgency(new List<UnmarkProblemAccountAgencyModel>()
                {
                    new UnmarkProblemAccountAgencyModel()
                    {
                        CustID = model.CustID,
                        CTSCustID = model.CTSCustID,
                        RoleID = model.RoleID,
                        SubscriberID = model.SubscriberID,
                        CreatedBy = CurrentSession.UserId,
                        Remark = model.Remark,
                        IsExceptDirectDownline = model.IsExceptDirectDownline,
                        IsMarkedDirectly = true
                    }
                }, false);

            if (resultUnmark.CustInfos.IsNotNullOrEmpty())
            {
                _rescanClassificationAgencyService.RescanCustomerClassificationAgency(new RescanCustClassificationAgencyModel
                {
                    CustIds = resultUnmark.CustInfos.Select(x => x.CustID),
                    FunctionId = FunctionId.UnmarkCustomerCategory,
                    IsUpdateDangerToMainDB = false // sma not update danger
                });
            }

            return resultUnmark.ErrorMessage;
        }

        private string UnmarkAndRescanProblemAccount(SaveUnmarkCustomerCategoryViewModel model)
        {
            var resultUnmark = _problemAccountClassificationService.UnmarkProblemAccount(new List<UnmarkProblemAccountModel>()
                {
                    new UnmarkProblemAccountModel()
                    {
                        CustID = model.CustID,
                        CTSCustID = model.CTSCustID,
                        RoleID = model.RoleID,
                        SubscriberID = model.SubscriberID,
                        CreatedBy = CurrentSession.UserId,
                        Remark = model.Remark,
                        IsExceptDirectDownline = model.IsExceptDirectDownline,
                        IsMarkedDirectly = true
                    }
                }, false);

            if (resultUnmark.CustInfos.IsNotNullOrEmpty())
            {
                _rescanClassificationService.RescanCustomerClassification(new RescanCustClassificationModel
                {
                    CustIds = resultUnmark.CustInfos.Select(x => x.CustID),
                    FunctionId = FunctionId.UnmarkCustomerCategory,
                    IsUpdateDangerToMainDB = true
                });
            }

            return resultUnmark.ErrorMessage;
        }

        #endregion Customer Category

        #region Robot Betting
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult RobotBetting()
        {
            return PartialView("RobotBetting/RobotBetting");
        }

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileRobotBetting)]
        public ActionResult TrackingRobotBetting()
        {
            return null;
        }

        [HttpPost]
        public ActionResult GetRobotBetting(long custID)
        {
            IUserService userService = new UserService();
            var hasPermission = HasAccountInfoAndDetailPermission(userService);

            if (!hasPermission)
            {
                return Json(new
                {
                    data = (RobotBettingViewModel)null,
                    errorCode = -1,
                    errorMessage = "Permission denied"
                }, JsonRequestBehavior.AllowGet);
            }

            var rbData = _robotBettingService.GetRobotBetting(custID);
            var rbBetting = new RobotBettingViewModel(rbData);

            return Json(new
            {
                data = rbBetting,
                errorCode = 0,
                errorMessage = string.Empty
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.GetRobotAILPDetail)]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetRobotAILPDetail(long custID, int rangeType)
        {
            var rbAiLpData = _robotBettingService.GetRobotAiLpDetail(custID, rangeType);

            var rbAILPDetail = rbAiLpData.Select(x => new RobotAiLpDetailViewModel(x));

            return Json(new
            {
                data = rbAILPDetail,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetRobotBettingTicketDetail(long custID, int robotTypeOrigin, int? robotTypeDisplay, int? clusterId, string createdTime)
        {
            var result = Enumerable.Empty<RobotBettingDetailViewModel>();

            try
            {
                var listTrans = _robotBettingService.GetTranIds(custID, robotTypeOrigin, robotTypeDisplay, clusterId, createdTime);

                if (listTrans != null && listTrans.Any())
                {
                    var transToString = _robotBettingService.GetTransFromList(listTrans);

                    var isSCETicketDetails = (robotTypeOrigin == (int)RobotPeriodRangeType.SCEConsecutive
                                            || robotTypeOrigin == (int)RobotPeriodRangeType.SCEUglyStake) ? true : false;

                    var ticketDetail = _robotBettingService.GetRobotBettingTicketDetails(transToString, isSCETicketDetails);

                    if (ticketDetail != null && ticketDetail.Any())
                    {
                        result = ticketDetail.OrderBy(x => x.TransDate).Select(x => new RobotBettingDetailViewModel
                        {
                            TransDate = x.TransDate.ToString(Consts.FormatDatetimeToString_ddMMMyyyyHHmmssfff),
                            HomeID = x.HomeId,
                            AwayID = x.AwayId,
                            TransID = x.TransId.ToString(),
                            BetType = x.BetType,
                            BetTypeName = CachedRef.GetBettypeName(x.BetType),
                            BetTeam = x.BetTeam,
                            Stake = x.Stake,
                            Odds = x.Odds,
                            LiveScore = x.LiveIndicator ? (x.LiveHomeScore + " - " + x.LiveAwayScore) : "-",
                            MatchName = GetMatchName(x)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogExtension.LogError(ex.Message);

                result = Enumerable.Empty<RobotBettingDetailViewModel>();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetRobotBettingReasonDetail(long custID, int robotTypeOrigin)
        {
            var inputParas = new RobotBettingSceDetailParaModel { CustId = custID, RobotTypeOrigin = robotTypeOrigin, QueryType = RobotDetectionQueryType.RobotReasonDetail };
            var robotReasonDetail = _robotBettingService.GetRobotDetectionSCEDetails(inputParas);

            return Json(new
            {
                childTableId = new Random().Next(),
                reasonGroups = robotReasonDetail?.ToList().OrderBy(c => c.FirstTransDate) ?? Enumerable.Empty<RobotUserDetailSceModel>()
            }, JsonRequestBehavior.AllowGet);
        }

        public string GetMatchName(RobotBettingTicketDetailModel ticket)
        {
            var rs = "-";

            var HomeName = CachedRef.GetTeamName(ticket.HomeId);
            var AwayName = CachedRef.GetTeamName(ticket.AwayId);

            if (!string.IsNullOrEmpty(HomeName) || !string.IsNullOrEmpty(AwayName))
            {
                rs = HomeName + " vs " + AwayName;
            }

            return rs;
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetRobotRDMatch(RobotPeriodRangeType rangeType, long custID)
        {
            var rdMatches = Enumerable.Empty<RobotRDMatchViewModel>();


            switch (rangeType)
            {
                case RobotPeriodRangeType.OCRD:
                case RobotPeriodRangeType.TD:
                    {
                        var robotMatches = _robotBettingService.GetRobotRDMatches((int)rangeType, custID);

                        if (robotMatches.Any())
                        {
                            var matchIDs = string.Join(",", robotMatches.Select(x => x.MatchID).Distinct());

                            var mainMatches = _commonService.GetMainMatchInfosByIDs(matchIDs);

                            rdMatches = mainMatches.Join(robotMatches, m => m.MatchID, r => r.MatchID,
                                                     (m, r) => new RobotRDMatchViewModel(m, r.TotalTickets))
                                                     .OrderByDescending(x => x.EventDate)
                                                     .ThenByDescending(x => x.TotalTicket)
                                                     .ThenByDescending(x => x.MatchID).Take(50);
                        }
                        break;
                    }
            }

            return Json(new { data = rdMatches }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetailTab, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetRobotRDTicketDetail(RobotPeriodRangeType rangeType, long custID, long matchID)
        {
            var rdTickets = Enumerable.Empty<RobotRDTicketViewModel>();
            var matchSize = 1;
            var tranSize = 1000;

            switch (rangeType)
            {
                case RobotPeriodRangeType.OCRD:
                case RobotPeriodRangeType.TD:
                    {
                        var rdTrans = _robotBettingService.GetRobotRDTrans((int)rangeType, custID, matchID);

                        if (rdTrans.Any())
                        {
                            var tranInfos = JsonConvert.SerializeObject(rdTrans.Distinct(x => x.TransID).Select(x => new RobotRDTicketDetailViewModel
                            {
                                TransID = x.TransID,
                                MatchID = x.MatchID
                            }));

                            var tickets = _commonService.GetMainTicketDetails(tranInfos, tranSize, matchSize);

                            rdTickets = tickets.OrderByDescending(x => x.TransDate).ThenByDescending(x => x.TransId)
                                .Select(x => new RobotRDTicketViewModel(x));
                        }
                        break;
                    }
            }

            return Json(new { data = rdTickets }, JsonRequestBehavior.AllowGet);
        }
        #endregion Robot Betting

        private bool HasAccountInfoAndDetailPermission(IUserService userService)
        {
            var userId = CurrentSession.UserId;
            var siteId = NapSettings.Settings.SiteId;

            return userService.IsHavingPermission(userId, siteId, FSFunctionKeys.AccountInfo, NAP.Core.Business.Constant.Policy.Read)
                   || userService.IsHavingPermission(userId, siteId, FSFunctionKeys.AccountDetailTab, NAP.Core.Business.Constant.Policy.Read);
        }
        #endregion
    }
}