namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.Account.AssociatedAccount;
    using Fanex.NAP.CTS.Model.Account.RobotBetting;
    using Fanex.NAP.CTS.Model.Association;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassification.ProblemAccount;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Model.Transaction;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
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
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IGraphService _graphService;
        private readonly ITransactionService _transService;
        private readonly ICustomerEvidenceService _customerEvidenceService;
        private readonly IRescanCustomerClassificationService _rescanClassificationService;
        private readonly ICustomerClassificationService _customerClassificationService;
        private readonly ICommonService _commonService;
        private readonly IRobotClassificationService _robotClassificationService;
        private readonly IRobotBettingService _robotBettingService;
        private readonly IAntiFraudCenterService _antiFraudCenterService;
        private readonly IProblemAccountManagementService _pamService;
        private readonly IProblemAccountClassificationService _problemAccountClassificationService;
        private readonly IDataLogTrackingServices dataLogTrackingServices;

        public override string BaseControllerName { get => string.Empty; }

        public AccountController()
        {
            _accountService = new AccountService();
            _graphService = new GraphService();
            _transService = new TransactionService();
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
        }

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileRpt)]
        public ActionResult Index(long Id)
        {
            if (Id > 0)
            {
                IUserService userService = new UserService();

                var customerInfoFromCTS = _accountService.GetCustomerInfoByCTSCustId(Id);

                if (customerInfoFromCTS != null)
                {
                    var customerGeneralStatusViewModel = new CustomerGeneralStatusViewModel()
                    {
                        CTSCustId = Id,
                        IsHostPermission = userService.IsHavingPermission(CurrentSession.UserId, NapSettings.Settings.SiteId, FSFunctionKeys.AccountDetail, NAP.Core.Business.Constant.Policy.Host),
                        IsBettingStatisticPermission = CheckBettingStatisticPermission(userService, customerInfoFromCTS.RoleID),
                        IsViewIPTransactionPermission = userService.IsHavingPermission(CurrentSession.UserId, NapSettings.Settings.SiteId, FSFunctionKeys.ViewIP, NAP.Core.Business.Constant.Policy.Read),
                        CurrentUserId = CurrentSession.UserId
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
                            var logMessage = new LogMessage(ex.Message)
                                  .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                                  .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                            LogService.Error(logMessage);

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
                            var logMessage = new LogMessage(ex.Message)
                                  .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                                  .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                            LogService.Error(logMessage);

                            throw;
                        }
                    });

                    return View(customerGeneralStatusViewModel);
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


        [HttpPost]
        public ActionResult GetStatusRobotClassification(int custID)
        {
            var result = _robotClassificationService.GetStatusRobotClassification(custID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
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
        public ActionResult GetCustomerFlagInfo(long ctsCustId)
        {
            var result = _accountService.GetCustomerFlagInfo(ctsCustId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
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
            var customerClassification = string.Empty;

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

        #region Customer Profile

        [HttpPost]
        public ActionResult GetCustProfileAssociationAccount(long ctsCustId)
        {
            var getAssociationFromMySQL = AppSettings.Get("GetAssociationProfileFromMySQL") == "1";

            var data = _accountService.GetCustomerInfoAssociationAccount(ctsCustId, getAssociationFromMySQL);

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
                    var logMessage = new LogMessage(ex.Message)
                          .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                          .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error(logMessage);

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
                    var logMessage = new LogMessage(ex.Message)
                          .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                          .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error(logMessage);

                    throw;
                }
            });

            return Json(custProfileEvidence, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCustProfileLoginInfo(long ctsCustId)
        {
            var fromDate = DateTime.Now.AddDays(-90);
            var toDate = DateTime.Now;

            var data = _accountService.GetCustomerInfoLoginStats(ctsCustId, fromDate, toDate);

            if (data != null)
            {
                var custProfileLoginStats = new CustomerInfoLoginStatsViewModel
                {
                    DesktopUsage = data.DesktopUsage,
                    DesktopUsagePercent = data.DesktopUsagePercent.ToString("0.00"),
                    MobileUsage = data.MobileUsage,
                    MobileUsagePercent = data.MobileUsagePercent.ToString("0.00"),
                    OthersUsage = data.OthersUsage,
                    OthersUsagePercent = data.OthersUsagePercent.ToString("0.00"),
                    LoginCount = data.Logins,
                    IPCount = data.IPs,
                    DeviceCount = data.Devices
                };

                return Json(custProfileLoginStats, JsonRequestBehavior.AllowGet);
            }

            return Json(new CustomerInfoLoginStatsViewModel(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCustProfileDates(long ctsCustId)
        {
            var data = _accountService.GetCustProfileDates(ctsCustId);

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

        #endregion Customer Profile

        #region Associated By Account

        public ActionResult AssociatedByAccount(string username)
        {
            var getAssociationStatus = _accountService.GetCustomerStaticStatus(3, true);

            getAssociationStatus.Insert(0, new CustomerStaticStatusModel { ItemId = 0, ItemNameDisplay = "All" });

            var getCustomerStaticAssType = _accountService.GetCustomerStaticStatus(2, true);

            getCustomerStaticAssType.Insert(0, new CustomerStaticStatusModel { ItemId = 0, ItemNameDisplay = "All" });

            var accountStatus = _accountService.GetAccountStaticStatuses(1, true);

            var subscribers = _commonService.GetSubscriberGroups();

            var categories = GetAllCategories();

            var associatedByAccountViewModel = new AssociatedByAccountViewModel
            {
                AssociationStatus = getAssociationStatus,
                AssociationTypeList = getCustomerStaticAssType,
                AccountStatusList = accountStatus,
                SubscribersList = subscribers.ToList(),
                CategoryList = categories.ToList(),
                Username = username
            };

            return PartialView("Associated/AccountByAccount", associatedByAccountViewModel);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileAssAccountRpt, "isSkipTracking")]
        public ActionResult GetAssociatedByAccount(AssociatedFilterModel model, bool? isSkipTracking)
        {
            var totalItem = 0;

            var result = new List<CustomerAssociationByAccountDetailViewModel>();

            if (model.CTSCustId == 0)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var getAssociationFromMySql = AppSettings.Get("GetAssociationProfileFromMySQL") == "1";
            var graphAssociationPage = _accountService.GetAssociatedAccount(model, getAssociationFromMySql);

            if (graphAssociationPage != null && graphAssociationPage.AssociationList != null && graphAssociationPage.AssociationList.Any())
            {
                totalItem = graphAssociationPage.TotalItems;

                var customerAssociation = graphAssociationPage.AssociationList;

                var ctsCustIdList = customerAssociation.Select(x => new { CTSCustId = x.CTSCustID, SubscriberId = x.SubscriberID });

                customerAssociation = GetCustClassification(string.Join(",", customerAssociation.Select(c => c.CustId).Distinct()), customerAssociation.ToList());

                var flaggedAffectedTagList = _accountService.GetDirectAssociatedAccountTagFlagged(JsonConvert.SerializeObject(ctsCustIdList));

                result = customerAssociation.Select(x =>
                {
                    var newX = new CustomerAssociationByAccountDetailViewModel
                    {
                        CustId = x.CustId.HasValue ? x.CustId.Value : 0,
                        Username = !string.IsNullOrWhiteSpace(x.Username) ? x.Username : x.Username2,
                        LoginId = !string.IsNullOrWhiteSpace(x.Username2) ? x.Username2 : x.Username,
                        AssociationDate = x.AssociationDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                        AssociationType = x.AssociationType,
                        AssociationTypeName = StandardizeAssociationTypeName(x.AssociationTypeName),
                        Status = x.StatusName == "NA" ? "N/A" : x.StatusName,
                        StatusId = x.StatusId.HasValue ? x.StatusId.Value : 0,
                        SubscriberId = x.SubscriberID,
                        SubscriberName = !string.IsNullOrEmpty(x.SubscriberName) ? x.SubscriberName : "-",
                        CTSCustId = x.CTSCustID,
                        IsLinked = x.AssociationStatus == 1,
                        IsProblemAccount = x.ParentIDs != null ? x.ParentIDs.Split(',').Any(y => y == "50") : false,
                        IsPAProbation = x.IsProbation,
                        CategoryIds = x.CategoryIDs,
                        CategoryNames = !string.IsNullOrEmpty(x.CategoryNames) ? x.CategoryNames : "-",
                        CustomerClasses = x.CustomerClasses,
                        GroupIDs = x.GroupIDs,
                        OriginGroupIDs = x.OriginGroupIDs
                    };

                    var flaggedAffected = flaggedAffectedTagList.FirstOrDefault(y => y.SubscriberID.Equals(newX.SubscriberId) && y.CTSCustID.Equals(newX.CTSCustId));
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

        private IEnumerable<CategoryGroupViewModel> GetAllCategories()
        {
            var categoryList = new List<CategoryGroupViewModel>();

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
                               CategoryName = y.CategoryName
                           }).OrderBy(y => y.CategoryName)
                       }).ToList();
            }

            return categoryList;
        }

        private string StandardizeAssociationTypeName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var spanBettingPattern = $"<span class='association-type-name betting-pattern'>{Consts.AssociationTypeName_BettingPattern}</span>";
                var spanIPAssociation = $"<span class='association-type-name ip-association'>{Consts.AssociationTypeName_IP}</span>";

                name = name.Replace(Consts.AssociationTypeName_BettingPattern, spanBettingPattern);
                name = name.Replace(Consts.AssociationTypeName_IP, spanIPAssociation);
            }

            return name;
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
                                          OriginGroupIDs = group.a.OriginGroupIDs
                                      }).ToList();

            return result;
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileAssAccountViewOnGraph)]
        public ActionResult GetDirectAssociationGraph(AssociatedFilterModel model, string displayName)
        {
            var graphCustomer = new GraphCustomerItemViewModel();

            var getAssociationFromMySql = AppSettings.Get("GetAssociationProfileFromMySQL") == "1";

            var graphAssociationPaging = _accountService.GetAssociatedAccount(model, getAssociationFromMySql);

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
        public ActionResult GetShortestAssociationGraph(int custIdFrom, int custIdTo)
        {
            var shortestPathOfAccount = _accountService.GetShortestPathAssociationByGraph(ConfigSettings.GetShortestAssociation, custIdFrom, custIdTo);
            var graphCustomer = _graphService.GetGraphCustomerItemByShortestPathOfAccount(shortestPathOfAccount);

            return Json(graphCustomer, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
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
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileAssAccountInsert)]
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
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileAssAccountLinkedUnlinked)]
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
            var data = new MatchesAssociationByAIViewModel();

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
                        data = new MatchesAssociationByAIViewModel
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
                    LogService.Error($"Association Detail throws exception - SPU_AIML_GB_GetAssociationInfo: {groupBettingAssociation.ErrorMessage} - {JsonConvert.SerializeObject(new { para1, para2, originGroupIDs })}");
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetMatchesAssociationDetailByAI(MatchesAssociationDetailViewModel para)
        {
            var data = new List<MatchesDetailInfoViewModel>();
            try
            {
                if (para == null)
                {
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

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
                var logMessage = new LogMessage(ex.Message)
                         .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                         .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileAssAccountDetailRpt)]
        public ActionResult GetCoupleInformationsByAI(long custID1, long custID2)
        {
            List<CoupleInfoViewModel> data = new List<CoupleInfoViewModel>();

            try
            {
                var para1 = (custID1 <= custID2) ? custID1 : custID2;
                var para2 = (custID1 > custID2) ? custID1 : custID2;

                data = _accountService.GetCoupleInfos(para1, para2).Select(s => { s.CustID1 = custID1; s.CustID2 = custID2; return s; }).OrderByDescending(o => o.ToDate).ToList();
            }

            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetStakeIPPatternDetailByAI(CoupleInfoModel para)
        {
            var data = new List<StakeIpPatternDetailViewModel>();

            try
            {
                if (para == null)
                {
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

                StakeIpPatternDetailModel stakeIpPatternDetailModel = new StakeIpPatternDetailModel()
                {
                    CoupleID = para.CoupleID,
                    CustId1 = para.CustID1,
                    CustId2 = para.CustID2,
                    CoupleFraudDetails = _accountService.GetCoupleFraudDetails(para.CoupleID, para.FromDate, para.ToDate),
                    CoupleIPFraudDetails = _accountService.GetCoupleIPFraudDetails(para.CoupleID, para.FromDate, para.ToDate)
                };

                stakeIpPatternDetailModel.TicketDetails = GetTicketDetailsModel(stakeIpPatternDetailModel.CoupleFraudDetails, stakeIpPatternDetailModel.CoupleIPFraudDetails);

                data.AddRange(GetStakePatternDetail(stakeIpPatternDetailModel));
                var laststakePatternDetail = data.LastOrDefault();
                stakeIpPatternDetailModel.LastGroupID = laststakePatternDetail != null ? laststakePatternDetail.GroupID : 1;
                stakeIpPatternDetailModel.LastGroupCustID = laststakePatternDetail != null ? laststakePatternDetail.GroupCustID : 1;

                data.AddRange(GetIpPatternDetail(stakeIpPatternDetailModel));

            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                         .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                         .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<TicketDetailsModel> GetTicketDetailsModel(IEnumerable<CoupleFraudDetailModel> coupleFraudDetails, IEnumerable<CoupleIPFraudDetailModel> coupleIPFraudDetails)
        {
            var unionAllFraudTransList = coupleFraudDetails.Select(s => s.FraudTransList).Union(coupleIPFraudDetails.Select(s => s.FraudTransList)).ToArray();
            var fraudTransList = string.Join(",", unionAllFraudTransList);

            return _accountService.GetTicketDetailsModel(fraudTransList);
        }

        private IEnumerable<StakeIpPatternDetailViewModel> GetStakePatternDetail(StakeIpPatternDetailModel stakeIpPatternDetailModel)
        {
            var result = new List<StakeIpPatternDetailViewModel>();

            int groupID = 1;
            int groupCustId = 1;
            var groupStateIds = stakeIpPatternDetailModel.CoupleFraudDetails.GroupBy(g => g.StateID).Select(s => new { StateId = s.Key, SumStake = s.Max(m => m.SumStake) });
            foreach (var groupStateId in groupStateIds.OrderByDescending(desc => desc.SumStake))
            {
                var coupleFraudDetailByStateIds = stakeIpPatternDetailModel.CoupleFraudDetails.Where(p => p.StateID == groupStateId.StateId);
                foreach (var coupleFraudDetail in coupleFraudDetailByStateIds)
                {
                    var transIds = coupleFraudDetail.FraudTransListSperation.ToArray();
                    var ticketDetailsByCust1 = stakeIpPatternDetailModel.TicketDetails.Where(p => coupleFraudDetail.MatchID == p.MatchID && stakeIpPatternDetailModel.CustId1 == p.CustID && transIds.Contains(p.TransId.ToString())).OrderBy(asc => asc.TransDate);
                    int countTicketDetailsByCust1 = ticketDetailsByCust1.Count();
                    foreach (var ticketDetail in ticketDetailsByCust1)
                    {
                        StakeIpPatternDetailViewModel stakeIpPatternDetail = new StakeIpPatternDetailViewModel()
                        {
                            CoupleID = stakeIpPatternDetailModel.CoupleID,
                            GroupID = groupID,
                            IsStakePattern = true,
                            TicketsPerMatch = coupleFraudDetail.BetCount,
                            CountGroupCustID = countTicketDetailsByCust1,
                            TotalStake = coupleFraudDetail.SumStake,
                            MatchID = coupleFraudDetail.MatchID,
                            GroupCustID = groupCustId
                        };

                        SetStakeIpPatternDetailViewModel(stakeIpPatternDetail, ticketDetail);
                        result.Add(stakeIpPatternDetail);
                    }
                    groupCustId++;

                    var ticketDetailsByCust2 = stakeIpPatternDetailModel.TicketDetails.Where(p => coupleFraudDetail.MatchID == p.MatchID && stakeIpPatternDetailModel.CustId2 == p.CustID && transIds.Contains(p.TransId.ToString())).OrderBy(asc => asc.TransDate);
                    int countTicketDetailsByCust2 = ticketDetailsByCust2.Count();
                    foreach (var ticketDetail in ticketDetailsByCust2)
                    {
                        StakeIpPatternDetailViewModel stakeIpPatternDetail = new StakeIpPatternDetailViewModel()
                        {
                            CoupleID = stakeIpPatternDetailModel.CoupleID,
                            GroupID = groupID,
                            IsStakePattern = true,
                            TicketsPerMatch = coupleFraudDetail.BetCount,
                            CountGroupCustID = countTicketDetailsByCust2,
                            TotalStake = coupleFraudDetail.SumStake,
                            MatchID = coupleFraudDetail.MatchID,
                            GroupCustID = groupCustId
                        };

                        SetStakeIpPatternDetailViewModel(stakeIpPatternDetail, ticketDetail);
                        result.Add(stakeIpPatternDetail);
                    }
                    groupCustId++;

                }
                groupID++;
            }

            return result;
        }

        private void SetStakeIpPatternDetailViewModel(StakeIpPatternDetailViewModel stakeIpPatternDetail, TicketDetailsModel ticketDetail)
        {
            if (ticketDetail == null)
            {
                return;
            }

            stakeIpPatternDetail.UserName = ticketDetail.UserName;
            stakeIpPatternDetail.TransId = ticketDetail.TransId;
            stakeIpPatternDetail.IP = ticketDetail.IP;
            stakeIpPatternDetail.Odds = ticketDetail.Odds;
            stakeIpPatternDetail.Stake = ticketDetail.Stake;
            stakeIpPatternDetail.StakeRM = Math.Truncate(ticketDetail.StakeRM);
            stakeIpPatternDetail.BetTeam = ticketDetail.BetTeam;
            stakeIpPatternDetail.BetType = ticketDetail.BetType;
            stakeIpPatternDetail.Currency = ticketDetail.CurrencyName;
            stakeIpPatternDetail.LiveScore = ticketDetail.LiveHomeScore != null && ticketDetail.LiveAwayScore != null ? $"{ticketDetail.LiveHomeScore}-{ticketDetail.LiveAwayScore}" : "";
            stakeIpPatternDetail.TransDateString = ticketDetail.TransDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss);
        }

        private IEnumerable<StakeIpPatternDetailViewModel> GetIpPatternDetail(StakeIpPatternDetailModel stakeIpPatternDetailModel)
        {
            var result = new List<StakeIpPatternDetailViewModel>();
            int groupID = stakeIpPatternDetailModel.LastGroupID + 1;
            int groupCustId = stakeIpPatternDetailModel.LastGroupCustID + 1;
            var groupTypeIPEntanglements = stakeIpPatternDetailModel.CoupleIPFraudDetails.GroupBy(g => g.TypeIPEntanglement).Select(s => s.Key);
            foreach (var groupTypeIPEntanglement in groupTypeIPEntanglements)
            {
                var coupleIPFraudDetails = stakeIpPatternDetailModel.CoupleIPFraudDetails.Where(p => p.TypeIPEntanglement == groupTypeIPEntanglement);
                foreach (var coupleIPFraudDetail in coupleIPFraudDetails)
                {
                    var transIds = coupleIPFraudDetail.FraudTransListSperation.ToArray();
                    var ticketDetailsByCust1 = stakeIpPatternDetailModel.TicketDetails.Where(p => coupleIPFraudDetail.MatchID == p.MatchID && stakeIpPatternDetailModel.CustId1 == p.CustID && transIds.Contains(p.TransId.ToString())).OrderBy(asc => asc.TransDate);
                    int countTicketDetailsByCust1 = ticketDetailsByCust1.Count();
                    foreach (var ticketDetail in ticketDetailsByCust1)
                    {
                        StakeIpPatternDetailViewModel stakeIpPatternDetail = new StakeIpPatternDetailViewModel()
                        {
                            CoupleID = stakeIpPatternDetailModel.CoupleID,
                            GroupID = groupID,
                            MatchID = coupleIPFraudDetail.MatchID,
                            GroupCustID = groupCustId,
                            IPPatternType = coupleIPFraudDetail.IPPatternType,
                            CountGroupCustID = countTicketDetailsByCust1
                        };

                        SetStakeIpPatternDetailViewModel(stakeIpPatternDetail, ticketDetail);
                        result.Add(stakeIpPatternDetail);
                    }
                    groupCustId++;

                    var ticketDetailsByCust2 = stakeIpPatternDetailModel.TicketDetails.Where(p => coupleIPFraudDetail.MatchID == p.MatchID && stakeIpPatternDetailModel.CustId2 == p.CustID && transIds.Contains(p.TransId.ToString())).OrderBy(asc => asc.TransDate);
                    int countTicketDetailsByCust2 = ticketDetailsByCust2.Count();
                    foreach (var ticketDetail in ticketDetailsByCust2)
                    {
                        StakeIpPatternDetailViewModel stakeIpPatternDetail = new StakeIpPatternDetailViewModel()
                        {
                            CoupleID = stakeIpPatternDetailModel.CoupleID,
                            GroupID = groupID,
                            MatchID = coupleIPFraudDetail.MatchID,
                            GroupCustID = groupCustId,
                            IPPatternType = coupleIPFraudDetail.IPPatternType,
                            CountGroupCustID = countTicketDetailsByCust2
                        };

                        SetStakeIpPatternDetailViewModel(stakeIpPatternDetail, ticketDetail);
                        result.Add(stakeIpPatternDetail);
                    }
                    groupCustId++;

                }
                groupID++;

                //var coupleFraudDetailByStateId = stakeIpPatternDetailModel.CoupleIPFraudDetails.Where(p => p.TypeIPEntanglement == groupTypeIPEntanglement);
                //foreach (var coupleIPFraudDetail in coupleFraudDetailByStateId)
                //{
                //    var ticketDetails = stakeIpPatternDetailModel.TicketDetails.Where(p => coupleIPFraudDetail.FraudTransListSperation.Any(transId => transId == p.TransId.ToString())).OrderBy(asc => asc.TransDate);

                //    foreach (var ticketDetail in ticketDetails)
                //    {
                //        StakeIpPatternDetailViewModel stakeIpPatternDetail = new StakeIpPatternDetailViewModel()
                //        {
                //            CoupleID = stakeIpPatternDetailModel.CoupleID,
                //            GroupID = groupID,
                //            MatchID = coupleIPFraudDetail.MatchID,
                //            GroupCustID = groupCustId,
                //            IPPatternType = coupleIPFraudDetail.IPPatternType,
                //            TicketsPerMatch = coupleIPFraudDetail.FraudTransListSperation.Count()
                //        };

                //        SetStakeIpPatternDetailViewModel(stakeIpPatternDetail, ticketDetail);
                //        result.Add(stakeIpPatternDetail);
                //    }
                //    groupCustId++;
                //}
                groupID++;
            }
            return result;
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileAssAccountDetailRpt)]
        public ActionResult GetIPAssociation(long custId1, long custId2)
        {
            var data = new AssociationByIPInfoViewModel();
            var para1 = custId1;
            var para2 = custId2;

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

                data = new AssociationByIPInfoViewModel
                {
                    TotalMatchCount = info.TotalMatchCount,
                    TotalIPSharingCount = info.TotalIPCount,
                    ErrorMessage = info.ErrorMessage,

                    ItemList = info.ItemList.Select(x =>
                    {
                        var homeName = CachedRef.GetTeamName(x.HomeID);
                        var awayName = CachedRef.GetTeamName(x.AwayID);
                        var leagueName = CachedRef.GetLeagueName(x.LeagueID);
                        var betTypeList = new List<string>();
                        var ip = x.IP;
                        var eventDateStr = "-";

                        if (x.BettypeList != null)
                        {
                            var arrBetType = x.BettypeList.Split(',');

                            foreach (var item in arrBetType)
                            {
                                var betTypeId = 0;

                                int.TryParse(item, out betTypeId);

                                betTypeList.Add(CachedRef.GetBettypeName(betTypeId));
                            }
                        }

                        if (ip != null)
                        {
                            ip = ip.Replace(",", ", ");
                        }

                        var match = matchInfos.Where(y => y.MatchID == x.MatchID).FirstOrDefault();

                        if (match != null && match.EventDate != DateTime.MinValue)
                            eventDateStr = match.EventDate.ToString(Consts.FormatDateToString_MMMddyyyy);

                        return new AssociationByIPItemViewModel
                        {
                            MatchID = x.MatchID,
                            EventName = homeName != string.Empty ? (homeName + " - vs - " + awayName) : "-",
                            LeagueName = leagueName != string.Empty ? leagueName : "-",
                            EventDateString = eventDateStr,
                            BetTypes = string.Join(", ", betTypeList),
                            IP = ip
                        };
                    }).ToList()
                };
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion Associated By Account

        #region Chain To Evidence

        public ActionResult ChainToEvidence()
        {
            return PartialView("ChainToEvidence/ChainToEvidence");
        }

        [HttpGet]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult GetEvidenceList()
        {
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
            var result = new AccountEvidenceViewModel();
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
                result.FlaggedEvidenceList = flaggedEvidences.OrderBy(x => x.EvidenceID).Select(x => new FlaggedEvidenceViewModel
                {
                    EvidenceID = x.EvidenceID,
                    EvidenceCode = x.EvidenceCode,
                    EvidenceName = x.EvidenceName,
                    EvidenceDisplay = x.EvidenceCode + " " + x.EvidenceName,
                    Comment = x.Remark,
                    Creator = x.Creator,
                    FlaggedDate = x.CreatedDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss)
                }).ToList();

                evidenceList.AddRange(result.FlaggedEvidenceList.Select(x => new EvidenceViewModel
                {
                    EvidenceID = x.EvidenceID,
                    EvidenceCode = x.EvidenceCode,
                    EvidenceDisplay = x.EvidenceCode + " " + x.EvidenceName
                }));
            }

            if (affectedRetractEvidences.AffectedEvidenceList != null && affectedRetractEvidences.AffectedEvidenceList.Any())
            {
                result.AffectedRetractEvidenceList.AddRange(affectedRetractEvidences.AffectedEvidenceList.OrderBy(x => x.EvidenceID).Select(x => new AffectedRetractEvidenceViewModel
                {
                    EvidenceID = x.EvidenceID,
                    EvidenceCode = x.EvidenceCode,
                    EvidenceName = x.EvidenceName,
                    EvidenceDisplay = x.EvidenceCode + " " + x.EvidenceName,
                    LoginID = string.IsNullOrEmpty(x.LoginID) ? x.UserName : x.LoginID,
                    CTSCustID = x.CTSCustID,
                    UserName = string.IsNullOrEmpty(x.UserName) ? x.LoginID : x.UserName,
                    AssociationType = x.AssociationType,
                    AffectedTime = x.CreatedDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                    LastLoginTime = x.LastLoginTime.Year != 0001 ? x.LastLoginTime.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss) : "None",
                    IsRetract = false
                }));
            }

            if (affectedRetractEvidences.RetractEvidenceList != null && affectedRetractEvidences.RetractEvidenceList.Any())
            {
                result.AffectedRetractEvidenceList.AddRange(affectedRetractEvidences.RetractEvidenceList.OrderBy(x => x.EvidenceID).Select(x => new AffectedRetractEvidenceViewModel
                {
                    EvidenceID = x.EvidenceID,
                    EvidenceCode = x.EvidenceCode,
                    EvidenceName = x.EvidenceName,
                    EvidenceDisplay = x.EvidenceCode + " " + x.EvidenceName,
                    LoginID = string.IsNullOrEmpty(x.LoginID) ? x.UserName : x.LoginID,
                    CTSCustID = x.CTSCustID,
                    UserName = string.IsNullOrEmpty(x.UserName) ? x.LoginID : x.UserName,
                    AssociationType = x.AssociationType,
                    AffectedTime = x.CreatedDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                    LastLoginTime = x.LastLoginTime.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                    Creator = x.Creator,
                    Comment = string.IsNullOrWhiteSpace(x.Remark) ? string.Empty : x.Remark,
                    IsRetract = true
                }));
            }

            evidenceList.AddRange(result.AffectedRetractEvidenceList.Select(x => new EvidenceViewModel
            {
                EvidenceID = x.EvidenceID,
                EvidenceCode = x.EvidenceCode,
                EvidenceDisplay = x.EvidenceCode + " " + x.EvidenceName
            }));

            result.EvidenceList.AddRange(evidenceList.Distinct(x => x.EvidenceCode).OrderBy(x => x.EvidenceID));

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
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
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
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
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
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

        //[HttpPost]
        //[NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        //[TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileChainstoEvidenceInsertEvidence)]
        //public ActionResult PlaceEvidence(UpdatePlaceEvidenceModel model)
        //{
        //    model.UserId = model.UserId == 0 ? CurrentSession.UserId : model.UserId;
        //    var result = _accountService.ProcessPlaceEvidence(model);

        //    if (result.ErrorCode == 0)
        //    {


        //        var syncCustIds = Enumerable.Empty<long>();

        //        var paLosingClassification = _rescanClassificationService.GetProblemAccountLosing(objects, FunctionId.PlaceSingleEvidence);

        //        var custEvidenceJson = JsonConvert.SerializeObject(paLosingClassification);

        //        var syncResult = _accountService.SyncCustEvidenceToCategory(custEvidenceJson, false, out syncCustIds);

        //        if (syncResult.ErrorCode == 0)
        //        {
        //            _customerClassificationService.PushDataToExternal(syncCustIds, new PushDataToExternalModel
        //            {
        //                ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
        //                IsPushCCToSoon88 = true,
        //                IsUpdateDangerToMainDB = true,
        //                DangerLevelType = DangerLevel.ABI
        //            });

        //            var tasks = new List<Task>
        //            {
        //                Task.Run(() =>
        //                {
        //                     _accountService.CheckNewPAAssociationCustomerForRollback(syncResult.CustInfo);
        //                }),
        //                Task.Run(() =>
        //                {
        //                    _antiFraudCenterService.InsertAFCAccounts(syncCustIds, FromFlow.CTSWEB);
        //                })
        //            };

        //            Task.WhenAll(tasks.ToArray());
        //        }
        //    }

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //[NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        //[TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileChainstoEvidenceRemoveEvidence)]
        //public ActionResult RemoveFlaggedEvidence(RemoveEvidenceModel model)
        //{
        //    model.UserId = CurrentSession.UserId;

        //    var result = _accountService.RemoveFlaggedEvidence(model);

        //    if (result.ErrorCode == 0)
        //    {
        //        var objects = new[] { new {
        //           CTSCustID = model.CTSCustId,
        //           CustID = model.CustId,
        //           SubscriberID = model.SubscriberId,
        //           EvidenceID = model.EvidenceId,
        //           CreatedBy = model.UserId,
        //           ActionType = 2
        //        } };

        //        var syncCustIds = Enumerable.Empty<long>();
        //        var custEvidenceJson = JsonConvert.SerializeObject(objects);

        //        // Sync Evidence to Category
        //        var syncResult = _accountService.SyncCustEvidenceToCategory(custEvidenceJson, false, out syncCustIds);

        //        if (syncResult.ErrorCode == 0)
        //        {
        //            // Rescan Problem Account & Normal Account
        //            _rescanClassificationService.RescanCustomerClassify(new RescanCustClassificationModel
        //            {
        //                CustIds = syncCustIds,
        //                IsCheckPAFromNAP = true,
        //                IsPushCCToSoon88 = true,
        //                FunctionId = FunctionId.RemoveFlaggedEvidenceWeb,
        //                IsUpdateDangerToMainDB = true,
        //                DangerLevelType = DangerLevel.ABI
        //            });
        //        }

        //        var custIdsFlaggedEvidences = _customerEvidenceService.CheckCleanCustFlaggedEvidence(model.CTSCustId.ToString());

        //        if (custIdsFlaggedEvidences != null && custIdsFlaggedEvidences.Any())
        //        {
        //            _customerEvidenceService.UpdateBetLimitOfUnflaggedCustomer(string.Join(",", custIdsFlaggedEvidences.Select(c => c.CustID)));
        //        }
        //    }

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
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
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult RestoreAffectedEvidence(RemoveEvidenceModel model)
        {
            model.UserId = CurrentSession.UserId;

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
                model.SubscriberId,
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
                Flagged = ((Flagged)x.Flagged).GetStringSlow(),
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

        public ActionResult CustomerCategory()
        {
            return PartialView("CustomerCategory/CustomerCategory");
        }

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassification)]
        public ActionResult TrackingCustomerCategory()
        {
            return null;
        }

        public ActionResult GetCustomerClassification(long ctsCustID, int roleID)
        {
            var isPinCategory = false;
            var isExcludedTagAssPA = false;
            var isVVip = false;
            var isProblemAccount = false;
            var isPAProbation = false;
            var categoryList = new List<CustomerCategoryViewModel>();
            var classificationHistoryList = new List<CustomerClassificationHistoryViewModel>();

            var classification = _accountService.GetCustomerClassification(ctsCustID, out isPinCategory, out isExcludedTagAssPA);

            if (classification != null && classification.Any())
            {
                categoryList = classification
                    .Select(x => new CustomerCategoryViewModel
                    {
                        CategoryId = x.CategoryID,
                        CategoryType = x.CategoryType,
                        CategoryName = x.CategoryName ?? string.Empty,
                        IsProblemAccount = x.CategoryType == 2
                    }).ToList();

                isVVip = categoryList.Any(x => x.CategoryId == 210); // VVIP             
                isProblemAccount = categoryList.Any(x => x.IsProblemAccount);

                isPAProbation = isProblemAccount && classification.Any(x => x.IsProbation);
            }

            var classificationHistory = _accountService.GetCustomerClassificationHistory(ctsCustID);

            if (classificationHistory != null && classificationHistory.Any())
            {
                classificationHistoryList = classificationHistory.Select(x => new CustomerClassificationHistoryViewModel
                {
                    CreatedTimeString = x.CreatedTime.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                    SourceTypeName = x.SourceTypeName,
                    CategoryName = (x.SourceTypeID == (int)SourceTypeID.NoShow_Detail) ? "-" : (x.CategoryName != null ? x.CategoryName : string.Empty),
                    CustomerClass = x.CustomerClass,
                    Reason = GetReason(x, roleID),
                    TurnOver = x.TurnOver,
                    Winloss = x.Winloss,
                    Margin = x.Margin,
                    BetCount = x.BetCount,
                    ActiveDays = x.ActiveDays,
                    LastXDaysMargin = x.LastXDaysMargin,
                    ProbationPeriodWinloss = x.ProbationPeriodWinloss,
                    CreatedBy = x.CreatedBy
                }).ToList();
            }

            return Json(new
            {
                isPinCategory,
                isExcludedTagAssPA,
                isVVip,
                isProblemAccount,
                isPAProbation,
                categoryList,
                data = classificationHistoryList,
            }, JsonRequestBehavior.AllowGet);
        }

        private string GetReason(CustomerClassificationHistoryModel cc, int roleID)
        {
            var divReason = $"<div>";
            var spanReason = string.Empty;
            var winlost = Math.Round(cc.Winloss, 3);

            if (cc.SourceTypeID == (int)SourceTypeID.Classification_Rescan)
            {
                spanReason += $"Win lost: {winlost}";
            }
            else if (cc.IsFromTVS)
            {
                if (cc.TVSRequestID > 0)
                    spanReason += $"Void by RequestID: { cc.TVSRequestID } ";
                else
                    spanReason += $"Void by BetID ";

                if (roleID == 1)
                    spanReason += $"| Win lost: {winlost} ";
            }

            if (spanReason == string.Empty)
                spanReason = MappingReasonByCategoryID(spanReason, cc);

            if (spanReason == string.Empty)
                spanReason = MappingReasonBySourceTypeID(spanReason, cc, roleID);

            if (spanReason == string.Empty)
                divReason += $"<div>{MappingAdditionalReason(cc)}</div>" + $"<div>{cc.SourceTypeName}</div>";
            else
                divReason += $"<div>{MappingAdditionalReason(cc) + spanReason }</div>";

            return divReason;
        }

        private string MappingReasonByCategoryID(string spanReason, CustomerClassificationHistoryModel cc)
        {
            var margin = Math.Round(cc.Margin, 3);
            var winlost = Math.Round(cc.Winloss, 3);
            var turnOver = Math.Round(cc.TurnOver, 3);
            var betCount = cc.BetCount;
            var activeDays = cc.ActiveDays;
            var twGroupBettingRate = cc.TWGroupBettingRate;
            var twTicketRejectRate = cc.TWTicketRejectRate;
            var twDesktopTicket = cc.TWDesktopUsageRate;

            switch (cc.CategoryID)
            {
                case (int)ClassificationSourceType.NewMember:
                case (int)ClassificationSourceType.NewMember31:
                case (int)ClassificationSourceType.NewMember32:
                case (int)ClassificationSourceType.NewMember33:
                case (int)ClassificationSourceType.NormalAccount:
                case (int)ClassificationSourceType.NormalAccount121:
                case (int)ClassificationSourceType.NormalAccount122:
                case (int)ClassificationSourceType.NormalAccount123:
                case (int)ClassificationSourceType.Probation:
                case (int)ClassificationSourceType.Probation51:
                case (int)ClassificationSourceType.Probation52:
                case (int)ClassificationSourceType.Probation53:
                case (int)ClassificationSourceType.SmartPunter:
                case (int)ClassificationSourceType.SmartPunter61:
                case (int)ClassificationSourceType.SmartPunter62:
                case (int)ClassificationSourceType.SmartPunter63:
                case (int)ClassificationSourceType.HighRiskPunter:
                case (int)ClassificationSourceType.HighRiskPunter71:
                case (int)ClassificationSourceType.HighRiskPunter72:
                case (int)ClassificationSourceType.HighRiskPunter73:
                    spanReason += $"Win lost: {winlost} | Margin: {margin} | Bet Count: {betCount}";
                    break;
                case (int)ClassificationSourceType.GoodMember:
                case (int)ClassificationSourceType.GoodMember111:
                case (int)ClassificationSourceType.GoodMember112:
                case (int)ClassificationSourceType.GoodMember113:
                    spanReason += $"Turnover: {turnOver} | Win lost: {winlost} | Margin: {margin} | Bet Count: {betCount} | Active Day: {activeDays}";
                    break;
                case (int)ClassificationSourceType.Probation57:
                case (int)ClassificationSourceType.SmartPunter67:
                case (int)ClassificationSourceType.HighRiskPunter77:
                    spanReason += $"Win lost: {winlost} | Margin: {margin} | Bet Count: {betCount} | Group Betting Percent: {twGroupBettingRate}";
                    break;
                case (int)ClassificationSourceType.Probation58:
                case (int)ClassificationSourceType.SmartPunter68:
                case (int)ClassificationSourceType.HighRiskPunter78:
                    spanReason += $"Win lost: {winlost} | Margin: {margin} | Bet Count: {betCount} | Desktop Percent: {twDesktopTicket} | Reject Percent: {twTicketRejectRate}";
                    break;
                case (int)ClassificationSourceType.NormalAccountInactive:
                case (int)ClassificationSourceType.ProPunterInactive:
                    spanReason += $"<span> { cc.SourceTypeName } </span>";
                    break;

                case (int)ClassificationSourceType.RobotUser61:
                case (int)ClassificationSourceType.RobotUser163:
                case (int)ClassificationSourceType.RobotUser164:
                    if (cc.IsFromTVS)
                    {
                        if (cc.TVSRequestID > 0)
                            spanReason += $"Void by RequestID: { cc.TVSRequestID } | Win lost: {winlost}";
                        else
                            spanReason += $"Void by BetID |  Win lost: {winlost}";
                    }
                    else if (cc.IsFromTW)
                    {
                        spanReason += $"Counter: {cc.RobotCounter} | Win lost: {winlost}";
                    }
                    else if (cc.IsFromAI)
                    {
                        spanReason += $"Win lost: {winlost}";
                    }
                    else if (cc.IsFromCTS)
                    {
                        spanReason += $"Robot is marked directly ";

                        spanReason += $" | Win lost: {winlost}";
                        if (!string.IsNullOrEmpty(cc.Remark))
                            spanReason += $" | {cc.Remark}";
                    }

                    break;

                default:
                    break;
            }

            return spanReason;
        }

        private string MappingReasonBySourceTypeID(string spanReason, CustomerClassificationHistoryModel cc, int roleID)
        {
            var winlost = Math.Round(cc.Winloss, 3);

            switch (cc.SourceTypeID)
            {
                case (int)SourceTypeID.Classification_PAMarkDirectly:
                case (int)SourceTypeID.Classification_PAAffectedByUplines:
                    if (!string.IsNullOrEmpty(cc.Remark) && cc.Remark.Contains(Consts.MarkPA_AssociatedGroup))
                    {
                        spanReason += $"<span> { cc.Remark } </span>";

                        if (roleID == 1)
                            spanReason += $"| Win lost: {winlost} ";
                    }
                    else
                    {
                        spanReason += $"<span> { cc.SourceTypeName } </span>";
                        if (roleID == 1)
                            spanReason += $"| Win lost: {winlost} ";

                        if (!string.IsNullOrEmpty(cc.Remark))
                            spanReason += $"| {cc.Remark}";
                    }

                    break;
                case (int)SourceTypeID.Classification_PAUnmarkDirectly:
                case (int)SourceTypeID.Classification_UnmarkingPAAffectedDirectly:
                    spanReason += $"<span> { cc.SourceTypeName } </span>";
                    if (!string.IsNullOrEmpty(cc.Remark))
                    {
                        spanReason += $"| {cc.Remark}";
                    }
                    break;
                case (int)SourceTypeID.Classification_ClassifyGBMatchMonitor:
                    if (!string.IsNullOrEmpty(cc.Remark))
                    {
                        spanReason += $"Match ID {cc.Remark} | Win lost: {winlost}";
                    }
                    else
                    {
                        spanReason += $"Win lost: {winlost}";
                    }

                    break;
                case (int)SourceTypeID.NoShow_Detail:
                    spanReason += $"{cc.CategoryName}";
                    break;
                default:
                    break;
            }

            return spanReason;
        }

        private string MappingAdditionalReason(CustomerClassificationHistoryModel cc)
        {
            if (cc.SourceTypeID != (int)SourceTypeID.NoShow_Detail && !string.IsNullOrEmpty(cc.LicVIP))
            {
                return $"<div>{cc.LicVIP}</div>";
            }

            if (cc.SourceTypeID != (int)SourceTypeID.NoShow_Detail && !string.IsNullOrEmpty(cc.LicBA))
            {
                return $"<div>{cc.LicBA}</div>";
            }

            return string.Empty;
        }

        //[NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        //[TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassificationPinCategory)]
        //[HttpPost]
        //public ActionResult PinCategoryCustomerClassification(long ctsCustId, int subscriberId, bool isPinCategory)
        //{
        //    var result = _accountService.PinCategoryCustomerClassification(ctsCustId, subscriberId, isPinCategory, CurrentSession.UserId);

        //    return Json(new
        //    {
        //        errorCode = result.ErrorCode,
        //        errorMessage = result.ErrorMsg
        //    }, JsonRequestBehavior.AllowGet);
        //}

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassificationNormalCategory)]
        [HttpPost]
        public ActionResult ChangeExcludedTagAssPA(int custId, long ctsCustId, int subscriberId, bool isExcludedTagAssPA, bool isProblemAccount, int custSubId)
        {
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
                        IsRescanPA = false,
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
                LogService.Error($"CTS - ExcludeTaggingAssociationWithPA exception: { ex.Message }", ex);

                return Json(new { errorCode = -1, errorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult MarkCustomerCategory(long ctsCustID, int roleID, string userName, bool isRobotUser, bool isPinCategory)
        {
            var result = new MarkCustomerCategoryViewModel
            {
                RoleID = roleID,
                UserName = userName,
                IsRobotUser = isRobotUser,
                IsPinCategory = isPinCategory,
                ProblemAccountList = _pamService.GetProblemAccountCategories()
            };

            var classifications = _accountService.GetCustomerClassification(ctsCustID);

            if (classifications != null && classifications.Any())
            {
                result.CategoryList = classifications.Select(x => new CustomerCategoryViewModel
                {
                    CategoryId = x.CategoryID,
                    CategoryName = x.CategoryName ?? string.Empty,
                    CategoryGroup = x.CategoryGroup
                });

                result.IsVVip = classifications.Any(x => x.CategoryID == 210); // VVIP
            }

            return PartialView("CustomerCategory/MarkCustomerCategory", result);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassificationMarkCategory)]
        public ActionResult SaveMarkVVIP(SaveMarkCustomerCategoryViewModel model)
        {
            var result = new DBResult();

            var categoryModel = new CustomerCategoryModel
            {
                CustId = model.CustID,
                CTSCustId = model.CTSCustID,
                SubscriberId = model.SubscriberID,
                CategoryId = model.CategoryID,
                RoleId = model.RoleID,
                IsAffectToAllDownlines = model.IsAffectAllDownlines,
                Remark = model.Remark,
                IsVVip = model.IsVVip,
                CreatedBy = CurrentSession.UserId,
                ActionType = ActionTypeQueueCustCategory.Insert
            };

            var isMarkDirect = MarkDirectVVIP(categoryModel);

            if (isMarkDirect && categoryModel.RoleId != 1 && categoryModel.IsAffectToAllDownlines)
            {
                result = _accountService.AddQueueAffectDownlinesCustomerCategory(categoryModel);
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
                var latestClassifications = _accountService.MarkVVIPCategory(categoryModel);

                if (categoryModel.IsVVip)
                {
                    latestClassifications = latestClassifications?.Where(c => c.CustId != 0);

                    if (latestClassifications != null && latestClassifications.Any())
                    {
                        // Update CC in table custinfo to MainDB
                        _customerClassificationService.RetryPushCCToMainDB(latestClassifications, 2);

                        Task.Run(() =>
                        {
                            _antiFraudCenterService.DeleteAFCAccounts(latestClassifications.Select(c => c.CustId), FromFlow.CTSWEB);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                            .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                            .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);

                return false;
            }

            return true;
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassificationMarkCategory)]
        public ActionResult SaveMarkCustomerCategory(SaveMarkCustomerCategoryViewModel model)
        {
            var pas = new List<InsertProblemAccountModel>()
            {
                new InsertProblemAccountModel
                {
                    CustID = model.CustID,
                    CTSCustID = model.CTSCustID,
                    RoleID = model.RoleID,
                    SubscriberID = model.SubscriberID,
                    IsLicensee = model.IsLicensee,
                    CategoryID = model.CategoryID,
                    CategoryGroup = model.CategoryGroup,
                    Remark = model.Remark,
                    IsDirectPA = true,
                    IsFromCTS = true,
                    CreatedBy = CurrentSession.UserId
                }
            };

            var result = _problemAccountClassificationService.InsertProblemAccountPackage(pas, InsertPA_ByType.Category, "ProfileAddProblemAccount", FromFlow.CTSWEB, true);

            return Json(new
            {
                errorCode = result.ErrorCode,
                errorMessage = result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassificationMarkCategory)]
        public ActionResult SaveMarkRobotAccount(SaveMarkCustomerCategoryViewModel model)
        {
            var dbResult = new DBResult();

            var robotAccounts = new List<InsertRobotModel>()
                {
                    new InsertRobotModel
                    {
                        CustID = model.CustID,
                        Remark = model.Remark,
                        IsDirect = true,
                        IsFromCTS = true,
                        CreatedBy = CurrentSession.UserId
                    }
                };

            var custInfos = _problemAccountClassificationService.InsertRobotAccountPackge(robotAccounts, false);

            if (custInfos.Any())
            {
                var custIds = custInfos.Select(x => x.CustID).Distinct();

                _customerClassificationService.PushDataToExternalPackge(custIds, new PushDataToExternalModel
                {
                    ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
                    IsPushCCToSoon88 = true,
                    IsUpdateDangerToMainDB = true,
                    IsSyncAFC = false
                }, FromFlow.CTSWEB);

                dbResult = new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
            }
            else
            {
                dbResult = new DBResult { ErrorCode = -1, ErrorMsg = "Mark PA Robot failed" };
            }

            return Json(new
            {
                errorCode = dbResult.ErrorCode,
                errorMessage = dbResult.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
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

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassificationRemoveCategory)]
        public ActionResult SaveUnmarkVVIP(CustomerCategoryModel model)
        {
            try
            {
                var result = new DBResult();
                model.CreatedBy = CurrentSession.UserId;
                model.ActionType = ActionTypeQueueCustCategory.Remove;

                var custIds = _accountService.RemoveVVIPCategory(model);

                if (custIds != null && custIds.Any())
                {
                    if (model.RoleId != 1 && model.IsAffectToAllDownlines)
                    {
                        result = _accountService.AddQueueAffectDownlinesCustomerCategory(model);
                    }

                    if (model.CustId != 0)
                    {
                        _rescanClassificationService.RescanCustomerClassification(new RescanCustClassificationModel
                        {
                            CustIds = new List<long> { model.CustId },
                            IsRescanPA = true,
                            IsToQueue = (model.RoleId != 1 && !model.IsAffectToAllDownlines), // add to queue for case SMA and not checking affect to downline to affect PA for downline
                            IsPushCCToSoon88 = true,
                            IsUpdateDangerToMainDB = true,
                            FunctionId = FunctionId.UnmarkCustomerCategory,
                            ServiceName = "RemoveCustomerCategory"
                        });
                    }
                }

                return Json(new
                {
                    errorCode = result.ErrorCode,
                    errorMessage = result.ErrorMsg
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogService.Error($"CTS - Unmark VVIP exception: { ex.Message }", ex);

                dataLogTrackingServices.LogErrorData(ex, "WEB-UnmarkVVIP");

                return Json(new { errorCode = -1, errorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassificationRemoveCategory)]
        public ActionResult SaveUnmarkCustomerCategory(SaveUnmarkCustomerCategoryViewModel model)
        {
            var paUnmark = new List<UnmarkProblemAccountModel>()
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
                        IsDirectPA = true
                    }
                };

            var unmarkPAResult = _problemAccountClassificationService.UnmarkProblemAccount(paUnmark, false);

            if (unmarkPAResult.ErrorCode == 0 && unmarkPAResult.CustomerInfo.Any())
            {
                var custIds = unmarkPAResult.CustomerInfo.Select(x => x.CustID);

                _problemAccountClassificationService.UpdateBetLimitOfUnflaggedCustomer(custIds.Distinct());

                var paramRescan = new RescanCustClassificationModel
                {
                    CustIds = custIds,
                    IsPushCCToSoon88 = true,
                    FunctionId = FunctionId.UnmarkCustomerCategory,
                    IsUpdateDangerToMainDB = true
                };

                var smCustIds = _rescanClassificationService.GetAgentCusts(custIds);

                _rescanClassificationService.RescanNormalAccount(paramRescan, Enumerable.Empty<long>(), smCustIds);
            }

            return Json(new
            {
                errorCode = unmarkPAResult.ErrorCode,
                errorMessage = unmarkPAResult.ErrorMessage
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion Customer Category

        #region Customer Category By Sport
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileGetCustomerCategoryBySportRpt)]
        public ActionResult GetCustomerCategoryBySport(long ctsCustID)
        {
            var custs = Enumerable.Empty<GetCustomerCategoryBySportViewModel>();

            var data = _accountService.GetCustomerCategoryBySport(ctsCustID);

            if (data != null && data.Any())
            {
                custs = data.Select(x => new GetCustomerCategoryBySportViewModel(x));
            }

            return Json(new { data = custs }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileGetCustomerCategoryHistoryBySportRpt)]
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

        #region Robot Classification

        //public ActionResult RobotClassification()
        //{
        //    return PartialView("RobotClassfication/LastestRobotClassification");
        //}

        //[TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileRobotClassification)]
        //public ActionResult TrackingRobotClassification()
        //{
        //    return null;
        //}

        //[HttpPost]
        //public ActionResult GetHistoryRobotClassification(long custId)
        //{
        //    var data = Enumerable.Empty<RobotClassificationViewModel>();
        //    var bettingStatisticFormat = "Turnover: {0} | Win lost: {1} | Margin: {2}% | Bet count: {3} | Active days: {4}";

        //    var robotData = _robotClassificationService.GetHistoryRobotClassification(custId);

        //    if (robotData != null && robotData.Any())
        //    {
        //        data = robotData.OrderByDescending(d => d.FromDate).Select(x =>
        //        {
        //            decimal margin = 0;

        //            if (x.Turnover != 0)
        //            {
        //                margin = x.Winloss / x.Turnover * 100;
        //            }

        //            return new RobotClassificationViewModel
        //            {
        //                ScannedDate = x.FromDate.ToString(Consts.FormatDateToString_MMMddyyyy),
        //                BettingStatistic = string.Format(bettingStatisticFormat
        //                                 , Math.Round(x.Turnover, 3).ToString("0.000").Replace(".000", string.Empty)
        //                                 , Math.Round(x.Winloss, 3).ToString("0.000").Replace(".000", string.Empty)
        //                                 , Math.Round(margin, 3).ToString("0.000").Replace(".000", string.Empty)
        //                                 , x.BetCount, x.ActiveDays),
        //                RobotType = x.RobotType
        //            };
        //        });
        //    }

        //    return Json(new { data }, JsonRequestBehavior.AllowGet);
        //}

        #endregion Robot Classification

        #region Robot Betting

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
            var firstRobotDate = string.Empty;
            var lastRobotDate = string.Empty;
            var robotTypeClassify = -2;
            var robotBettingList = Enumerable.Empty<RobotBettingViewModel>();

            var robotBettings = _robotBettingService.GetRobotBetting(custID, out RobotDateAndTypeModel robotDateAndType);

            if (robotBettings != null && robotBettings.Any())
            {
                robotBettingList = robotBettings.Select(x => new RobotBettingViewModel
                {
                    CustID = x.CustID,
                    PeriodRangeType = x.PeriodRangeType,
                    PeriodRangeName = x.PeriodRangeName
                });

                firstRobotDate = robotDateAndType.FirstRobotDate.ToString(Consts.FormatDatetimeToString_ddMMMyyyyHHmmssfff);
                lastRobotDate = robotDateAndType.LastRobotDate.ToString(Consts.FormatDatetimeToString_ddMMMyyyyHHmmssfff);
                robotTypeClassify = robotDateAndType.RobotTypeClassify;
            }

            return Json(new
            {
                firstRobotDate = firstRobotDate,
                lastRobotDate = lastRobotDate,
                robotTypeClassify = robotTypeClassify,
                data = robotBettingList,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetRobotBettingTicketDetail(long custID, int robotTypeOrigin, int? robotTypeDisplay, int? clusterId, string createdTime)
        {
            var result = Enumerable.Empty<RobotBettingDetailViewModel>();

            try
            {
                var listTrans = _robotBettingService.GetTranIds(custID, robotTypeOrigin, robotTypeDisplay, clusterId, createdTime);

                if (listTrans != null && listTrans.Any())
                {
                    var transToString = _robotBettingService.GetTransFromList(listTrans);

                    var isSCETicketDetails = (robotTypeOrigin == (int)RobotDetectionReasonQueryType.ConsecutiveTicket
                                            || robotTypeOrigin == (int)RobotDetectionReasonQueryType.UglyStakeTicket) ? true : false;

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
                LogService.Error(ex.Message);

                result = Enumerable.Empty<RobotBettingDetailViewModel>();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetRobotBettingReasonDetail(long custID, int robotTypeOrigin)
        {
            var inputParas = new RobotBettingSceDetailParaModel { CustId = custID, RobotTypeOrigin = robotTypeOrigin, QueryType = RobotDetectionQueryType.RobotReasonDetail };
            var robotReasonDetail = _robotBettingService.GetRobotDetectionSCEDetails(inputParas);

            return Json(new
            {
                childTableId = new Random().Next(),
                reasonGroups = robotReasonDetail?.ToList().OrderBy(c => c.FirstTransDate) ?? Enumerable.Empty<RobotUserDetailSceModel>()
            }, JsonRequestBehavior.AllowGet); ;
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

        #endregion Robot Betting

        #region Betting Statistic

        [NapAuth(FunctionKey = FSFunctionKeys.BettingStatistics, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetPerformanceBySport(long custId, int getTop)
        {
            var tasks = new List<Task>();
            var result = _accountService.GetPerformanceBySport(custId, getTop)?.OrderBy(c => c.Ranking);
            var totalSportBettingStatistic = new SportBettingStatisticViewModel();
            var bestSportBettingStatistic = new SportBettingStatisticViewModel();
            var pieChartSportStatisticSessions = new List<SportStatisticPieChartViewModel>();

            if (result != null && result.Any())
            {
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        totalSportBettingStatistic.TurnOver = result.Sum(c => c.TurnOver);
                        totalSportBettingStatistic.WinLoss = result.Sum(c => c.WinLoss);
                        totalSportBettingStatistic.BetCount = result.Sum(c => c.BetCount);
                    }
                    catch (Exception ex)
                    {
                        LogService.Error(ex.Message);

                        throw;
                    }
                }));

                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        var tempBestSport = result.FirstOrDefault(c => c.Ranking == result.Min(x => x.Ranking));
                        bestSportBettingStatistic.SportID = tempBestSport.SportID;
                        bestSportBettingStatistic.SportName = tempBestSport.SportName;
                        bestSportBettingStatistic.TurnOver = tempBestSport.TurnOver;
                        bestSportBettingStatistic.WinLoss = tempBestSport.WinLoss;
                        bestSportBettingStatistic.BetCount = tempBestSport.BetCount;
                    }
                    catch (Exception ex)
                    {
                        LogService.Error(ex.Message);

                        throw;
                    }
                }));

                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        CalculatePieChartTurnOverPercentage(result, pieChartSportStatisticSessions);
                    }
                    catch (Exception ex)
                    {
                        LogService.Error(ex.Message);

                        throw;
                    }
                }));

                Task.WhenAll(tasks.ToArray()).Wait();
            }

            return Json(new
            {
                totalSportBettingStatistic,
                bestSportBettingStatistic,
                pieChartSportStatisticSessions
            }, JsonRequestBehavior.AllowGet);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.BettingStatistics, Policy = NAP.Core.Business.Constant.Policy.Read)]
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

        [NapAuth(FunctionKey = FSFunctionKeys.BettingStatistics, Policy = NAP.Core.Business.Constant.Policy.Read)]
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

            bool temp = userService.IsHavingPermission(CurrentSession.UserId, NapSettings.Settings.SiteId, FSFunctionKeys.AccountDetail, NAP.Core.Business.Constant.Policy.Read);

            if (!temp) return false;

            return temp = userService.IsHavingPermission(CurrentSession.UserId, NapSettings.Settings.SiteId, FSFunctionKeys.BettingStatistics, NAP.Core.Business.Constant.Policy.Read);
        }

        #endregion Betting Statistic

    }
}