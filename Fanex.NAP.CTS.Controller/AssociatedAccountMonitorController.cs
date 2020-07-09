
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
            var result = new AssociatedAccountMonitorDetailViewModel();
            var totalItem = 0;
            //.toString("MMM dd, yyyy")

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


        private AssociatedAccountMonitorDetailViewModel GetResultAccountData(IEnumerable<AssociatedAccountMonitorModel> associatedAccounts)
        {
            var result = new AssociatedAccountMonitorDetailViewModel();

            result.AssociatedAccount = associatedAccounts;
            var tasks = new List<Task>
            {
                Task.Run(() =>
                {
                    result.Subscriber = associatedAccounts.Select(c=>c.SubscriberName).Distinct();

                    //result.AssociatedAccount..GroupList = ticketsModel.GroupBy(x => new { x.GroupID })
                    //     .Select(s => new DetailGroup { GroupId = s.Key.GroupID, GroupName = s.Key.GroupID.ToString() }).Distinct();
                }),

                Task.Run(() =>
                {
                    result.UserName =  associatedAccounts.Select(c=>c.Username).Distinct();
                    //     .Select(s => new DetailGroup { GroupId = s.Key.GroupID, GroupName = s.Key.GroupID.ToString() }).Distinct();
                }),
            };

            Task.WhenAll(tasks.ToArray()).Wait();
            return result;
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
