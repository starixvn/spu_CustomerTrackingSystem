namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor;
    using System;
    using System.Collections.Generic;
    using System.Linq;   
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.AssociatedGroupMonitor, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class AssociatedGroupMonitorController : BaseController
    {
        private readonly IAssociatedGroupMonitorService _associatedGroupService;

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public AssociatedGroupMonitorController()
        {
            _associatedGroupService = new AssociatedGroupMonitorService();
        }

        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.AssociatedGroupMonitor,
                            NAP.Core.Business.Constant.Policy.Host);

            return View(isHostPermission);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult GetAssociatedGroupMonitor()
        {
            var associatedGroups = Enumerable.Empty<AssociatedGroupMonitorViewModel>();

            var data = _associatedGroupService.GetAssociatedGroupMonitor();

            if (data != null && data.Any())
            {
                associatedGroups = data.Select(x => new AssociatedGroupMonitorViewModel
                {
                    GroupID = x.GroupID,
                    GroupName = x.GroupName,
                    ABI = x.ABI,
                    TotalAccount = x.TotalAccount,
                    Remark = x.Remark,
                    CreatedDate = x.CreatedDate.ToString(Consts.FormatDateToString_MMMddyyyy),
                    CreatedBy = x.CreatedBy
                });
            }
            else
            {
                associatedGroups = GenegateDataTest();
            }

            return Json(new { data = associatedGroups }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<AssociatedGroupMonitorViewModel> GenegateDataTest()
        {
            var list = new List<AssociatedGroupMonitorViewModel>();

            for (int i = 1; i < 1000; i++)
            {
                list.Add(new AssociatedGroupMonitorViewModel
                {
                    GroupID = i,
                    GroupName = "Name" + i,
                    CreatedBy = "sa",
                    ABI = 47,
                    Remark = "Remark",
                    CreatedDate = DateTime.Now.ToString(Consts.FormatDateToString_MMMddyyyy),
                    TotalAccount = i + 20
                });
            }

            return list;
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedGroupMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult AddAssociatedGroupMonitor(AddAssociatedGroupViewModel model)
        {
            var result = _associatedGroupService.AddAssociatedGroupMonitor(model);

            return Json(new
            {
                errorCode = result.ErrorCode,
                errorMessage = result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedGroupMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult RemoveAssociatedGroupMonitor(RemoveAssociatedGroupViewModel model)
        {
            var result = _associatedGroupService.RemoveAssociatedGroupMonitor(model);

            return Json(new
            {
                errorCode = result.ErrorCode,
                errorMessage = result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedGroupMonitor, Policy = NAP.Core.Business.Constant.Policy.Host)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.AssociatedGroupMonitorRpt)]
        public ActionResult AddAssociatedAccount(AddAssociatedAccountViewModel model)
        {
            var result = _associatedGroupService.AddAssociatedAccount(model);

            return Json(new
            {
                errorCode = result.ErrorCode,
                errorMessage = result.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
