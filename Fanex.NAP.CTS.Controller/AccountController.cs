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

    [NapAuth(FunctionKey = FSFunctionKeys.AccountDetail, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IGraphService _graphService;

        private readonly ISubscriberAssignmentService _subscriberAssignmentService;

        public AccountController()
        {
            _accountService = new AccountService();
            _graphService = new GraphService();
            _subscriberAssignmentService = new SubscriberAssignmentService();
        }

        public override string BaseControllerName { get => Consts.MenuItemSearch; }

        public ActionResult Index(long Id)
        {
            if (Id > 0)
            {
                var customerInfoFromCTS = _accountService.GetCustomerInfoByCTSCustId(Id);

                if (customerInfoFromCTS != null)
                {
                    var customerGeneralStatusViewModel = new CustomerGeneralStatusViewModel()
                    {
                        CTSCustId = Id
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
            LogService.Info("GetAccountProfile - Before: " + DateTime.Now.ToString());
            var profile = new AccountProfileViewModel();

            GetCustomerGeneralStatus(custId, ref profile);
            GetCustomerProfileFraudInfo(custId, ref profile);
            GetCustomerRobotUserStatus(custId, ref profile);
            GetCustomerFlagInfo(ctsCustId, subscriberId, ref profile);
            GetCustomerEsportAutoSuspendedStatus(custId, ref profile);

            LogService.Info("GetAccountProfile - After: " + DateTime.Now.ToString());
            return Json(profile, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCustomerTotalAssociationByCTSCustId(long ctsCustId)
        {
            LogService.Info("GetCustomerTotalAssociationByCTSCustId - Before: " + DateTime.Now.ToString());
            var result = _accountService.GetCustomerTotalAssociationByCTSCustId(ctsCustId, ConfigSettings.GetStatisticAssociation);
            LogService.Info("GetCustomerTotalAssociationByCTSCustId - After: " + DateTime.Now.ToString());

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
        [HttpPost]
        public ActionResult AssociatedByAccount(string username)
        {
            var getCustomerStaticStatus = _accountService.GetCustomerStaticStatus(1, true);
            var getCustomerStaticAssType = _accountService.GetCustomerStaticStatus(2, true);

            getCustomerStaticAssType.Insert(0, new CustomerStaticStatusModel { ItemId = 0, ItemNameDisplay = "All" });

            var associatedByAccountViewModel = new AssociatedByAccountViewModel
            {
                AccountStatusList = getCustomerStaticStatus,
                AssociationTypeList = getCustomerStaticAssType,
                Username = username
            };

            return PartialView("Associated/AccountByAccount", associatedByAccountViewModel);
        }

        [HttpPost]
        public ActionResult GetAssociatedByAccount(long ctsCustId, string statusIds, int associationType, bool isDirect)
        {
            LogService.Info("GetAssociatedByAccount - Before: " + DateTime.Now.ToString());
            var result = new List<CustomerAssociationByAccountDetailViewModel>();

            if (string.IsNullOrWhiteSpace(statusIds) || ctsCustId == 0)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var customerAssociation = isDirect ?
                _accountService.GetCustomerAssociationByGraph(ConfigSettings.GetDirectAssociation, ctsCustId) :
                _accountService.GetCustomerAssociationByGraph(ConfigSettings.GetInDirectAssociation, ctsCustId);

            if (customerAssociation != null && customerAssociation.Any())
            {
                var custIds = string.Join(",", customerAssociation.Select(x => x.CustId));

               // LogService.Info("custIds:" + custIds);

                var getStatusByCustIds = _accountService.GetCustomerGeneralStatus(custIds);

                var getAllSubscribers = _subscriberAssignmentService.GetSubscribers(0);

                customerAssociation = _accountService.FilterCustomerAssociationByStatus(statusIds, customerAssociation, getStatusByCustIds);

                //LogService.Info("customerAssociation1:" + JsonConvert.SerializeObject(customerAssociation));

                customerAssociation = _accountService.FilterCustomerAssociationByAssociationType(associationType, customerAssociation);

                //LogService.Info("customerAssociation2:" + JsonConvert.SerializeObject(customerAssociation));

                result = customerAssociation.Select(x => new CustomerAssociationByAccountDetailViewModel
                {
                    CustId = x.CustId.HasValue ? x.CustId.Value : 0,
                    Username = !string.IsNullOrWhiteSpace(x.Username) ? x.Username : x.Username2,
                    LoginId = !string.IsNullOrWhiteSpace(x.Username2) ? x.Username2 : x.Username,
                    AssociationDate = x.AssociationDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                    SharedDevices = isDirect ? x.SharedDevices : -1,
                    SharedIps = isDirect ? x.SharedIPs : -1,
                    AssociationType = x.AssociationType,
                    AssociationTypeName = x.AssociationType == 0 ? string.Format("{0}, {1}", x.GetAssociationTypeName(Model.AssociationType.Device), x.GetAssociationTypeName(Model.AssociationType.IP)) : x.AssociationTypeName,
                    Status = x.StatusName,
                    StatusId = x.StatusId,
                    SubscriberId = x.SubscriberId,
                    Subscriber = getAllSubscribers.Any(aa => aa.SubscriberId == x.SubscriberId) ? getAllSubscribers.FirstOrDefault(a => a.SubscriberId == x.SubscriberId).SubscriberName : "-",
                    CTSCustId = x.CTSCustID
                }).ToList();

                if (result != null && result.Any())
                {
                    var tasks = new List<Task>();


                    var getFlaggedTags = new List<TagModel>();
                    var getAffectedTags = new List<TagModel>();
                    var getCustIdsAffectedTag = new List<GraphDirectAssociationModel>();

                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            var custIdsFlaggedTag = result.Select(a => new { a.SubscriberId, a.CTSCustId });
                            getFlaggedTags = _accountService.GetDirectAssociatedAccountTagFlagged(JsonConvert.SerializeObject(custIdsFlaggedTag));
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
                            var custIdsAffectedTag = result.Select(a => a.CTSCustId).ToList();
                            getCustIdsAffectedTag = _accountService.GetDirectAssocationByGraph(ConfigSettings.GetDirectAssociationMultiCust, custIdsAffectedTag);

                            if (getCustIdsAffectedTag != null && getCustIdsAffectedTag.Any())
                            {
                                var jsonParam = JsonConvert.SerializeObject(getCustIdsAffectedTag);

                                getAffectedTags = _accountService.GetDirectAssociatedAccountTagAffected(jsonParam);
                            }
                            else
                            {
                                LogService.Error("GetDirectAssociationMultiCust is null");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogService.Info(ex.Message);

                            throw;
                        }
                    }));

                    Task.WhenAll(tasks.ToArray()).Wait();

                    result = result.Select(x =>
                    {
                        var affectedTags = getAffectedTags.Where(z => z.FromCTSCustId == x.CTSCustId);

                        x.AffectedTag = (affectedTags != null && affectedTags.Any()) ? affectedTags.FirstOrDefault().AffectedTag : 0;

                        var flaggedTag = getFlaggedTags.Where(z => z.CTSCustId == x.CTSCustId);

                        x.FlaggedTag = (flaggedTag != null && flaggedTag.Any()) ? flaggedTag.FirstOrDefault().FlaggedTag : 0;

                        return x;
                    })
                    .OrderBy(a => a.StatusId)
                    .ToList();
                }
            }

            LogService.Info("GetAssociatedByAccount - Done: " + DateTime.Now.ToString());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDirectAssociationGraph(Int64 ctsCustid, string username)
        {
            var graphCustomer = new GraphCustomerItemViewModel();

            var customerAssociationList = _accountService.GetCustomerAssociationByGraph(ConfigSettings.GetDirectAssociation, ctsCustid);
            if (customerAssociationList != null && customerAssociationList.Any())
            {
                var now = DateTime.Now.Ticks.ToString();
                var graphCustomerList = customerAssociationList
                    .Select(x => new GraphCustomerItemViewModel
                    {
                        CTSCustId = x.CTSCustID,
                        ItemCode = x.CTSCustID.ToString() + "_" + now,
                        DisplayName = !string.IsNullOrWhiteSpace(x.Username) ? x.Username : x.Username2
                    }).ToList();

                graphCustomer.children = graphCustomerList.ToArray();
            }

            graphCustomer.CTSCustId = ctsCustid;
            graphCustomer.DisplayName = username;

            return Json(graphCustomer, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetShortestAssociationGraph(int custIdFrom, int custIdTo)
        {
            var shortestPathOfAccount = _accountService.GetShortestPathAssociationByGraph(ConfigSettings.GetShortestAssociation, custIdFrom, custIdTo);
            var graphCustomer = _graphService.GetGraphCustomerItemByShortestPathOfAccount(shortestPathOfAccount);

            return Json(graphCustomer, JsonRequestBehavior.AllowGet);
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
        public ActionResult PlaceEvidence(UpdatePlaceEvidenceModel model)
        {
            model.UserId = model.UserId == 0 ? CurrentSession.UserId : model.UserId;
            var result = _accountService.PlaceEvidence(model);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveFlaggedEvidence(RemoveEvidenceModel model)
        {
            model.UserId = CurrentSession.UserId;

            var result = _accountService.RemoveFlaggedEvidence(model);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RetractAffectedEvidence(RemoveEvidenceModel model)
        {
            model.UserId = CurrentSession.UserId;

            var result = _accountService.RetractAffectedEvidence(model);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
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

            profile.SuspiciousInfo = result.SuspiciouseCustomer;
            profile.FraudCases = fraudCaseName;
        }

        private void GetCustomerRobotUserStatus(long custId, ref AccountProfileViewModel profile)
        {
            profile.RobotUserStatus = _accountService.GetCustomerRobotUserStatus(custId);
        }

        private void GetCustomerFlagInfo(long ctsCustId, int subscriberId, ref AccountProfileViewModel profile)
        {
            profile.FlagInfo = _accountService.GetCustomerFlagInfo(ctsCustId, subscriberId);
        }

        private void GetCustomerEsportAutoSuspendedStatus(long custId, ref AccountProfileViewModel profile)
        {
            profile.EsportAutoSuspendStatus = _accountService.GetCustomerEsportAutoSuspendedStatus(custId);
        }

    }
}