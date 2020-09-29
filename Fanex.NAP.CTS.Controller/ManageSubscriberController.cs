namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Model.ManageSubscriber;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.ManageSubscriber;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

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
        public ActionResult UpdateSubscriber(SubscriberUpdateModel subscriberInsertModel)
        {
            var result = _manageSubscriberService.UpdateSubscriber(subscriberInsertModel, CurrentSession.UserId);

            return Json(new
            {
                result.ErrorCode,
                result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.ManageSubscriber, Policy = NAP.Core.Business.Constant.Policy.Host)]
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
