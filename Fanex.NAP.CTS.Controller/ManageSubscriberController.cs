namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.ManageSubscriber;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.ManageSubscriber;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.ManageSubscriber, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class ManageSubscriberController : BaseController
    {
        private readonly IManageSubscriberService _manageSubscriberService;

        public ManageSubscriberController()
        {
            _manageSubscriberService = new ManageSubscriberService();

        }

        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.ManageSubscriber,
                            NAP.Core.Business.Constant.Policy.Host);

            var userId = CurrentSession.UserId;

            return View(new ManageSubscriberViewModel
            {
                IsHostPermission = isHostPermission,
                CustId = userId
            });
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(systemId = SystemId.CTSWeb, functionId = TrackingCtsFunctionId.ManageSubscriberRpt)]
        public ActionResult GetSubscribers()
        {
            var subscribers = _manageSubscriberService.GetSubscribers().OrderByDescending(c => c.IsActive).ThenByDescending(c => c.CreatedDate).ThenByDescending(c=>c.TerminatedDate);

            return Json(new
            {
               data = subscribers
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetailedSite(long subscriberID)
        {
            var detailedSites = _manageSubscriberService.GetDetailedSite(subscriberID);

            return Json(detailedSites, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.ManageSubscriber, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(systemId = SystemId.CTSWeb, functionId = TrackingCtsFunctionId.ManageSubscriberAdd)]
        public ActionResult UpdateSubscriber(SubscriberUpdateModel subscriberInsertModel)
        {
            if (string.IsNullOrEmpty(subscriberInsertModel.IBCUserNamePrefix))
            {
                subscriberInsertModel.IBCUserNamePrefix = string.Empty;
            }

            var result = _manageSubscriberService.UpdateSubscriber(subscriberInsertModel, CurrentSession.UserId);

            Task.Run(() =>
            {
                try
                {
                    //Update all SubID for SiteID which having sub null
                    if (result.Subscribers != null && result.Subscribers.Any())
                    {
                        _manageSubscriberService.ReUpdateSubriberIDBySiteID(result.Subscribers);
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error($"Update Subscriber - Update SubId for Site exception {ex.Message} with Data: {JsonConvert.SerializeObject(result.Subscribers)}", ex);
                }

            });

            return Json(new
            {
                result.Result.ErrorCode,
                result.Result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.ManageSubscriber, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(systemId = SystemId.CTSWeb, functionId = TrackingCtsFunctionId.ManageSubscriberRemove)]
        public ActionResult RemoveSubscriber(int subscriberID)
        {
            var result = _manageSubscriberService.RemoveSubscriber(subscriberID, CurrentSession.UserId);

            return Json(new
            {
                result.ErrorCode,
                result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
