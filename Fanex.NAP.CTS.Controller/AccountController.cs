namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.Account.RobotBetting;
    using Fanex.NAP.CTS.Model.Association;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Model.ImportEvidences;
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
    using Fanex.NAP.CTS.ViewModel.Account.CustomerInfo;
    using Fanex.NAP.CTS.ViewModel.Account.Evidence;
    using Fanex.NAP.CTS.ViewModel.Account.RobotBetting;
    using Fanex.NAP.CTS.ViewModel.Account.TransactionHistory;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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
                            var custDangerous = _accountService.GetCustomerInfoByCustIds(customerInfoFromCTS.CustID, 2);

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
                            var robotUser = _robotClassificationService.GetStatusRobotClassification(customerInfoFromCTS.CustID);

                            customerGeneralStatusViewModel.BindingRobotUser(robotUser);
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
            var result = _accountService.GetCustomerClassFromCTSByCustId(custId.ToString());
            var customerClassification = string.Empty;

            if (result != null && result.Any())
            {
                customerClassification = result.First().CustClass.ToString();
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
                    var customerClass = _accountService.GetCustomerInfoClass(custId.ToString());

                    if (customerClass != null)
                        custProfileEvidence.Class = customerClass.CustomerClass != -1 ? customerClass.CustomerClass.ToString() : "";
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
                    var custPolicies = _accountService.GetCustomerInfoPolicies(custId);

                    if (custPolicies != null)
                        custProfileEvidence.LicVIP = custPolicies.IsVIP ? "Yes" : "No";
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
            var custClassification = new List<LatestClassification4ReportModel>();

            if (!string.IsNullOrEmpty(custIds))
            {
                custClassification = _accountService.GetCustomerClassFromCTSByCustId(custIds)?.ToList();
            }

            var result = assModel.GroupJoin(custClassification,
                                 a => a.CustId,
                                 b => b.CustId,
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
                                          CustomerClasses = (temp != null && temp.CustClass != null && group.a.CustSubId == 0) ? temp.CustClass.ToString() : "-",
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

                    var matchesInfo = _accountService.GetMatchesInfo(string.Join(",", matchIDs), custID1 + "," + custID2);

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

                if(groupBettingAssociation.ErrorCode != 0)
                {
                    LogService.Error($"Association Detail throws exception - {groupBettingAssociation.ErrorMessage} - {JsonConvert.SerializeObject(new { para1, para2, originGroupIDs })}");
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
                            if (generalGBs != null)
                            {
                                var groupInfo = _accountService.GetCustGroupAssociation(para.GroupIDs);

                                if (groupInfo.Any())
                                {
                                    var matchGroupInfo = _accountService.MergeCustMatchInfo(generalGBs, groupInfo);

                                    var custIds = string.Join(",", matchGroupInfo.Select(c => c.CustID).Distinct());

                                    var ggbTicketDetails = _accountService.GetTicketDetails(para, custIds, null);

                                    if (ggbTicketDetails != null && ggbTicketDetails.Any())
                                    {
                                        ggbData = _accountService.MergeCustGroupTicketInfo(matchGroupInfo, ggbTicketDetails, true);
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

                        return new AssociationByIPItemViewModel
                        {
                            MatchID = x.MatchID,
                            EventName = homeName != string.Empty ? (homeName + " - vs - " + awayName) : "-",
                            LeagueName = leagueName != string.Empty ? leagueName : "-",
                            EventDateString = x.EventDate != DateTime.MinValue
                                ? x.EventDate.ToString(Consts.FormatDateToString_MMMddyyyy)
                                : "-",
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
            ViewBag.EvidenceList = _accountService.GetEvidenceList();

            return PartialView("ChainToEvidence/ChainToEvidence");
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
                    _customerEvidenceService.UpdateBetLimitOfUnflaggedCustomer(string.Join(",", custIdsFlaggedEvidences.Select(c => c.CustID)));
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
                    _customerEvidenceService.UpdateBetLimitOfFlaggedCustomer(string.Join(",", custExistEvidence.Select(c => c.CustID)));
                }
            }

            return Json(new
            {
                errorCode = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileChainstoEvidenceInsertEvidence)]
        public ActionResult PlaceEvidence(UpdatePlaceEvidenceModel model)
        {
            model.UserId = model.UserId == 0 ? CurrentSession.UserId : model.UserId;
            var result = _accountService.ProcessPlaceEvidence(model);

            if (result.ErrorCode == 0)
            {
                var objects = new[] { new CustEvidenceModel {
                    CTSCustId = model.CTSCustId,
                    CustId = model.CustId,
                    SubscriberId = model.SubscriberId,
                    EvidenceId = model.EvidenceId,
                    ActionType = 0
                } };

                var syncCustIds = Enumerable.Empty<long>();     

                var paLosingClassification = _rescanClassificationService.GetProblemAccountLosing(objects, FunctionId.PlaceSingleEvidence);

                var custEvidenceJson = JsonConvert.SerializeObject(paLosingClassification);

                var syncResult = _accountService.SyncCustEvidenceToCategory(custEvidenceJson, false, out syncCustIds);

                if (syncResult.ErrorCode == 0)
                {
                    _customerClassificationService.PushDataToExternal(syncCustIds, new PushDataToExternalModel
                    {
                        ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
                        IsPushCCToSoon88 = true,
                        IsUpdateDangerToMainDB = true,
                        DangerLevelType = DangerLevel.ABI
                    });

                    var tasks = new List<Task>
                    {
                        Task.Run(() =>
                        {
                             _accountService.CheckNewPAAssociationCustomerForRollback(syncResult.CustInfo);
                        }),
                        Task.Run(() =>
                        {
                            _antiFraudCenterService.InsertAFCAccounts(syncCustIds, FromFlow.CTSWEB);
                        })
                    };

                    Task.WhenAll(tasks.ToArray());
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileChainstoEvidenceRemoveEvidence)]
        public ActionResult RemoveFlaggedEvidence(RemoveEvidenceModel model)
        {
            model.UserId = CurrentSession.UserId;

            var result = _accountService.RemoveFlaggedEvidence(model);

            if (result.ErrorCode == 0)
            {              
                var objects = new[] { new {
                   CTSCustID = model.CTSCustId,
                   CustID = model.CustId,
                   SubscriberID = model.SubscriberId,
                   EvidenceID = model.EvidenceId,
                   ActionType = 2
                } };

                var syncCustIds = Enumerable.Empty<long>();
                var custEvidenceJson = JsonConvert.SerializeObject(objects);

                // Sync Evidence to Category
                var syncResult = _accountService.SyncCustEvidenceToCategory(custEvidenceJson, false, out syncCustIds);

                if (syncResult.ErrorCode == 0)
                {
                    // Rescan Problem Account & Normal Account
                    _rescanClassificationService.RescanCustomerClassify(new RescanCustClassificationModel
                    {
                        CustIds = syncCustIds,
                        IsCheckPAFromNAP = true,
                        IsPushCCToSoon88 = true,
                        FunctionId = FunctionId.RemoveFlaggedEvidenceWeb,
                        IsUpdateDangerToMainDB = true,
                        DangerLevelType = DangerLevel.ABI
                    });
                }

                var custIdsFlaggedEvidences = _customerEvidenceService.CheckCleanCustFlaggedEvidence(model.CTSCustId.ToString());

                if (custIdsFlaggedEvidences != null && custIdsFlaggedEvidences.Any())
                {
                    _customerEvidenceService.UpdateBetLimitOfUnflaggedCustomer(string.Join(",", custIdsFlaggedEvidences.Select(c => c.CustID)));
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

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
                    _customerEvidenceService.UpdateBetLimitOfUnflaggedCustomer(string.Join(",", custIdsFlaggedEvidences.Select(c => c.CustID)));
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

                    _customerEvidenceService.UpdateBetLimitOfFlaggedCustomer(custId.ToString());
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
                TransId = x.TransId,
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

        public ActionResult GetCustomerClassification(long ctsCustId)
        {
            var isPinCategory = false;
            var isExcludedTagAssPA = false;
            var isVVip = false;
            var isProblemAccount = false;
            var isPAProbation = false;
            var categoryList = new List<CustomerCategoryViewModel>();
            var classificationHistoryList = new List<CustomerClassificationHistoryViewModel>();

            var classification = _accountService.GetCustomerClassification(ctsCustId, out isPinCategory, out isExcludedTagAssPA);

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

                isVVip = categoryList.Any(x => x.CategoryName.Equals("VVIP"));

                isProblemAccount = categoryList.Any(x => x.IsProblemAccount);

                isPAProbation = isProblemAccount && classification.Any(x => x.IsProbation);
            }

            var classificationHistory = _accountService.GetCustomerClassificationHistory(ctsCustId);

            if (classificationHistory != null && classificationHistory.Any())
            {
                classificationHistoryList = classificationHistory.Select(x => new CustomerClassificationHistoryViewModel
                {
                    CreatedTimeString = x.CreatedTime.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                    SourceTypeName = x.SourceTypeName,
                    CategoryName = x.CategoryName != null ? x.CategoryName : string.Empty,
                    CustomerClass = x.CustomerClass,
                    Reason = GetReason(x),
                    TurnOver = x.TurnOver,
                    Winloss = x.Winloss,
                    Margin = x.Margin,
                    BetCount = x.BetCount,
                    ActiveDays = x.ActiveDays,
                    LastXDaysMargin = x.LastXDaysMargin,
                    ProbationPeriodWinloss = x.ProbationPeriodWinloss,
                    CreatedBy = string.IsNullOrEmpty(x.CreatedBy) ? x.LastModifiedBy.ToString() : x.CreatedBy,
                    TaggingType = x.TaggingType
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

        private string GetReason(CustomerClassificationHistoryModel cc)
        {
            var reason = $"<div><span> { cc.SourceTypeName } </span>";

            if (!string.IsNullOrEmpty(cc.TaggingType))
            {
                reason += $"<span> | Normal Category Status: { cc.TaggingType }</span>";
            }

            reason += "<br />";

            var margin = Math.Round(cc.Margin, 3);
            var winlost = Math.Round(cc.Winloss, 3);
            var turnOver = Math.Round(cc.TurnOver, 3);
            var betCount = cc.BetCount;
            var activeDays = cc.ActiveDays;
            var last7DayMargin = Math.Round(cc.LastXDaysMargin, 3);
            var winlost20Days = Math.Round(cc.ProbationPeriodWinloss, 3);
            var twBetCount = cc.TWBetCount;
            var twGroupBettingRate = cc.TWGroupBettingRate;
            var twTicketRejectRate = cc.TWTicketRejectRate;
            var twDesktopTicket = cc.TWDesktopUsageRate;
            var robotCounter = cc.RobotCounter;

            switch (cc.CategoryID)
            {
                case (int)ClassificationSourceType.NormalAccount:
                case (int)ClassificationSourceType.NormalAccount121:
                case (int)ClassificationSourceType.NormalAccount122:
                case (int)ClassificationSourceType.NormalAccount123:
                    reason += $"Margin: {margin} - Bet Count: {betCount}";
                    break;

                case (int)ClassificationSourceType.GoodMember:
                case (int)ClassificationSourceType.GoodMember111:
                case (int)ClassificationSourceType.GoodMember112:
                case (int)ClassificationSourceType.GoodMember113:
                    reason += $"Margin: {margin} - Turn Over: {turnOver} - Active Days: {activeDays} - Bet Count: {betCount}";
                    break;

                case (int)ClassificationSourceType.Probation:
                case (int)ClassificationSourceType.Probation51:
                case (int)ClassificationSourceType.Probation52:
                case (int)ClassificationSourceType.Probation53:
                    if (cc.SourceTypeID == 5)
                        reason += $"Margin: {margin} - Win Lost: {winlost} - Bet Count: {betCount} - Last 7 day's Margin: {last7DayMargin} - WinLost(20 days): {winlost20Days}";
                    else
                        reason += $"Margin: {margin}";
                    break;

                case (int)ClassificationSourceType.SmartPunter:
                case (int)ClassificationSourceType.SmartPunter61:
                case (int)ClassificationSourceType.SmartPunter62:
                case (int)ClassificationSourceType.SmartPunter63:
                case (int)ClassificationSourceType.HighRiskPunter:
                case (int)ClassificationSourceType.HighRiskPunter71:
                case (int)ClassificationSourceType.HighRiskPunter72:
                case (int)ClassificationSourceType.HighRiskPunter73:
                    if (cc.SourceTypeID == 4)
                        reason += $"Margin: {margin} - Win Lost: {winlost} - Bet Count: {betCount} - Last 7 day's Margin: {last7DayMargin} - WinLost(20 days): {winlost20Days}";
                    else
                        reason += $"Margin: {margin} - Win Lost: {winlost} - Bet Count: {betCount}";
                    break;

                case (int)ClassificationSourceType.Probation57:
                case (int)ClassificationSourceType.SmartPunter67:
                case (int)ClassificationSourceType.HighRiskPunter77:
                    reason += $"Account has total {twBetCount} tickets | Group betting percent is {twGroupBettingRate}%";
                    break;

                case (int)ClassificationSourceType.Probation58:
                case (int)ClassificationSourceType.SmartPunter68:
                case (int)ClassificationSourceType.HighRiskPunter78:
                    reason += $"Account has {twDesktopTicket}% tickets from Desktop | Reject percent is {twTicketRejectRate}%";
                    break;

                case (int)ClassificationSourceType.Robot61:
                case (int)ClassificationSourceType.Robot81:
                case (int)ClassificationSourceType.Robot161:
                    reason += $"Counter: {robotCounter}";
                    break;

                default:
                    {
                        if (cc.SourceTypeID == 23)
                        {
                            reason += $"Margin: {margin} - Turn Over: {turnOver} - Active Days: {activeDays} - Bet Count: {betCount}";
                        }    

                        break;
                    }

            }

            reason += "</div>";

            return reason;
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult MarkCatergory(long ctsCustId)
        {
            var customerCategoryList = _accountService.GetCustomerCategoryByCTSCustId(ctsCustId).ToList();

            var model = new MarkCategoryViewModel
            {
                CustomerCategoryList = customerCategoryList.Where(x => x.CategoryName.Equals("VVIP") || x.ParentId == 0).ToList()
            };

            return PartialView("CustomerCategory/MarkCatergory", model);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult RemoveCatergory(long ctsCustId)
        {
            var model = new MarkCategoryViewModel
            {
                CustomerCategoryList = new List<Model.DTO.CustomerCategory> { new Model.DTO.CustomerCategory { CategoryId = 210, CategoryName = "VVIP", ParentId = 200 } }
            };

            return PartialView("CustomerCategory/RemoveCategory", model);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassificationMarkCategory)]
        public ActionResult AddCustomerCategory(AddCustomerCategoryModel addCategoryModel)
        {
            var errorCode = 0;
            var errorMessage = string.Empty;
            var latestClassifications = Enumerable.Empty<LatestClassificationModel>();

            var resultToCTS = _accountService.AddCustomerCategory(addCategoryModel, CurrentSession.UserId, out latestClassifications);

            if (resultToCTS.ErrorCode == 0 && latestClassifications != null)
            {
                if (addCategoryModel.IsVVip)
                {
                    int custIdInt = 0;
                    int.TryParse(addCategoryModel.CustId.ToString(), out custIdInt);

                    if (custIdInt != 0)
                    {
                        latestClassifications = latestClassifications.Where(c => c.CustId != 0);

                        if (latestClassifications.Any())
                        {
                            // Update CC in table custinfo to MainDB
                            var isSuccessToMain = _customerClassificationService.RetryPushCCToMainDB(latestClassifications, 2);

                            if (isSuccessToMain)
                            {
                                var updateApplyCCs = latestClassifications.Select(x => new { x.CustId, x.ScannedTime });

                                var jsonUpdateApplyCCs = JsonConvert.SerializeObject(updateApplyCCs);

                                // Update IsAppliedCC = 1 in CTS
                                _customerClassificationService.UpdateAppliedCustomerClassification(jsonUpdateApplyCCs);
                            }
                        }

                        Task.Run(() =>
                        {
                            _antiFraudCenterService.DeleteAFCAccounts(new List<long> { custIdInt }, FromFlow.CTSWEB);
                        });
                    }
                }
            }
            else
            {
                errorCode = resultToCTS.ErrorCode;
                errorMessage = resultToCTS.ErrorMsg;
            }

            return Json(new
            {
                errorCode,
                errorMessage
            }, JsonRequestBehavior.AllowGet);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassificationRemoveCategory)]
        public ActionResult RemoveCustomerCategory(long custId, long ctsCustId, int categoryId, bool isVVip)
        {
            var errorCode = 0;
            var errorMessage = string.Empty;

            var resultToCTS = _accountService.RemoveCustomerCategory(custId, ctsCustId, categoryId, CurrentSession.UserId);

            if (string.IsNullOrEmpty(resultToCTS.OpErrorMessage))
            {
                if (isVVip)
                {
                    var custIdInt = 0;
                    int.TryParse(custId.ToString(), out custIdInt);

                    if (custIdInt != 0)
                    {
                        // Rescan Problem Account & Normal Account
                        _rescanClassificationService.RescanCustomerClassify(new RescanCustClassificationModel
                        {
                            CustIds = new List<long> { custIdInt },
                            IsCheckPAFromNAP = true,
                            IsPushCCToSoon88 = true,
                            FunctionId = FunctionId.RemoveVIPWeb,
                            IsUpdateDangerToMainDB = true,
                            DangerLevelType = DangerLevel.ABI
                        });
                    }
                }
            }
            else
            {
                errorCode = -1;
                errorMessage = resultToCTS.OpErrorMessage;
            }

            return Json(new
            {
                errorCode,
                errorMessage
            }, JsonRequestBehavior.AllowGet);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassificationPinCategory)]
        [HttpPost]
        public ActionResult PinCategoryCustomerClassification(long ctsCustId, int subscriberId, bool isPinCategory)
        {
            var result = _accountService.PinCategoryCustomerClassification(ctsCustId, subscriberId, isPinCategory, CurrentSession.UserId);

            return Json(new
            {
                errorCode = result.ErrorCode,
                errorMessage = result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileCustomerClassificationNormalCategory)]
        [HttpPost]
        public ActionResult ChangeExcludedTagAssPA(int custId, long ctsCustId, int subscriberId, bool isExcludedTagAssPA, bool isProblemAccount, int custSubId)
        {
            var result = _accountService.ChangeExcludedTagAssPA(ctsCustId, subscriberId, isExcludedTagAssPA, CurrentSession.UserId);

            if (result.ErrorCode == 0
                && !isProblemAccount
                && custSubId == 0)
            {
                _rescanClassificationService.RescanCustomerClassify(new RescanCustClassificationModel
                {
                    CustIds = new List<long> { custId },
                    IsCheckPAFromNAP = true,
                    IsPushCCToSoon88 = true,
                    FunctionId = FunctionId.ExcludeTaggingAssPAWeb,
                    IsUpdateDangerToMainDB = true,
                    DangerLevelType = DangerLevel.ABI
                });
            }

            return Json(new
            {
                errorCode = result.ErrorCode,
                errorMessage = result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion Customer Category

        #region Robot Classification

        public ActionResult RobotClassification()
        {
            return PartialView("RobotClassfication/LastestRobotClassification");
        }

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AccountProfileRobotClassification)]
        public ActionResult TrackingRobotClassification()
        {
            return null;
        }

        [HttpPost]
        public ActionResult GetHistoryRobotClassification(long custId)
        {
            var data = Enumerable.Empty<RobotClassificationViewModel>();
            var bettingStatisticFormat = "Turnover: {0} | Win lost: {1} | Margin: {2}% | Bet count: {3} | Active days: {4}";

            var robotData = _robotClassificationService.GetHistoryRobotClassification(custId);

            if (robotData != null && robotData.Any())
            {
                data = robotData.OrderByDescending(d => d.FromDate).Select(x =>
                {
                    decimal margin = 0;

                    if (x.Turnover != 0)
                    {
                        margin = x.Winloss / x.Turnover * 100;
                    }

                    return new RobotClassificationViewModel
                    {
                        ScannedDate = x.FromDate.ToString(Consts.FormatDateToString_MMMddyyyy),
                        BettingStatistic = string.Format(bettingStatisticFormat
                                         , Math.Round(x.Turnover, 3).ToString("0.000").Replace(".000", string.Empty)
                                         , Math.Round(x.Winloss, 3).ToString("0.000").Replace(".000", string.Empty)
                                         , Math.Round(margin, 3).ToString("0.000").Replace(".000", string.Empty)
                                         , x.BetCount, x.ActiveDays),
                        RobotType = x.RobotType
                    };
                });
            }

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

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
            var robotBettingList = Enumerable.Empty<RobotBettingViewModel>();

            var robotBettings = _robotBettingService.GetRobotBetting(custID, out RobotDateModel robotDate);

            if (robotBettings != null && robotBettings.Any())
            {
                robotBettingList = robotBettings.Select(x => new RobotBettingViewModel
                {
                    CustID = x.CustID,
                    PeriodRangeType = x.PeriodRangeType,
                    PeriodRangeName = x.PeriodRangeName
                });

                firstRobotDate = robotDate.FirstRobotDate.ToString(Consts.FormatDatetimeToString_ddMMMyyyyHHmmssfff);
                lastRobotDate = robotDate.LastRobotDate.ToString(Consts.FormatDatetimeToString_ddMMMyyyyHHmmssfff);
            }

            return Json(new
            {
                firstRobotDate = firstRobotDate,
                lastRobotDate = lastRobotDate,
                data = robotBettingList,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetRobotBettingDetail(long custID, int periodRangeType)
        {
            var result = Enumerable.Empty<RobotBettingDetailViewModel>();

            var robotUserDetails = _robotBettingService.GetRobotUserDetails(custID, periodRangeType);

            if (robotUserDetails != null && robotUserDetails.Any())
            {
                var suspiciousTransList = robotUserDetails.Select(x => x.SuspiciousTransList);

                var tranIdList = GetTranIdList(suspiciousTransList);

                var ticketDetail = _robotBettingService.GetRobotBettingTicketDetails(tranIdList);

                if (ticketDetail != null && ticketDetail.Any())
                {
                    result = ticketDetail.Select(x => new RobotBettingDetailViewModel
                    {
                        TransDate = x.TransDate.ToString(Consts.FormatDatetimeToString_ddMMMyyyyHHmmssfff),
                        HomeID = x.HomeId,
                        AwayID = x.AwayId,
                        TransID = x.TransId,
                        BetType = x.BetType,
                        BetTypeName = CachedRef.GetBettypeName(x.BetType),
                        BetTeam = x.BetTeam,
                        Stake = x.Stake,
                        Odds = x.Odds,
                        LiveScore = x.LiveIndicator ? (x.LiveHomeScore + " - " + x.LiveAwayScore) : "-",
                        MatchName = GetMatchName(x)
                    }).OrderByDescending(x => x.TransDate);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string GetTranIdList(IEnumerable<string> suspiciousTransList)
        {
            var tranIdList = new List<string>();

            foreach (var level1 in suspiciousTransList)
            {
                var susTranIdsLevel1 = level1.Split(',');

                foreach (var level2 in susTranIdsLevel1)
                {
                    var susTranIdsLevel2 = level2.Split('#');

                    if (susTranIdsLevel2.Length > 1)
                    {
                        tranIdList.AddRange(susTranIdsLevel2);
                    }
                    else
                    {
                        tranIdList.Add(level2);
                    }
                }
            }

            var tranIds = string.Join(",", tranIdList.Distinct());

            return tranIds.Replace(@".0", "");
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
                        totalSportBettingStatistic.BetCount14 = result.Sum(c => c.BetCount14);
                        totalSportBettingStatistic.TurnOver14 = result.Sum(c => c.TurnOver14);
                    }
                    catch (Exception ex)
                    {
                        LogService.Info(ex.Message);

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
                        bestSportBettingStatistic.TurnOver14 = tempBestSport.TurnOver14;
                        bestSportBettingStatistic.BetCount14 = tempBestSport.BetCount14;
                    }
                    catch (Exception ex)
                    {
                        LogService.Info(ex.Message);

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
                        LogService.Info(ex.Message);

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