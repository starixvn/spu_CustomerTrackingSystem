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

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.ManageSubscriber, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult InsertSubscriber(SubscriberInsertModel subscriberInsertModel)
        {
            var result = _manageSubscriberService.InsertSubscriber(subscriberInsertModel, CurrentSession.UserId);

            return Json(new
            {
                result.ErrorCode,
                result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.ManageSubscriber, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult VerifySubscriberToInsert(string prefixSubscriberName)
        {
            var result = _manageSubscriberService.VerifySubscriberToInsert(prefixSubscriberName);

            return Json(new
            {
               IsSubscriberPrefixValidation = (result != null && result.Count > 0)? 1 : 0
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.ManageSubscriber, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult EditSubscriber(SubscriberInsertModel subscriberInsertModel)
        {
            var result = _manageSubscriberService.UpdateSubscriber(subscriberInsertModel);

            return Json(new
            {
                result.ErrorCode,
                result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }


        [NapAuth(FunctionKey = FSFunctionKeys.ManageSubscriber, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult RemoveSubscriber(int subscriberID, int siteID)
        {
            var result = _manageSubscriberService.RemoveSubscriber(subscriberID, siteID);

            return Json(new
            {
                result.ErrorCode,
                result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
