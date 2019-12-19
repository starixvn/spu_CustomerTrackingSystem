namespace Fanex.NAP.CTS.Controller
{
    using System.Net;
    using System.Web.Mvc;
    using System.Collections.Generic;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.ViewModel.Account;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.ViewModel.Evidence;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.CTS.ViewModel.Account.GeneralInformation;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.ViewModel.Account.Evidence;
    using Fanex.NAP.CTS.ViewModel.Account.Evidence.Exception;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Resource;
    using Fanex.NAP.CTS.ViewModel.Account.TransactionHistory;
    using Fanex.NAP.CTS.Model.Account;
    using System.Threading.Tasks;
    using System.Linq;

    [NapAuth(FunctionKey = FSFunctionKeys.SearchAccount, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        //private readonly IEvidenceService _evidenceService;
        //private readonly ITransactionService _transactionService;

        public AccountController()
        {
            _accountService = new AccountService();
            //_evidenceService = new EvidenceService();
            //_transactionService = new TransactionService();
        }

        public override string BaseControllerName { get => Consts.MenuItemSearch; }

        public ActionResult Index(int ctsCustId)
        {
            if (ctsCustId > 0)
            {
                var tasks = new List<Task>();

                var customerInfoFromCTS = _accountService.GetCustomerInfoByCTSCustId(ctsCustId);

                if (customerInfoFromCTS != null)
                {
                    var customerGeneralStatusViewModel = new CustomerGeneralStatusViewModel();
                    var customerStatus = new CustomerGeneralStatusModel();
                    var customerTotalAssociation = new CustomerAssociationByCTSCustIdModel();
                    var customerProfileFraudInfo = new CustomerProfileFraudInfoModel();

                    var customerEvidences = new List<CustomerEvidenceByCTSCustIdModel>();

                    tasks.Add(Task.Run(() => { customerStatus = _accountService.GetCustomerGeneralStatus(customerInfoFromCTS.CustId); }));
                    tasks.Add(Task.Run(() => { customerTotalAssociation = _accountService.GetCustomerTotalAssociationByCTSCustId(ctsCustId); }));
                    tasks.Add(Task.Run(() => { customerEvidences = _accountService.GetCustomerEvidenceByCTSCustId(ctsCustId); }));
                    tasks.Add(Task.Run(() => { customerProfileFraudInfo = _accountService.GetCustomerProfileFraudInfo(customerInfoFromCTS.CustId); }));

                    Task.WhenAll(tasks.ToArray()).Wait();

                    customerGeneralStatusViewModel
                        .BindingGeneralDataFromMainDB(customerStatus)
                        .BindingGeneralDataFromCTS(customerInfoFromCTS)
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
        public ActionResult GetAssociatedAccountByAccount()
        {
            var getCustomerStaticStatus = _accountService.GetCustomerStaticStatus(1, true);

            var getCustomerStaticAssType = _accountService.GetCustomerStaticStatus(4, true);

            var associatedByAccountViewModel = new AssociatedByAccountViewModel
            {
                AccountStatusList = getCustomerStaticStatus,
                AssociationTypeList = getCustomerStaticAssType,
                CustomerInfoDetailList = new List<CustomerInfoDetailModel>(),
                Status = getCustomerStaticStatus.FirstOrDefault().ItemId,
                AssociationType = getCustomerStaticAssType.FirstOrDefault().ItemId
            };

            return PartialView("Associated/AccountByAccount", associatedByAccountViewModel);
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