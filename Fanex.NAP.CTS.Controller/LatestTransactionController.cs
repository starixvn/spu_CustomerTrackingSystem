namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Model.AssociationReport;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.ViewModel.LatestTransaction;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.LatestTransaction, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class LatestTransactionController : BaseController
    {
        private readonly ITransactionService _transService;
        private readonly IAssociationReportService _associationService;

        public override string BaseControllerName { get => Consts.MenuItemReport; }

        public LatestTransactionController()
        {
            _transService = new TransactionService();
            _associationService = new AssociationReportService();
        }

        public ActionResult Index()
        {
            var subscribers = _associationService.GetAllSubscriberGroups().ToList();
            subscribers.Insert(0, new SubscriberGroupModel {  SubscriberID = -1, SubscriberName = "All" });

            ViewBag.Subscribers = subscribers;

            return View();
        }

        [HttpPost]
        public ActionResult GetLatestTransaction(int subscriberId, int length)
        {
            var result = Enumerable.Empty<LatestTransactionViewModel>();

            var data = _transService.GetLatestTransaction(subscriberId, length);

            if (data != null && data.Any())
            {
                result = data.Select(x => new LatestTransactionViewModel
                {
                    TransactionDateString = x.TransactionDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                    Username = x.Username,
                    LoginID = x.LoginID,
                    Device = x.Device,
                    IP = x.IP,
                    RobotTracking = x.RobotTracking,
                    Action = x.Action,
                    ActionResult = x.ActionResult,
                    DeviceType = x.DeviceType,
                    Browser = x.Browser,
                    Domain = x.Domain
                });
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
