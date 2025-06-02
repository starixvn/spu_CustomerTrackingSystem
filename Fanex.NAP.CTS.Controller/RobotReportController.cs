namespace Fanex.NAP.CTS.Controller
{
    using System;
    using System.Web.Mvc;
    using Newtonsoft.Json;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Service.Implement.RobotReport;
    using Fanex.NAP.CTS.Service.Interface.RobotReport;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.RobotReport;
    using Fanex.NAP.CTS.ViewModel.RobotReport;
    using Fanex.NAP.CTS.Resource.Consts;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.RobotReport, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class RobotReportController : BaseController
    {
        private const string _robotReportDailyTemplate = @"~\Template\RobotReport\RobotReportDailyTemplate.xlsx";
        private const string _robotReportMonthlyTemplate = @"~\Template\RobotReport\RobotReportMonthlyTemplate.xlsx";
        private const string _aiRobotReportDailyTemplate = @"~\Template\RobotReport\AIRobotReportDailyTemplate.xlsx";
        private const string _aiRobotReportMonthlyTemplate = @"~\Template\RobotReport\AIRobotReportMonthlyTemplate.xlsx";

        private const string _robotReportDailyFileName = "RobotDailyStatisticReport_{0}_{1}.xlsx";
        private const string _robotReportMonthlyFileName = "RobotMonthlyStatisticReport_{0}_{1}.xlsx";
        private const string _aiRobotReportDailyFileName = "AIRobotDailyStatisticReport_{0}_{1}.xlsx";
        private const string _aiRobotReportMonthlyFileName = "AIRobotMonthlyStatisticReport_{0}_{1}.xlsx";
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
                LogExtension.LogError("Get Robot Report Error: " + ex.Message + ex.StackTrace + "\n"
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
                LogExtension.LogError("Get AI Robot Report Error: " + ex.Message + ex.StackTrace + "\n"
                     + innterException + "\n"
                     + parameterJson);
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.RobotReport_ExportToExcel)]
        public ActionResult ExportRobotReport(RobotReportParamModel param)
        {
            try
            {
                var templatePath = Server.MapPath(param.IsMonthly ? _robotReportMonthlyTemplate : _robotReportDailyTemplate);

                var dateTime = DateTime.Now;
                var fileName = string.Format(
                                param.IsMonthly ? _robotReportMonthlyFileName : _robotReportDailyFileName,
                                dateTime.ToString(FormatDateTimeToString_YYYYmmdd),
                                dateTime.ToString(FormatTimeToString_hhmmss));

                var excelFile = _robotReportService.ExportRobotReport(templatePath, fileName, param);

                return File(excelFile, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - Robot Report Page");
            }

            return new EmptyResult();
        }

        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.RobotReport_ExportToExcel)]
        public ActionResult ExportAIRobotReport(RobotReportParamModel param)
        {
            try
            {
                var templatePath = Server.MapPath(param.IsMonthly ? _aiRobotReportMonthlyTemplate : _aiRobotReportDailyTemplate);

                var dateTime = DateTime.Now;
                var fileName = string.Format(
                                param.IsMonthly ? _aiRobotReportMonthlyFileName : _aiRobotReportDailyFileName,
                                dateTime.ToString(FormatDateTimeToString_YYYYmmdd),
                                dateTime.ToString(FormatTimeToString_hhmmss));

                var excelFile = _robotReportService.ExportAIRobotReport(templatePath, fileName, param);

                return File(excelFile, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                LogExtension.LogError("CTS - Robot Report Page");
            }

            return new EmptyResult();
        }
    }
}
