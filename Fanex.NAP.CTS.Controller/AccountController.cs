namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.ViewModel.Account;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using Fanex.NAP.CTS.ViewModel.Account.ChainToEvidence;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Transaction;
    using Fanex.NAP.CTS.ViewModel.Account.TransactionHistory;
    using Fanex.NAP.CTS.Model.Association;
    using Fanex.NAP.CTS.ViewModel.Account.CustomerCategory;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.CTS.ViewModel.Account.BettingStatistic;

    [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IGraphService _graphService;

        private readonly ITransactionService _transService;

        private readonly ISubscriberAssignmentService _subscriberAssignmentService;

        public override string BaseControllerName { get => string.Empty; }

        public AccountController()
        {
            _accountService = new AccountService();
            _graphService = new GraphService();
            _subscriberAssignmentService = new SubscriberAssignmentService();
            _transService = new TransactionService();
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
                    GetCustomerProfileFraudInfo(custId, ref profile);
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
                    GetCustomerFlagInfo(ctsCustId, subscriberId, ref profile);
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
        public ActionResult GetCustomerFlagInfo(long ctsCustId, int subscriberId)
        {
            var result = _accountService.GetCustomerFlagInfo(ctsCustId, subscriberId);

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

        #region Associated By Account

        public ActionResult AssociatedByAccount(string username)
        {
            var getAssociationStatus = _accountService.GetCustomerStaticStatus(3, true);

            getAssociationStatus.Insert(0, new CustomerStaticStatusModel { ItemId = 0, ItemNameDisplay = "All" });

            var getCustomerStaticAssType = _accountService.GetCustomerStaticStatus(2, true);

            getCustomerStaticAssType.Insert(0, new CustomerStaticStatusModel { ItemId = 0, ItemNameDisplay = "All" });

            var associatedByAccountViewModel = new AssociatedByAccountViewModel
            {
                AssociationStatus = getAssociationStatus,
                AssociationTypeList = getCustomerStaticAssType,
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
                ? _accountService.GetCustomerAssociationByGraph(directAssociationUrl, model.CTSCustId, model.AssociationStatus, skip, take)
                : new GraphAssociationPaging();

            if (graphAssociationPage != null && graphAssociationPage.AssociationList != null && graphAssociationPage.AssociationList.Any())
            {
                totalItem = graphAssociationPage.TotalItems;
                var getAllSubscribers = _subscriberAssignmentService.GetSubscribers(0);

                var customerAssociation = graphAssociationPage.AssociationList;

                var custIds = string.Join(",", customerAssociation.Select(x => x.CustId));

                var getStatusByCustIds = _accountService.GetCustomerGeneralStatus(custIds);

                customerAssociation = _accountService.FilterCustomerAssociationByStatus(string.Empty, customerAssociation, getStatusByCustIds);

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
                        StatusId = x.StatusId,
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
                }).OrderBy(a => a.StatusId)
                .ThenBy(a => !a.IsLinked)
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
        public ActionResult GetDirectAssociationGraph(Int64 ctsCustid, string displayName, int associationStatus, int start, int length)
        {
            var graphCustomer = new GraphCustomerItemViewModel();

            var graphAssociationPaging = _accountService.GetCustomerAssociationByGraph(ConfigSettings.GetDirectAssociation, ctsCustid, associationStatus, start, length);
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

            return Json(new
            {
                errorCode = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult RemoveException(UpdateAccountExceptionModel model)
        {
            model.UserId = CurrentSession.UserId;

            var result = _accountService.RemoveAccountException(model);

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
            var result = _accountService.PlaceEvidence(model);

            if (result.ErrorCode == 0)
            {
                Task.Run(() =>
                {

                    try
                    {
                        _accountService.FPSAddNewEvidence(ConfigSettings.AddNewEvidence, model);
                    }
                    catch (Exception ex)
                    {
                        LogService.Error(ex.Message);
                    }
                });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult RemoveFlaggedEvidence(RemoveEvidenceModel model)
        {
            model.UserId = CurrentSession.UserId;

            var result = _accountService.RemoveFlaggedEvidence(model);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult RetractAffectedEvidence(RemoveEvidenceModel model)
        {
            model.UserId = CurrentSession.UserId;

            var result = _accountService.RetractAffectedEvidence(model);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult RestoreAffectedEvidence(RemoveEvidenceModel model)
        {
            model.UserId = CurrentSession.UserId;

            var result = _accountService.RestoreAffectedEvidence(model);

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

        private void GetCustomerProfileFraudInfo(long custId, ref AccountProfileViewModel profile)
        {
            var result = _accountService.GetCustomerProfileFraudInfo(custId);

            var fraudCaseName = string.Empty;

            if (result.FraudCaseList != null && result.FraudCaseList.Any())
            {
                fraudCaseName = string.Join(", ", result.FraudCaseList.Select(x => x.FraudCaseName));
            }

            profile.FraudCases = fraudCaseName;
        }

        private void GetCustomerRobotUserStatus(long custId, ref AccountProfileViewModel profile)
        {
            profile.RobotUserStatus = _accountService.GetCustomerRobotUserStatus(custId);
        }

        private void GetCustomerFlagInfo(long ctsCustId, int subscriberId, ref AccountProfileViewModel profile)
        {
            profile.FlagedAffectedEvidence = _accountService.GetCustomerFlagInfo(ctsCustId, subscriberId);
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
                Ip = x.Ip,
                IpId = x.IpId,
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

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult MarkCatergory(long ctsCustId)
        {
            var model = new MarkCategoryViewModel
            {
                SportGroupList = _accountService.GetAllSportGroup().ToList(),
                CustomerCategoryList = _accountService.GetCustomerCategoryByCTSCustId(ctsCustId).ToList()
            };

            return PartialView("CustomerCategory/MarkCatergory", model);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult EditMarkCatergory(long ctsCustId, int subscriberId, string categoryName, int categoryId, string sportGroupIdList, string comment)
        {
            var model = new MarkCategoryViewModel
            {
                SportGroupList = _accountService.GetAllSportGroup().ToList(),
                CategoryId = categoryId,
                CategoryName = categoryName,
                SportGroupIdList = sportGroupIdList,
                CTSCustId = ctsCustId,
                SubscriberId = subscriberId,
                Comment = comment
            };

            return PartialView("CustomerCategory/EditMarkCatergory", model);
        }

        public ActionResult GetCustomerClassification(long custId)
        {
            var data = _accountService.GetCustomerClassification(custId);

            if (data != null && data.Any())
            {
                var result = data.Select(x => new CustomerCategoryViewModel
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    LastModifiedBy = x.LastModifiedBy,
                    LastModifiedDate = x.LastModifiedDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                    Remark = x.Remark,
                    SportGroupIdList = x.SportGroupIdList,
                    SportGroupNameList = x.SportGroupNameList,
                    ParentCategoryId = x.ParentCategoryId,
                    ParentCategoryName = x.ParentCategoryName,
                    IsEnabledDelete = x.IsEnabledDelete
                });

                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { data = new List<CustomerCategoryViewModel>() }, JsonRequestBehavior.AllowGet);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult AddCustomerClassification(long custId, long ctsCustId, int subscriberId, int categoryId, string sportGroupIds, string remark)
        {
            if (ctsCustId <= 0 || subscriberId <= 0 || categoryId <= 0)
            {
                return Json(new
                {
                    errorCode = -100,
                    errorMessage = "Invalid parameter"
                }, JsonRequestBehavior.AllowGet);
            }

            var result = _accountService.AddCustomerClassification(custId, ctsCustId, subscriberId, categoryId, sportGroupIds, remark, CurrentSession.UserId);

            return Json(new
            {
                errorCode = result.ErrorCode,
                errorMessage = result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult UpdateCustomerClassification(long custId, long ctsCustId, int subscriberId, int categoryId, string sportGroupIds, string remark)
        {
            if (ctsCustId <= 0 || subscriberId <= 0 || categoryId <= 0)
            {
                return Json(new
                {
                    errorCode = -100,
                    errorMessage = "Invalid parameter"
                }, JsonRequestBehavior.AllowGet);
            }

            var result = _accountService.UpdateCustomerClassification(custId, ctsCustId, subscriberId, categoryId, sportGroupIds, remark, CurrentSession.UserId);

            return Json(new
            {
                errorCode = result.ErrorCode,
                errorMessage = result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult RemoveCustomerClassification(long custId, long ctsCustId, int categoryId)
        {
            if (custId <= 0 || categoryId <= 0)
            {
                return Json(new
                {
                    errorCode = -100,
                    errorMessage = "Invalid parameter"
                }, JsonRequestBehavior.AllowGet);
            }

            var result = _accountService.RemoveCustomerClassification(custId, ctsCustId, categoryId, CurrentSession.UserId);

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
            var result = _accountService.GetPerformanceBySport(custId, getTop)?.OrderBy(c=>c.Ranking);
            var totalSportBettingStatistic = new SportBettingStatisticViewModel();
            var bestSportBettingStatistic = new SportBettingStatisticViewModel();
            var pieChartSportStatisticSessions = new List<SportStatisticPieChartViewModel>();

            if(result != null && result.Any())
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
            var result = _accountService.GetPerformanceDaily(custId).OrderBy(c=>c.RptDate);
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
                                              .Select(x => new { BettypeId = x.Key,TotalTurnOver = x.Sum(w => w.Turnover) }))
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
                for (int i= 0; i < loop; i++)
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