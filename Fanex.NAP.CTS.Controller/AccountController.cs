namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.Extentions;    
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;    
    using Fanex.NAP.CTS.Model.Association;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Model.Transaction;
    using Fanex.NAP.CTS.Resource;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.Account;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using Fanex.NAP.CTS.ViewModel.Account.BettingStatistic;
    using Fanex.NAP.CTS.ViewModel.Account.ChainToEvidence;
    using Fanex.NAP.CTS.ViewModel.Account.CustomerCategory;
    using Fanex.NAP.CTS.ViewModel.Account.TransactionHistory;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IGraphService _graphService;
        private readonly ITransactionService _transService;
        private readonly ISubscriberAssignmentService _subscriberAssignmentService;
        private readonly ICustomerEvidenceService _customerEvidenceService;    
        private readonly ICustomerClassifyService _customerClassifyService;
        private readonly IPushDataToExternalService _pushDataToExternalService;

        public override string BaseControllerName { get => string.Empty; }

        public AccountController()
        {
            _accountService = new AccountService();
            _graphService = new GraphService();
            _subscriberAssignmentService = new SubscriberAssignmentService();
            _transService = new TransactionService();
            _customerEvidenceService = new CustomerEvidenceService();
            _customerClassifyService = new CustomerClassifyService();
            _pushDataToExternalService = new PushDataToExternalService();
        }

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
                        IsBettingStatisticPermission = CheckBettingStatisticPermission(userService, customerInfoFromCTS.RoleId),
                        IsViewIPTransactionPermission = userService.IsHavingPermission(CurrentSession.UserId, NapSettings.Settings.SiteId, FSFunctionKeys.ViewIP, NAP.Core.Business.Constant.Policy.Read),
                        CurrentUserId = CurrentSession.UserId
                    };

                    var customerLastTicketDate = _accountService.GetCustomerLastTicketDate(customerInfoFromCTS.CustId);

                    customerGeneralStatusViewModel
                        .BindingGeneralDataFromCTS(customerInfoFromCTS)
                        .BindingGeneralDataFromMainDB(customerLastTicketDate);

                    return View(customerGeneralStatusViewModel);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult GetAccountProfile(long custId, long ctsCustId, int subscriberId)
        {
            // LogService.Info("GetAccountProfile - Before: " + DateTime.Now.ToString());
            var profile = new AccountProfileViewModel();
            var tasks = new List<Task>();

            tasks.Add(Task.Run(() =>
            {
                try
                {
                    GetCustomerGeneralStatus(custId, ref profile);
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
                   GetCustomerProfilePoliciesInfo(custId, ref profile);
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
                    GetCustomerRobotUserStatus(custId, ref profile);
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
                    GetCustomerFlagInfo(ctsCustId, ref profile);
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
                    GetCustomerEsportAutoSuspendedStatus(custId, ref profile);
                }
                catch (Exception ex)
                {
                    LogService.Info(ex.Message);

                    throw;
                }
            }));

            Task.WhenAll(tasks.ToArray()).Wait();

            //LogService.Info("GetAccountProfile - After: " + DateTime.Now.ToString());

            return Json(profile, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCustomerTotalAssociationByCTSCustId(long ctsCustId)
        {
            // LogService.Info("GetCustomerTotalAssociationByCTSCustId - Before: " + DateTime.Now.ToString());
            var result = _accountService.GetCustomerTotalAssociationByCTSCustId(ctsCustId, ConfigSettings.GetStatisticAssociation);
            // LogService.Info("GetCustomerTotalAssociationByCTSCustId - After: " + DateTime.Now.ToString());

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
        public ActionResult GetCustomerGeneralStatus(long custId)
        {
            var model = new CustomerGeneralStatusViewModel();

            var result = _accountService.GetCustomerGeneralStatus(custId.ToString()).FirstOrDefault();
            var accountStatusName = string.Empty;

            if (result != null)
            {
                accountStatusName = model.GetAccountStatusName(model.GetAccountStatus(result));
            }
            else
            {
                accountStatusName = model.GetAccountStatusName(AccountStatus.NA);

                result = new CustomerGeneralStatusModel();
            }

            return Json(new { customerStatus = result, accountStatus = accountStatusName }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCustomerRobotUserStatus(long custId)
        {
            var result = _accountService.GetCustomerRobotUserStatus(custId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCustomerEsportAutoSuspendedStatus(long custId)
        {
            var result = _accountService.GetCustomerEsportAutoSuspendedStatus(custId);

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
            var result = _accountService.GetCustomerClassByCustId(custId.ToString());
            var customerClassification = string.Empty;

            if (result != null && result.Any())
            {
                customerClassification = result.First().CustomerClass.ToString();
            }

            return Json(new { customerClassification = customerClassification }, JsonRequestBehavior.AllowGet);
        }

        #region Associated By Account

        public ActionResult AssociatedByAccount(string username)
        {
            var getAssociationStatus = _accountService.GetCustomerStaticStatus(3, true);

            getAssociationStatus.Insert(0, new CustomerStaticStatusModel { ItemId = 0, ItemNameDisplay = "All" });

            var getCustomerStaticAssType = _accountService.GetCustomerStaticStatus(2, true);

            getCustomerStaticAssType.Insert(0, new CustomerStaticStatusModel { ItemId = 0, ItemNameDisplay = "All" });

            var accountStatus = _accountService.GetAccountStaticStatuses();
            var subscribers = _accountService.GetAllSubscriberGroups();

            var associatedByAccountViewModel = new AssociatedByAccountViewModel
            {
                AssociationStatus = getAssociationStatus,
                AssociationTypeList = getCustomerStaticAssType,
                AccountStatusList = accountStatus,
                SubscribersList = subscribers.ToList(),
                Username = username
            };

            return PartialView("Associated/AccountByAccount", associatedByAccountViewModel);
        }

        [HttpPost]
        public ActionResult GetAssociatedByAccount(AssociatedFilterModel model)
        {
            var take = model.length;
            var skip = model.start;
            var totalItem = 0;

            var result = new List<CustomerAssociationByAccountDetailViewModel>();

            if (model.CTSCustId == 0)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var directAssociationUrl = GetDirectAssociationUrl(model.AssociationType);

            var graphAssociationPage = model.IsDirect
                ? _accountService.GetCustomerAssociationByGraph(directAssociationUrl, model.CTSCustId, model.AssociationStatus, model.AccountStatusIds, model.SiteIds, skip, take)
                : new GraphAssociationPaging();

            if (graphAssociationPage != null && graphAssociationPage.AssociationList != null && graphAssociationPage.AssociationList.Any())
            {
                totalItem = graphAssociationPage.TotalItems;
                var getAllSubscribers = _subscriberAssignmentService.GetSubscribers(0);

                var customerAssociation = graphAssociationPage.AssociationList;

                var custIds = string.Join(",", customerAssociation.Select(x => x.CustId));

                //var getStatusByCustIds = _accountService.GetCustomerGeneralStatus(custIds);

                //customerAssociation = _accountService.FilterCustomerAssociationByStatus(string.Empty, customerAssociation, getStatusByCustIds);

                customerAssociation = _accountService.FilterCustomerAssociationByAssociationType(model.AssociationType, customerAssociation);

                var ctsCustIdList = customerAssociation.Select(x => new { CTSCustId = x.CTSCustID, x.SubscriberId });
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
                        AssociationTypeName = x.AssociationTypeName,
                        Status = x.StatusName == "NA" ? "N/A" : x.StatusName,
                        StatusId = x.StatusId.HasValue ? x.StatusId.Value : 0,
                        SiteId = x.SiteID.HasValue ? x.SiteID.Value : 0,
                        SiteName = !string.IsNullOrEmpty(x.SiteName) ? x.SiteName : "-",
                        SubscriberId = x.SubscriberId,
                        Subscriber = getAllSubscribers.Any(aa => x.SubscriberId != -1 && aa.SubscriberId == x.SubscriberId)
                                        ? getAllSubscribers.FirstOrDefault(a => a.SubscriberId == x.SubscriberId).SubscriberName
                                        : "-",
                        CTSCustId = x.CTSCustID,
                        IsLinked = x.AssociationStatus == 1
                    };

                    var flaggedAffected = flaggedAffectedTagList.FirstOrDefault(y => y.SubscriberID.Equals(newX.SubscriberId) && y.CTSCustID.Equals(newX.CTSCustId));
                    if (flaggedAffected != null)
                    {
                        newX.IsFlaggedTag = flaggedAffected.IsFlagged;
                        newX.IsAffectedTag = flaggedAffected.IsAffected;
                    }

                    return newX;
                }).OrderBy(a => a.SiteId)
                .ThenBy(a => !a.IsLinked)
                .ThenByDescending(a => a.Status)
                .ThenByDescending(a => a.AssociationDate)
                .ToList();
            }

            return Json(new
            {
                draw = model.draw,
                recordsTotal = totalItem,
                recordsFiltered = totalItem,
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        private string GetDirectAssociationUrl(int associationType)
        {
            switch (associationType)
            {
                case 1:
                    return ConfigSettings.GetDirectAssociationByDevice;
                case 2:
                    return ConfigSettings.GetDirectAssociationByIP;
                case 3:
                    return ConfigSettings.GetDirectAssociationByManual;
                default:
                    return ConfigSettings.GetDirectAssociation;
            }
        }

        [HttpPost]
        public ActionResult GetDirectAssociationGraph(Int64 ctsCustid, string displayName, int associationStatus, string accountStatusIds, string siteIds, int start, int length)
        {
            var graphCustomer = new GraphCustomerItemViewModel();

            var graphAssociationPaging = _accountService.GetCustomerAssociationByGraph(ConfigSettings.GetDirectAssociation, ctsCustid, associationStatus, accountStatusIds, siteIds, start, length);
            if (graphAssociationPaging != null && graphAssociationPaging.AssociationList != null && graphAssociationPaging.AssociationList.Any())
            {
                var now = DateTime.Now.Ticks.ToString();
                var graphCustomerList = graphAssociationPaging.AssociationList
                    .Select(x => new GraphCustomerItemViewModel
                    {
                        CTSCustId = x.CTSCustID,
                        ItemCode = x.CTSCustID.ToString() + "_" + now,
                        DisplayName = !string.IsNullOrWhiteSpace(x.Username) ? x.Username : x.Username2,
                        AssociationStatus = associationStatus,
                        Start = start,
                        Length = length
                    }).ToList();

                graphCustomer.children = graphCustomerList.ToArray();
            }

            graphCustomer.CTSCustId = ctsCustid;
            graphCustomer.DisplayName = displayName;
            graphCustomer.AssociationStatus = associationStatus;
            graphCustomer.Start = start;
            graphCustomer.Length = length;

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
        #endregion

        #region Chain To Evidence
        public ActionResult ChainToEvidence()
        {
            ViewBag.EvidenceList = _accountService.GetEvidenceList();

            return PartialView("ChainToEvidence/ChainToEvidence");
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
        public ActionResult AddException(UpdateAccountExceptionModel model)
        {
            model.UserId = CurrentSession.UserId;

            var result = _accountService.AddAccountException(model);

            if (result == 0)
            {
                var associatedAccountList = _customerEvidenceService.GetCleanAccounts(
                      new GetCleanAccountByCTSCustIdsCriteria
                      {
                          CTSCustIdList = model.FromCTSCustId.ToString() + "," + model.ToCTSCustId.ToString()
                      });

                if (associatedAccountList.Any())
                {
                    _customerEvidenceService.UpdateBetLimitOfUnflaggedCustomer(
                        new UpdateBetLimitByEvidenceRemovingCriteria
                        {
                            CustIDList = string.Join(",", associatedAccountList)
                        });
                }
            }

            return Json(new { errorCode = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult RemoveException(UpdateAccountExceptionModel model)
        {
            model.UserId = CurrentSession.UserId;

            var result = _accountService.RemoveAccountException(model);

            if (result == 0)
            {
                var associatedAccountList = _customerEvidenceService.GetAssociatedAccounts(
                          new GetAssociatedAccountByCTSCustIdsCriteria
                          {
                              CTSCustIdList = model.FromCTSCustId.ToString() + "," + model.ToCTSCustId.ToString()
                          });

                if (associatedAccountList.Any())
                {
                    _customerEvidenceService.UpdateBetLimitOfFlaggedCustomer(
                        new UpdateBetLimitByEvidenceAddingCriteria
                        {
                            CustIDList = string.Join(",", associatedAccountList)
                        });
                }
            }

            return Json(new
            {
                errorCode = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult PlaceEvidence(UpdatePlaceEvidenceModel model)
        {
            model.UserId = model.UserId == 0 ? CurrentSession.UserId : model.UserId;
            var result = _accountService.ProcessPlaceEvidence(model);
            
            if (result.ErrorCode == 0)
            {
                var objects = new[] { new {
                    model.CTSCustId,
                    model.CustId,                    
                    model.SubscriberId,
                    model.EvidenceId,
                    ActionType = 0
                } };
                var syncCustIds = Enumerable.Empty<int>();
                var custEvidenceJson = JsonConvert.SerializeObject(objects);
                // Sync Evidence to Category
                var syncResult =_accountService.SyncCustEvidenceToCategory(custEvidenceJson, false, out syncCustIds);

                if (syncResult.ErrorCode == 0)
                {
                    var lastestClassifications = _customerClassifyService.GetLatestClassification(string.Join(",", syncCustIds));

                    if (lastestClassifications != null && lastestClassifications.Any())
                    {
                        // Update CC in table custinfo to MainDB
                        var isSuccessToMain = _pushDataToExternalService.PushCustomerClassificationToMain(lastestClassifications, 2);

                        if (isSuccessToMain)
                        {
                            var updateApplyCCs = lastestClassifications.Select(x => new { x.CustId, x.ScannedTime });

                            var jsonUpdateApplyCCs = JsonConvert.SerializeObject(updateApplyCCs);

                            // Update IsAppliedCC = 1 in CTS
                            _customerClassifyService.UpdateAppliedCustomerClassification(jsonUpdateApplyCCs);
                        }

                        // Push CC to Soon88
                        _pushDataToExternalService.PushCustomerClassificationToSoon88(lastestClassifications);
                    }                   
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult RemoveFlaggedEvidence(RemoveEvidenceModel model)
        {
            model.UserId = CurrentSession.UserId;

            var result = _accountService.RemoveFlaggedEvidence(model);

            if (result.ErrorCode == 0)
            {
                var objects = new[] { new {
                    model.CTSCustId,
                    model.CustId,                    
                    model.SubscriberId,
                    model.EvidenceId,
                    ActionType = 2
                } };

                var syncCustIds = Enumerable.Empty<int>();
                var custEvidenceJson = JsonConvert.SerializeObject(objects);

                // Sync Evidence to Category
                var syncResult = _accountService.SyncCustEvidenceToCategory(custEvidenceJson, false, out syncCustIds);

                if (syncResult.ErrorCode == 0)
                {
                    // Rescan Problem Account & Normal Account
                    _customerClassifyService.RescanCustomerClassify(syncCustIds.ToList(), true, true);
                }

                var associatedAccountList = _customerEvidenceService.GetRemoveEvidenceAssociatedAccounts(
                    new GetRemoveEvidenceAssociatedAccountCriteria
                    {
                        CTSCustIdList = model.CTSCustId.ToString(),
                        EvidenceID = model.EvidenceId
                    });

                if (associatedAccountList.Any())
                {
                    _customerEvidenceService.UpdateBetLimitOfUnflaggedCustomer(
                        new UpdateBetLimitByEvidenceRemovingCriteria
                        {
                            CustIDList = string.Join(",", associatedAccountList)
                        });
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult RetractAffectedEvidence(RemoveEvidenceModel model)
        {
            model.UserId = CurrentSession.UserId;

            var result = _accountService.RetractAffectedEvidence(model);

            if (result == 0)
            {
                var associatedAccountList = _customerEvidenceService.GetCleanAccounts(
                      new GetCleanAccountByCTSCustIdsCriteria
                      {
                          CTSCustIdList = model.CTSCustId.ToString()
                      });

                if (associatedAccountList.Any())
                {
                    _customerEvidenceService.UpdateBetLimitOfUnflaggedCustomer(
                        new UpdateBetLimitByEvidenceRemovingCriteria
                        {
                            CustIDList = string.Join(",", associatedAccountList)
                        });
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
                _customerEvidenceService.UpdateBetLimitOfFlaggedCustomer(
                          new UpdateBetLimitByEvidenceAddingCriteria
                          {
                              CustIDList = model.CTSCustId.ToString()
                          });
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
        
        #endregion

        private void GetCustomerGeneralStatus(long custId, ref AccountProfileViewModel profile)
        {
            var model = new CustomerGeneralStatusViewModel();

            var result = _accountService.GetCustomerGeneralStatus(custId.ToString()).FirstOrDefault();
            var accountStatusName = string.Empty;

            if (result != null)
            {
                accountStatusName = model.GetAccountStatusName(model.GetAccountStatus(result));
            }
            else
            {
                accountStatusName = model.GetAccountStatusName(AccountStatus.NA);

                result = new CustomerGeneralStatusModel();
            }

            profile.GeneralStatusInfo = result;
            profile.StatusLabel = accountStatusName;
        }

        private void GetCustomerProfilePoliciesInfo(long custId, ref AccountProfileViewModel profile)
        {
            profile.PoliciesInfo = _accountService.GetCustomerProfilePoliciesInfo(custId);
          
            var customerClass = _accountService.GetCustomerClassByCustId(custId.ToString());

            if(customerClass != null && customerClass.Any())
            {
                profile.PoliciesInfo.CustomerClass = customerClass.First().CustomerClass;
            }
        }

        private void GetCustomerRobotUserStatus(long custId, ref AccountProfileViewModel profile)
        {
            profile.RobotUserStatus = _accountService.GetCustomerRobotUserStatus(custId);
        }

        private void GetCustomerFlagInfo(long ctsCustId, ref AccountProfileViewModel profile)
        {
            profile.FlagedAffectedEvidence = _accountService.GetCustomerFlagInfo(ctsCustId);
        }

        private void GetCustomerEsportAutoSuspendedStatus(long custId, ref AccountProfileViewModel profile)
        {
            profile.EsportAutoSuspendStatus = _accountService.GetCustomerEsportAutoSuspendedStatus(custId);
        }

        #region Transaciton History
        public ActionResult TransactionHistory()
        {
            return PartialView("TransactionHistory/TransactionHistory");
        }

        public ActionResult GetTransactionHistory(DataTableAjaxPostModel model)
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
                BrowserName = x.BrowserName,
                DeviceId = x.DeviceId,
                Flagged = x.Flagged,
                Ip = isViewIPPermission ? x.Ip : string.Empty,
                IpId = 0,
                OSName = x.OSName,
                TransTime = x.TransTime.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                Url = x.Url
            });

            return Json(new
            {
                draw = model.draw,
                recordsTotal = totalItem,
                recordsFiltered = totalItem,
                data = result
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Customer Category

        public ActionResult CustomerCategory()
        {
            return PartialView("CustomerCategory/CustomerCategory");
        }

        public ActionResult GetCustomerClassification(long custId)
        {
            var isPinCategory = false;
            var isVVip = false;
            var categoryList = new List<CustomerCategoryViewModel>();
            var classificationHistoryList = new List<CustomerClassificationHistoryViewModel>();

            var classification = _accountService.GetCustomerClassification(custId, out isPinCategory);

            if (classification != null && classification.Any())
            {
                categoryList = classification
                    .Select(x => new CustomerCategoryViewModel
                    {
                        CategoryId = x.CategoryId,
                        CategoryType = x.CategoryType,
                        CategoryName = x.CategoryName != null ? x.CategoryName : string.Empty,
                        IsProblemAccount = x.CategoryType == 2
                    }).ToList();

                isVVip = categoryList.Where(x => x.CategoryName.Equals("VVIP")).Any();
            }

            var classificationHistory = _accountService.GetCustomerClassificationHistory(custId);

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
                    CreatedBy = x.CreatedBy
                }).ToList();
            }

            return Json(new
            {
                isPinCategory,
                isVVip,
                categoryList,
                data = classificationHistoryList,
            }, JsonRequestBehavior.AllowGet);
        }

        private string GetReason(CustomerClassificationHistoryModel cc)
        {
            var reason = $"<div><span> { cc.SourceTypeName } </span><br>";            

            var margin = Math.Round(cc.Margin, 3);
            var winlost = Math.Round(cc.Winloss, 3);
            var turnOver = Math.Round(cc.TurnOver, 3);
            var betCount = cc.BetCount;
            var activeDays = cc.ActiveDays;
            var last7DayMargin = Math.Round(cc.LastXDaysMargin, 3);
            var winlost20Days = Math.Round(cc.ProbationPeriodWinloss, 3);

            switch (cc.CategoryID)
            {
                case (int)ClassificationSourceType.NormalAccount:                  
                    reason += $"Margin: {margin} - Bet Count: {betCount}";
                    break;
                case (int)ClassificationSourceType.VIP:                    
                    reason += $"Margin: {margin} - Turn Over: {turnOver} - Active Days: {activeDays} - Bet Count: {betCount}";
                    break;
                case (int)ClassificationSourceType.Probation:
                    if (cc.ItemID == 5)
                        reason += $"Margin: {margin} - Win Lost: {winlost} - Bet Count: {betCount} - Last 7 day's Margin: {last7DayMargin} - WinLost(20 days): {winlost20Days}";
                    else
                        reason += $"Margin: {margin}";
                    break;
                case (int)ClassificationSourceType.SmartPunter:
                case (int)ClassificationSourceType.HighRiskPunter:
                    if (cc.ItemID == 4)
                        reason += $"Margin: {margin} - Win Lost: {winlost} - Bet Count: {betCount} - Last 7 day's Margin: {last7DayMargin} - WinLost(20 days): {winlost20Days}";
                    else
                        reason += $"Margin: {margin} - Win Lost: {winlost} - Bet Count: {betCount}";
                    break;
                default:
                    break;
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
                CustomerCategoryList = customerCategoryList.Where(x=>x.CategoryName.Equals("VVIP") || x.ParentId == 0).ToList()
            };

            return PartialView("CustomerCategory/MarkCatergory", model);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult RemoveCatergory(long ctsCustId)
        {
            var customerCategoryList = _accountService.GetCustomerCategoryByCTSCustId(ctsCustId).ToList();

            var model = new MarkCategoryViewModel
            {
                CustomerCategoryList = new List<Model.DTO.CustomerCategory> { new Model.DTO.CustomerCategory { CategoryId = 213, CategoryName = "VVIP", ParentId = 200 } }
            };

            return PartialView("CustomerCategory/RemoveCategory", model);
        }               

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
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
                    var custIdInt = 0;
                    int.TryParse(addCategoryModel.CustId.ToString(), out custIdInt);

                    if (custIdInt != 0)
                    {
                        latestClassifications = latestClassifications.Where(c => c.CustId != 0);

                        if (latestClassifications.Any())
                        {
                            // Update CC in table custinfo to MainDB
                            var isSuccessToMain = _pushDataToExternalService.PushCustomerClassificationToMain(latestClassifications, 2);

                            if (isSuccessToMain)
                            {
                                var updateApplyCCs = latestClassifications.Select(x => new { x.CustId, x.ScannedTime });

                                var jsonUpdateApplyCCs = JsonConvert.SerializeObject(updateApplyCCs);

                                // Update IsAppliedCC = 1 in CTS
                                _customerClassifyService.UpdateAppliedCustomerClassification(jsonUpdateApplyCCs);
                            }
                        }
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
                        var custIds = new List<int>() { custIdInt };

                        // Rescan Problem Account & Normal Account
                        _customerClassifyService.RescanCustomerClassify(custIds, true, true);
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
        [HttpPost]
        public ActionResult PinCategoryCustomerClassification(int custId, long ctsCustId, int subscriberId, bool isPinCategory)
        {
            var result = _accountService.PinCategoryCustomerClassification(custId, ctsCustId, subscriberId, isPinCategory, CurrentSession.UserId);

            return Json(new
            {
                errorCode = result.ErrorCode,
                errorMessage = result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

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
        #endregion
        
    }
}