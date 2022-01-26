namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.ProblemCaseMonitor;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.ProblemCaseMonitor;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.ProblemCaseMonitor, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class ProblemCaseMonitorController : BaseController
    {
        private const string ProblemCaseTemplate = @"~\Template\ProblemCaseTemplate.xls";
        private readonly IProblemCaseMonitorService _problemCaseMonitorService;
        private readonly ICommonService _commonService;

        public ProblemCaseMonitorController()
        {
            _problemCaseMonitorService = new ProblemCaseMonitorService();
            _commonService = new CommonService();
        }

        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.ProblemCaseMonitor,
                            NAP.Core.Business.Constant.Policy.Host);

            var userId = CurrentSession.UserId;
            var problemCaseOptions = _problemCaseMonitorService.GetProblemCasesFilter((int)STATIC_LIST.PROBLEMCASE, true).ToList();
            var subscriber = _commonService.GetSubscriberGroups().ToList();

            return View(new ProblemCaseViewModel
            {
                IsHostPermission = isHostPermission,
                CustId = userId,
                ProblemCases = problemCaseOptions,
                Subscriber = subscriber
            });
        }

        [HttpPost]
        [LogActionFilterAttribute(systemId = SystemId.CTSWeb, functionId = MonitorCtsFunctionId.ProblemCaseMonitor)]
        public ActionResult GetProblemCasesMonitor(ProblemCasesMonitorFilterModel filter)
        {
            var result = new List<ProblemCasesMonitorModel>();

            try
            {
                result = _problemCaseMonitorService.GetProblemCaseMonitor(ConfigSettings.GetProblemCaseMonitor, filter).ToList();
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddMessage("CTS - Problem Case Monitor Page")
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportProblemCases(ProblemCasesMonitorFilterModel filter)
        {
            try
            {
                var templatePath = Server.MapPath(ProblemCaseTemplate);

                var fileName = string.Format(
                                "ProblemCasesMonitor_{0}_{1}.xlsx",
                                filter.FromDate.ToString(FormatDateTimeToString_YYYYmmdd),
                                filter.ToDate.ToString(FormatDateTimeToString_YYYYmmdd));

                var sheetName = "Problem Cases Monitor";

                var result = _problemCaseMonitorService.GetProblemCaseMonitor(ConfigSettings.GetProblemCaseMonitor, filter).ToList();

                var excelFile = _problemCaseMonitorService.ExportProblemCaseMonitor(result, fileName, templatePath, sheetName, filter);

                return File(excelFile, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddMessage("CTS - Problem Case Monitor Page")
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return new EmptyResult();
        }
    }
}