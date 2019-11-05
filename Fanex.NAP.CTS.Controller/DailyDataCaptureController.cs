namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Library.Core.Consts;
    using Fanex.NAP.CTS.Library.Criteria.DailyDataCapture;
    using Fanex.NAP.CTS.Library.ExcelConfig.DailyDataCapture;
    using Fanex.NAP.CTS.Library.Services.Implement;
    using Fanex.NAP.CTS.Library.Services.Interface;
    using System;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.DailyDataCapture, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class DailyDataCaptureController : ReportsController
    {
        private readonly IDailyDataCaptureService _service = new DailyCaptureDataService();

        public override string BaseControllerName { get => Consts.MenuItemSuspiciousBets; }

        // GET: DailyDataCapture
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetMatches(DateTime fromDate, DateTime toDate)
        {
            var result = _service.GetMatches(new DailyDataGetCriteria { FromDate = fromDate, ToDate = toDate });
            return Json(result);
        }

        public ActionResult GetMatchesExcel(DateTime fromDate, DateTime toDate)
        {
            var template = Server.MapPath("~/App_Data/Excel Template/DailyDataCapture/template.xlsx");

            var criteria = new DailyDataGetCriteria { FromDate = fromDate, ToDate = toDate };

            var file = RenderExcelFile(template, wb => new DailyDataCaptureExcelConfig().RenderExcel(wb, criteria, _service.GetMatches(criteria), null));

            return ExcelFile(file, GetExcelReportName("DailyDataMonitor"));
        }
    }
}