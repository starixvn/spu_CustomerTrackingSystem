
namespace Fanex.NAP.CTS.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.Logging;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.CTS.Model.AssociatedAccountMonitor;
    using Fanex.NAP.CTS.ViewModel;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.ViewModel.AssociatedAccountMonitor;

    [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountMonitor, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class AssociatedAccountMonitorController: BaseController
    {
        private readonly IAssociatedAccountService _associatedAccountService;

        public AssociatedAccountMonitorController()
        {
            _associatedAccountService = new AssociatedAccountService();
        }

        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.AssociatedAccountMonitor,
                            NAP.Core.Business.Constant.Policy.Host);

            return View(isHostPermission);
        }

        [HttpGet]
        public ActionResult GetCustProfile()
        {
            IUserService userService = new UserService();

            var result = new CustProfilerModel
            {
                CustID = CurrentSession.UserId,
                IsHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.AssociatedAccountMonitor,
                            NAP.Core.Business.Constant.Policy.Host)
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult InsertManualAssociatedAccount(string custIDList, string toUserNameList, string remark)
        {
            var resultInsert = new InsertManualAssociatedAccountViewModel { ErrorCode = -1 };
            bool isException = false;

            var resultValidated = _associatedAccountService.ValidateAssociatedAccount(toUserNameList, out isException);

            if (resultValidated.Any())
            {
                var invalidOrDuplicatedList = GetInvalidOrDuplicated(resultValidated, isException);

                resultInsert.ResultValidated = invalidOrDuplicatedList;

                var isNotAllowInsert = invalidOrDuplicatedList.UserNameInvalidList.Any() || invalidOrDuplicatedList.UserNameDuplicatedList.Any();

                return isNotAllowInsert
                    ? Json(resultInsert, JsonRequestBehavior.AllowGet)
                    : HandleAssociatedAccountInsertion(custIDList, remark);
            }

            return Json(resultInsert, JsonRequestBehavior.AllowGet);
        }
        
       
        
        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult ValidateManualAssociationMultiple(string userNameList)
        {
            var viewModel = new ValidatedAcssociatedAccountViewModel();
            bool isException = false;

            var resultValidated = _associatedAccountService.ValidateAssociatedAccount(userNameList, out isException);

            return resultValidated.Any()
                ? Json(GetInvalidOrDuplicated(resultValidated, isException), JsonRequestBehavior.AllowGet)
                : Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAssociatedAccountMonitor()
        {
            var result = new List<AssociatedAccountMonitorViewModel>();
            var totalItem = 0;

            var associatedAccounts = _associatedAccountService.GetAssociatedAccountMonitor();

            if (associatedAccounts != null && associatedAccounts.Any())
            {
                totalItem = associatedAccounts.Count();

                result = GetResultAccountData(associatedAccounts);
            }

            return Json(new
            {
                recordsTotal = totalItem,
                recordsFiltered = totalItem,
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult RemoveAssociatedaccount(long ctsCustId)
        {
            if (ctsCustId <= 0)
            {
                return Json(new
                {
                    errorCode = -100,
                    errorMessage = "Invalid parameter"
                }, JsonRequestBehavior.AllowGet);
            }

            var result = _associatedAccountService.RemoveAssociatedAccount(ctsCustId, CurrentSession.UserId);

            return Json(new
            {
                errorCode = result.ErrorCode,
                errorMessage = result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        private List<AssociatedAccountMonitorViewModel> GetResultAccountData(IEnumerable<AssociatedAccountMonitorModel> associatedAccounts)
        {
            var result = associatedAccounts.Select(x => new AssociatedAccountMonitorViewModel
                {
                    CTSCustId = x.CTSCustId,
                    CustId = x.CustId,
                    Username = x.Username,
                    Login = x.Username2,
                    AccountStatus = x.AccountStatus,
                    SubscriberName = x.SubscriberName,
                    Remark = x.Remark,
                    CreatedBy = x.CreatedBy,
                    CreatedTime = x.CreatedTime.ToString("MMM dd, yyyy")
                }).OrderByDescending(x => x.Username);

            return result.ToList();
        }

        private ValidatedAcssociatedAccountViewModel GetInvalidOrDuplicated(IEnumerable<ValidateAssociatedAccountModel> userNameList, bool isException)
        {
            return new ValidatedAcssociatedAccountViewModel
            {
                IsException = isException,
                ValideCTSCustId = string.Join(",",userNameList.Where(x => x.CTSCustID.HasValue == true && x.AccountStatus == 0).Select(c=>c.CTSCustID)),
                UserNameInvalidList = userNameList.Where(x => x.AccountStatus == 3 || x.AccountStatus == 4).Select(x => x.UserName),
                UserNameDuplicatedList = userNameList.Where(x => x.AccountStatus == 1 || x.AccountStatus == 2).Select(x => x.UserName)
            };
        }

        private ActionResult HandleAssociatedAccountInsertion(string custIDList, string remark)
        {

            var dbInsert = _associatedAccountService.InsertAssociatedAccountMultiple(CurrentSession.UserId, custIDList, remark);

            var resultInsert = new InsertManualAssociatedAccountViewModel()
            {
                ErrorCode = dbInsert.ErrorCode,
                ErrorMessage = dbInsert.ErrorMsg
            };

            return Json(resultInsert, JsonRequestBehavior.AllowGet);
        }
    }
}
