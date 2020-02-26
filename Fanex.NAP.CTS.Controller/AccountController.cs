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
            LogService.Info("Terry is testing");

            if (Id > 0)
            {
                var tasks = new List<Task>();

                var customerInfoFromCTS = _accountService.GetCustomerInfoByCTSCustId(Id);

                if (customerInfoFromCTS != null)
                {
                    var customerGeneralStatusViewModel = new CustomerGeneralStatusViewModel()
                    {
                        CTSCustId = Id
                    };

                    var customerStatus = new CustomerGeneralStatusModel();
                    var customerTotalAssociation = new CustomerAssociationByCTSCustIdModel();
                    var customerProfileFraudInfo = new CustomerProfileFraudInfoModel();
                    var customerLastTicketDate = new CustomerLastTicketDateModel();
                    var customerEsportAutoSuspendedStatus = new CustomerEsportAutoSuspendStatusModel();
                    var customerRobotUserStatusModel = new CustomerRobotUserStatusModel();
                    var customerFlagInfoModel = new CustomerFlagInfoModel();

                    var customerEvidences = new List<CustomerEvidenceByCTSCustIdModel>();

                    tasks.Add(Task.Run(() => { customerStatus = _accountService.GetCustomerGeneralStatus(customerInfoFromCTS.CustId.ToString()).FirstOrDefault(); }));
                    tasks.Add(Task.Run(() => { customerTotalAssociation = _accountService.GetCustomerTotalAssociationByCTSCustId(Id); }));
                    tasks.Add(Task.Run(() => { customerEvidences = _accountService.GetCustomerEvidenceByCTSCustId(Id); }));
                    tasks.Add(Task.Run(() => { customerProfileFraudInfo = _accountService.GetCustomerProfileFraudInfo(customerInfoFromCTS.CustId); }));
                    tasks.Add(Task.Run(() => { customerLastTicketDate = _accountService.GetCustomerLastTicketDate(customerInfoFromCTS.CustId); }));
                    tasks.Add(Task.Run(() => { customerEsportAutoSuspendedStatus = _accountService.GetCustomerEsportAutoSuspendedStatus(customerInfoFromCTS.CustId); }));
                    tasks.Add(Task.Run(() => { customerRobotUserStatusModel = _accountService.GetCustomerRobotUserStatus(customerInfoFromCTS.CustId); }));
                    tasks.Add(Task.Run(() => { customerFlagInfoModel = _accountService.GetCustomerFlagInfo(Id, customerInfoFromCTS.SubscriberId); }));

                    Task.WhenAll(tasks.ToArray()).Wait();

                    customerGeneralStatusViewModel
                        .BindingEvidenceListFromCTS(customerEvidences)
                        .BindingGeneralDataFromCTS(customerInfoFromCTS,
                                                    customerFlagInfoModel,
                                                    customerTotalAssociation,
                                                    customerEvidences)
                        .BindingGeneralDataFromMainDB(customerStatus,
                                                        customerLastTicketDate,
                                                        customerEsportAutoSuspendedStatus,
                                                        customerRobotUserStatusModel)
                        .BindingTotalAssociationFromCTS(customerTotalAssociation)
                        .BindingGeneralDataFromFS(customerProfileFraudInfo);

                    return View(customerGeneralStatusViewModel);
                }
            }

            return RedirectToAction("Index", "Home");
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
        public ActionResult GetAssociatedByAccount(int CustId, long ctsCustId, string statusIds, int associationType, bool isDirect)
        {
            var result = new List<CustomerAssociationByAccountDetailViewModel>();

            if (string.IsNullOrWhiteSpace(statusIds) || CustId == 0 || ctsCustId == 0)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var customerAssociation = isDirect ?
                _accountService.GetCustomerAssociationByGraph(ConfigSettings.GetDirectAssociation, ctsCustId) :
                _accountService.GetCustomerAssociationByGraph(ConfigSettings.GetInDirectAssociation, ctsCustId);

            if (customerAssociation != null && customerAssociation.Any())
            {
                var getStatusByCustIds = _accountService.GetCustomerGeneralStatus(string.Join(",", customerAssociation.Select(x => x.CustId)));

                var getAllSubscribers = _subscriberAssignmentService.GetSubscribers(0);

                customerAssociation = _accountService.FilterCustomerAssociationByStatus(statusIds, customerAssociation, getStatusByCustIds);

                customerAssociation = _accountService.FilterCustomerAssociationByAssociationType(associationType, customerAssociation);

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
                    Subscriber = getAllSubscribers.FirstOrDefault(a => a.SubscriberId == x.SubscriberId).SubscriberName,
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
                        var custIdsFlaggedTag = result.Select(a => new { a.SubscriberId, a.CTSCustId });
                        getFlaggedTags = _accountService.GetDirectAssociatedAccountTagFlagged(JsonConvert.SerializeObject(custIdsFlaggedTag));
                    }));

                    tasks.Add(Task.Run(() =>
                    {
                        var custIdsAffectedTag = result.Select(a => a.CTSCustId).ToList();
                        getCustIdsAffectedTag = _accountService.GetDirectAssocationByGraph(ConfigSettings.GetDirectAssociationMultiCust, custIdsAffectedTag);

                        getAffectedTags = _accountService.GetDirectAssociatedAccountTagAffected(JsonConvert.SerializeObject(getCustIdsAffectedTag));
                    }));

                    Task.WhenAll(tasks.ToArray()).Wait();

                    result = result.Select(x =>
                    {
                        x.AffectedTag = getAffectedTags.FirstOrDefault(z => z.FromCTSCustId == x.CTSCustId).AffectedTag;
                        x.FlaggedTag = getFlaggedTags.FirstOrDefault(z => z.CTSCustId == x.CTSCustId).FlaggedTag;

                        return x;
                    })
                    .OrderBy(a => a.StatusId)
                    .ToList();
                }
            }

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
                    .Select(c => new GraphCustomerItemViewModel
                    {
                        CustId = (c.CustId.HasValue ? c.CustId.Value : 0),
                        ItemCode = (c.CustId.HasValue ? c.CustId.Value.ToString() : c.Username2) + "_" + now,
                        Username = c.Username,
                        DisplayName = !string.IsNullOrWhiteSpace(c.Username) ? c.Username : c.Username2,
                        AssociationType = c.AssociationType,
                        AssociationTypeName = c.AssociationTypeName
                    }).ToList();
                graphCustomer.children = graphCustomerList.ToArray();
            }
            graphCustomer.CustId = ctsCustid;
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
                    LastLoginTime = x.LastLoginTime.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
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
            model.UserId = CurrentSession.UserId;
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
    }
}