namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.ViewModel.Account;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.SearchAccount, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IGraphService _graphService;
        //private readonly IEvidenceService _evidenceService;
        //private readonly ITransactionService _transactionService;

        public AccountController()
        {
            _accountService = new AccountService();
            _graphService = new GraphService();
            //_evidenceService = new EvidenceService();
            //_transactionService = new TransactionService();
        }

        public override string BaseControllerName { get => Consts.MenuItemSearch; }

        public ActionResult Index(int Id)
        {
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
                        .BindingGeneralDataFromCTS(customerInfoFromCTS, customerFlagInfoModel, customerTotalAssociation)
                        .BindingGeneralDataFromMainDB(customerStatus,
                                                        customerLastTicketDate,
                                                        customerEsportAutoSuspendedStatus,
                                                        customerRobotUserStatusModel)
                        .BindingTotalAssociationFromCTS(customerTotalAssociation)
                        .BindingEvidenceListFromCTS(customerEvidences)
                        .BindingGeneralDataFromFS(customerProfileFraudInfo);

                    return View(customerGeneralStatusViewModel);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        //public ActionResult DetailAccount(int id, int custId)
        //{
        //    var permissionIPLocation = new UserService().GetPermission(CurrentSession.UserId, SiteId, FSFunctionKeys.IPLocation);

        //    ViewBag.IsPermissionIPLocation = permissionIPLocation > 0;
        //    ViewBag.AccountId = id;
        //    ViewBag.CustId = custId;

        //    return View("DetailAccount");
        //}

        //#region General Information
        //[HttpPost]
        //public ActionResult GetGeneralInformation(GeneralInformationFilter filter)
        //{
        //    filter.IsMasterSubscriberAssignment = PermissionService.IsMasterSubscriberAssignment();
        //    filter.UserId = CurrentSession.UserId;

        //    var data = _accountService.GetAccountDetailGeneralInformation(filter);

        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetAllEvidences()
        //{
        //    var data = _evidenceService.GetAllEvidences();

        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult PlaceEvidenceByAccount(GeneralInformationPlaceEvidenceByAccount evidenceByAccount)
        //{
        //    var data = _evidenceService.PlaceEvidenceByAccount(evidenceByAccount);

        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult GetInformationEvidenceByAccount(InformationEvidenceByAccountFilter filter)
        //{
        //    var data = _evidenceService.GetInformationEvidenceByAccount(filter);

        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult UpdateEvidenceByAccountOfComment(InformationEvidenceByAccountUpdateOfComment evidenceOfComment)
        //{
        //    var data = _evidenceService.UpdateEvidenceByAccountOfComment(evidenceOfComment);

        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult RetractEvidenceByAccount(InformationEvidenceByAccountRetractEvidence evidenceRetract)
        //{
        //    var data = _evidenceService.RetractEvidenceByAccount(evidenceRetract);

        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //#endregion

        //#region Chain To Evidence
        //[HttpPost]
        //public ActionResult GetChainsToEvidence(ChainsToEvidenceFilter filter)
        //{
        //    filter.UserId = CurrentSession.UserId;

        //    filter.IsMasterSubscriberAssignment = PermissionService.IsMasterSubscriberAssignment();

        //    var data = _evidenceService.GetChainsToEvidence(filter);

        //    return PartialView("DetailInfoAccount/ChainsToEvidence", data);
        //}

        //[HttpPost]
        //public ActionResult AddExceptionEvidence(ExceptionEvidenceAdd exceptionEvidenceAdd)
        //{
        //    exceptionEvidenceAdd.UserId = CurrentSession.UserId;

        //    var data = _evidenceService.AddExceptionEvidence(exceptionEvidenceAdd);

        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //#endregion

        //#region Retract And Exception
        //[HttpPost]
        //public ActionResult GetRetractAndException(RetractedAndExceptionEvidenceFilter filter)
        //{
        //    filter.UserId = CurrentSession.UserId;

        //    filter.IsMasterSubscriberAssignment = PermissionService.IsMasterSubscriberAssignment();

        //    var data = _evidenceService.GetRetractedAndExceptionEvidence(filter);

        //    return PartialView("DetailInfoAccount/RetractAndException", data);
        //}

        //#endregion

        //#region Associated Account By Account
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
        public ActionResult GetAssociationByAccount(int CustId, int ctsCustId, string statusIds, int associationType, bool isDirect)
        {
            var result = new List<CustomerAssociationByAccountDetailModel>();

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

                customerAssociation = _accountService.FilterCustomerAssociationByStatus(statusIds, customerAssociation, getStatusByCustIds);

                customerAssociation = _accountService.FilterCustomerAssociationByAssociationType(associationType, customerAssociation);

                result = customerAssociation.Select(x => new CustomerAssociationByAccountDetailModel
                {
                    CustId = x.CustId.HasValue ? x.CustId.Value : 0,
                    Username = !string.IsNullOrWhiteSpace(x.Username) ? x.Username : x.Username2,
                    LoginId = !string.IsNullOrWhiteSpace(x.Username2) ? x.Username2 : x.Username,
                    AssociationDate = x.AssociationDate,
                    SharedDevices = isDirect ? x.SharedDevices : -1,
                    SharedIps = isDirect ? x.SharedIPs : -1,
                    AssociationType = x.AssociationType,
                    AssociationTypeName = x.AssociationType == 0 ? string.Format("{0}, {1}", x.GetAssociationTypeName(Model.AssociationType.Device), x.GetAssociationTypeName(Model.AssociationType.IP)) : x.AssociationTypeName,
                    Status = x.StatusName,
                    StatusId = x.StatusId
                }).ToList();

                if (result != null && result.Any())
                {
                    var param = result.Select(a => new { a.CustId, a.LoginId });

                    var getTags = _accountService.GetDirectCustomerAssociatedByAccount(JsonConvert.SerializeObject(param));

                    result = result.Select(x =>
                    {
                        var getTag = getTags.FirstOrDefault(a => a.CustId == x.CustId || a.LoginId == x.LoginId);

                        if (getTag != null)
                        {
                            x.Tag = getTag.Tag;
                            x.Subscriber = getTag.Subscriber;
                            x.CTSCustId = getTag.CTSCustId;
                        }

                        return x;
                    })
                        .OrderBy(a => a.StatusId)
                        .ToList();

                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDirectAssociationGraph(int ctsCustid, string username)
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
                        ItemCode = (c.CustId.HasValue ? c.CustId.Value : 0) + "_" + now,
                        Username = c.Username,
                        DisplayName = c.Username,
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

        //[HttpPost]
        //public ActionResult GetAssociatedAccountByAccountFilter(AssociatedAccountByAccountFilter filter)
        //{
        //    var msgError = string.Empty;

        //    filter.userId = CurrentSession.UserId;

        //    filter.isMasterSubscriberAssignment = PermissionService.IsMasterSubscriberAssignment();

        //    var data = _accountService.GetAssociatedAccountByAccounts(filter, out msgError);

        //    if (!string.IsNullOrEmpty(msgError))
        //    {
        //        return ResponseError(msgError);
        //    }

        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult GetShortestPath(int startCustId, int endCustId)
        //{
        //    var data = _accountService.GetShortestPath(startCustId, endCustId);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //private ActionResult ResponseError(string msgError)
        //{
        //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //    var errors = new List<string>();
        //    errors.Add(msgError);

        //    return Json(errors, JsonRequestBehavior.AllowGet);
        //}

        //private static Dictionary<AccountStatus, string> GetAccountStatusList()
        //{
        //    var listStatus = new Dictionary<AccountStatus, string>();
        //    listStatus.Add(AccountStatus.Open, ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_OPEN));
        //    listStatus.Add(AccountStatus.Suspended, ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_SUSPENDED));
        //    listStatus.Add(AccountStatus.Closed, ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_CLOSED));

        //    if (PermissionService.IsPermissionCN88())
        //    {
        //        listStatus.Add(AccountStatus.CN88_Active, ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_CN88_NAME_ACTIVE));
        //        listStatus.Add(AccountStatus.CN88_ViewOnly, ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_CN88_NAME_VIEW_ONLY));
        //        listStatus.Add(AccountStatus.CN88_Suspended, ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_CN88_NAME_SUSPENDED));
        //        listStatus.Add(AccountStatus.CN88_Inactive, ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_CN88_NAME_INACTIVE));
        //    }

        //    listStatus.Add(AccountStatus.NotYetDeposit, ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_NOTYETDEPOSIT));

        //    return listStatus;
        //}

        //#endregion

        //#region Associated Device By Account
        //[HttpPost]
        //public ActionResult GetAssociatedDeviceByAccount()
        //{
        //    return PartialView("Associated/DeviceByAccount");
        //}

        //[HttpPost]
        //public ActionResult GetAssociatedDeviceByAccountFilter(AssociatedDeviceByAccountFilter filter)
        //{
        //    var data = _accountService.GetAssociatedDevicesByAccount(filter);

        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetEvidencesByAccount(int accountId)
        //{
        //    var data = _evidenceService.GetEvidencesByAccount(accountId);

        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //#endregion

        //#region Transaction History
        //[HttpPost]
        //[NapAuth(FunctionKey = FSFunctionKeys.TransactionHistory, Policy = NAP.Core.Business.Constant.Policy.Read)]
        //public ActionResult GetTransactionHistory(int accountId)
        //{
        //    return PartialView("TransactionHistory/TransactionHistory");
        //}

        //[HttpPost]
        //[NapAuth(FunctionKey = FSFunctionKeys.TransactionHistory, Policy = NAP.Core.Business.Constant.Policy.Read)]
        //public ActionResult GetTransactionHistoryFilter(TransactionHistoryFilter filter)
        //{
        //    var data = _transactionService.GetTransactionHistory(filter);

        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult GetIPInformation(string ip)
        //{
        //    var data = _transactionService.GetIPInformation(ip);

        //    return PartialView("TransactionHistory/IPInformation", data);
        //}
        //#endregion
    }
}