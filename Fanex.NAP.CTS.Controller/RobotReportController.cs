namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Model.RobotReport;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.ViewModel.RobotReport;
    using System;
    using System.Web.Mvc;
    using Newtonsoft.Json;
    using Fanex.NAP.CTS.Service.Interface.RobotReport;
    using Fanex.NAP.CTS.Service.Implement.RobotReport;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;
    using Fanex.NAP.CTS.Model;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.RobotReport, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class RobotReportController : BaseController
    {
        private const string RobotReportDailyTemplate = @"~\Template\RobotReport\RobotReportDailyTemplate.xls";
        private const string RobotReportMonthlyTemplate = @"~\Template\RobotReport\RobotReportMonthlyTemplate.xls";
        private const string AIRobotReportDailyTemplate = @"~\Template\RobotReport\AIRobotReportDailyTemplate.xls";
        private const string AIRobotReportMonthlyTemplate = @"~\Template\RobotReport\AIRobotReportMonthlyTemplate.xls";
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
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.RobotReport)]
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
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.RobotReport)]
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

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.RobotReportExport)]
        public ActionResult ExportRobotReport(RobotReportParamModel param)
        {
            try
            {
                var templatePath = Server.MapPath(param.IsMonthly ? RobotReportMonthlyTemplate : RobotReportDailyTemplate);

                var dateTime = DateTime.Now;
                var fileName = string.Format(
                                param.IsMonthly ? "RobotStatisticMonthlyReport_{0}_{1}.xlsx" : "RobotStatisticDailyReport_{0}_{1}.xlsx",
                                dateTime.ToString(FormatDateTimeToString_YYYYmmdd),
                                dateTime.ToString(FormatTimeToString_hhmmss));

                var excelFile = _robotReportService.ExportRobotReport(templatePath, fileName, param);

                return File(excelFile, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddMessage("CTS - Robot Report Page")
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return new EmptyResult();
        }

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.RobotReportExport)]
        public ActionResult ExportAIRobotReport(RobotReportParamModel param)
        {
            try
            {
                var templatePath = Server.MapPath(param.IsMonthly ? AIRobotReportMonthlyTemplate : AIRobotReportDailyTemplate);

                var dateTime = DateTime.Now;
                var fileName = string.Format(
                                param.IsMonthly ? "AIRobotStatisticMonthlyReport_{0}_{1}.xlsx" : "AIRobotStatisticDailyReport_{0}_{1}.xlsx",
                                dateTime.ToString(FormatDateTimeToString_YYYYmmdd),
                                dateTime.ToString(FormatTimeToString_hhmmss));

                var excelFile = _robotReportService.ExportAIRobotReport(templatePath, fileName, param);

                return File(excelFile, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddMessage("CTS - Robot Report Page")
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return new EmptyResult();
        }
    }
}
