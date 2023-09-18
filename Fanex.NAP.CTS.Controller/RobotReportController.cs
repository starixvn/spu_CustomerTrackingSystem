namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Model.RobotReport;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.RobotReport;
    using System;
    using System.Web.Mvc;
    using Newtonsoft.Json;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.RobotReport, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class RobotReportController : BaseController
    {
        public override string BaseControllerName { get => Consts.MenuItemReport; }

        private readonly IRobotReportService _robotReportService;

        public RobotReportController()
        {
            _robotReportService = new RobotReportService();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetRobotReport(RobotReportParamModel param)
        {
            var result = default(RobotReportViewModel);

            try
            {
                result = _robotReportService.GetRobotReport(param);
            }
            catch (Exception ex)
            {
                var parameterJson = JsonConvert.SerializeObject(param ?? new RobotReportParamModel());
                var innterException = ex.InnerException != null ? ex.InnerException.Message + ex.InnerException.Source + ex.InnerException + ex.InnerException.StackTrace : string.Empty;
                LogService.Error("Get Robot Report Error: " + ex.Message + ex.StackTrace + "\n"
                     + innterException + "\n"
                     + parameterJson);
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAIRobotReport(RobotReportParamModel param)
        {
            var result = default(AIRobotReportViewModel);

            try
            {
                result = _robotReportService.GetAIRobotReport(param);
            }
            catch (Exception ex)
            {
                var parameterJson = JsonConvert.SerializeObject(param ?? new RobotReportParamModel());
                var innterException = ex.InnerException != null ? ex.InnerException.Message + ex.InnerException.Source + ex.InnerException + ex.InnerException.StackTrace : string.Empty;
                LogService.Error("Get AI Robot Report Error: " + ex.Message + ex.StackTrace + "\n"
                     + innterException + "\n"
                     + parameterJson);
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
