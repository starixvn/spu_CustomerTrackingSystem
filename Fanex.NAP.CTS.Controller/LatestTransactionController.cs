namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AssociationReport;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.LatestTransaction;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.LatestTransaction, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class LatestTransactionController : BaseController
    {
        private readonly ITransactionService _transService;
        private readonly ICommonService _commonService;

        public override string BaseControllerName { get => Consts.MenuItemReport; }

        public LatestTransactionController()
        {
            _transService = new TransactionService();
            _commonService = new CommonService();
        }

        public ActionResult Index()
        {
            var subscribers = _commonService.GetSubscribers().OrderBy(x => x.SubscriberName).ToList();

            subscribers.Insert(0, new SubscriberGroupModel {  SubscriberID = -1, SubscriberName = "All" });

            ViewBag.Subscribers = subscribers;

            return View();
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.LatestTransaction)]
        public ActionResult GetLatestTransaction(int subscriberId, int length)
        {
            var result = Enumerable.Empty<LatestTransactionViewModel>();

            var data = _transService.GetLatestTransaction(subscriberId, length);
            
            if (data != null && data.Any())
            {
                result = data.OrderByDescending(x=>x.TransTime)
                    .ThenByDescending(x=>x.TransID)
                    .Select(x => new LatestTransactionViewModel
                    {
                        TransID = x.TransID.ToString(),
                        UserName = x.UserName,
                        LoginID = x.LoginID,
                        CTSCustID = x.CTSCustID,
                        TransTimeString = x.TransTime.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                        Device = x.FirstDeviceCode,
                        IP = x.IP,
                        RobotTracking = ((Flagged)x.Flagged).GetStringSlow(),
                        Action = x.Action,
                        ActionResult = x.ActionResult,
                        DeviceType = x.OS,
                        Browser = x.Browser,
                        Domain = x.URLDetails,                        
                        SubscriberName = x.SubscriberName
                    });
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
