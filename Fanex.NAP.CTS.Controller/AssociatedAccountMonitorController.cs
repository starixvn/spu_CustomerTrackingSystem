
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

    //[NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountMonitor, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class AssociatedAccountMonitorController: BaseController
    {
        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.AssociatedAccountMonitor,
                            NAP.Core.Business.Constant.Policy.Host);

            return View(isHostPermission);
        }
    }
}
