namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.DangerMonitor;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.DangerMonitor;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.DangerMonitor, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class DangerMonitorController : MatchMonitorController
    {
        private readonly IDangerMonitorService _dangerMonitorService;

        public DangerMonitorController() : base()
        {
            _dangerMonitorService = new DangerMonitorService();
            Region = "DangerDetailRegion";
            ControllerName = "Danger Monitor";
            MMDetailDoubleGridTemplate = "~/App_Data/Excel Template/DangerMonitor/DoubleGridTemplate.xlsx";
            MMDetailSingleGridTemplate = "~/App_Data/Excel Template/DangerMonitor/SingleGridTemplate.xlsx";
        }

        #region danger main
        [HttpGet]
        public override ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetSportBettype()
        {
            var sportBettypeList = _dangerMonitorService.GetSportBettypes();

            return Json(new
            {
                data = sportBettypeList
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.DangerMonitorRpt, "isSkipTracking")]
        public ActionResult GetDangerMonitor(DangerMonitorSearchModel filter, bool? isSkipTracking)
        {
            var data = _dangerMonitorService.GetDangerMonitor(filter);
            return Json(new
            {
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region danger detail
        [HttpGet]
        public ActionResult Detail(DangerMonitorDetailViewModel para)
        {
            IUserService userService = new UserService();

            try
            {
                ViewBag.GroupCCList = _commonService.GetGroupCustomerClass();
                ViewBag.EventDateToString = para?.EventDateToString;
                ViewBag.MatchID = para?.MatchID;
                ViewBag.BetTypeID = para?.BetTypeID;
                ViewBag.BettypeName = para?.BettypeName ?? string.Empty;
                ViewBag.BetIDToString = para?.BetID.ToString();
                ViewBag.BetTypeSettings = _dangerMonitorService.GetSportBettypes().Where(x => x.SportType == para.SportType);
                ViewBag.MapX = para?.MX;
                ViewBag.MapYY = para?.MY;
                ViewBag.HomeName = para?.HomeName.ToUpper() ?? string.Empty;
                ViewBag.AwayName = para?.AwayName.ToUpper() ?? string.Empty;
                ViewBag.LiveIndicator = para?.LiveIndicator ?? true;
                ViewBag.Reason = para?.Reason;
                ViewBag.ReasonName = para?.ReasonName ?? string.Empty;
                ViewBag.LeagueName = para?.LeagueName.ToUpper() ?? string.Empty;
                ViewBag.ReasonGroup = para?.ReasonGroup ?? string.Empty;
                ViewBag.MarketName = para?.MarketName ?? string.Empty;
                ViewBag.BetChoice = para?.BetChoice;
                ViewBag.IsDisableScoreFilter = para?.SportType != null && para.SportType == (int)MatchMonitorSportType.Basketball;
                ViewBag.SportType = para?.SportType ?? 0;
                ViewBag.SportName = para?.SportName ?? string.Empty;
                ViewBag.KOTime = para?.KOTime;
                ViewBag.IsSupportShowAllTickets = true;
                ViewBag.Danger1 = para.Danger1;
                ViewBag.Danger2 = para.Danger2;
                ViewBag.Danger3 = para.Danger3;
                ViewBag.Danger4 = para.Danger4;
                ViewBag.Danger5 = para.Danger5;
                ViewBag.IsHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                                                                            NapSettings.Settings.SiteId,
                                                                            FSFunctionKeys.MatchMonitor,
                                                                            NAP.Core.Business.Constant.Policy.Host);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                   .AddMessage($"{ControllerName} Detail Error: " + ex.Message + ex.StackTrace + "\n"
                    + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                    + ex.InnerException.StackTrace)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return View();
        }
        #endregion

        #region Detail Tickets
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.DangerMonitorDetailRpt)]
        public override ActionResult GetMatchMonitorDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter)
        {
            return base.GetMatchMonitorDetail(para, filter);
        }
        #endregion

        #region Association Detect

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.DangerMonitorDetailManualDetectRpt)]
        public override ActionResult DetectGroupAssWithOptions(DetectGroupAssFilterViewModel model)
        {
            return base.DetectGroupAssWithOptions(model);
        }

        #endregion Association Detect

        #region Export to Excel
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.DangerMonitorDetailExportToExcelRpt)]
        public override ActionResult ExportToExcelMMDetail(FormCollection from)
        {
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var matchInfo = serializer.Deserialize<ExExlMatchInfoViewModel>(from["match"]);
            var isDisplaySingleGrid = from["isDisplaySingleGrid"];

            if (!string.IsNullOrEmpty(isDisplaySingleGrid) && isDisplaySingleGrid.ToLower() == "true")
            {
                var items = serializer.Deserialize<List<ExExcelTicketInfoViewModel>>(from["items"]);

                var templatePath = Server.MapPath(MMDetailSingleGridTemplate);

                var fileName = string.Format("DangerMM_{0}_{1}_{2}.xlsx", matchInfo.MatchName, matchInfo.BetTypeName, matchInfo.EventDateStr);

                var excelFile = _dangerMonitorService.ExportExcelMMDetailSG(items, fileName, templatePath);

                return File(excelFile, "application/vnd.ms-excel", fileName);
            }
            else
            {
                var homeMItems = serializer.Deserialize<List<ExExcelTicketInfoViewModel>>(from["homes"]);
                var awayMItems = serializer.Deserialize<List<ExExcelTicketInfoViewModel>>(from["aways"]);
                var templatePath = Server.MapPath(MMDetailDoubleGridTemplate);

                var fileName = string.Format("DangerMM_{0}_{1}_{2}.xlsx", matchInfo.MatchName, matchInfo.BetTypeName, matchInfo.EventDateStr);

                var excelFile = _dangerMonitorService.ExportExcelMMDetailDG(homeMItems, awayMItems, fileName, templatePath);

                return File(excelFile, "application/vnd.ms-excel", fileName);
            }
        }

        #endregion

        #region disable function
        #region match monitor
        [NonAction]
        public override ActionResult GetMatchMonitor(MatchMonitorSearchViewModel para, bool? isSkipTracking)
        {
            return base.GetMatchMonitor(para, isSkipTracking);
        }

        [NonAction]
        public override ActionResult Detail(MatchMonitorDetailViewModel para)
        {
            return base.Detail(para);
        }
        #endregion

        #region Verify Trans

        [NonAction]
        public override ActionResult VerifyTransMMDetail(MatchMonitorDetailVerifyTransParaModel para)
        {
            return base.VerifyTransMMDetail(para);
        }

        #endregion Verify Trans

        #region User Feedback
        [NonAction]
        public override ActionResult GetUserFeedback(UserFeedbackViewModel para)
        {
            return base.GetUserFeedback(para);
        }

        [NonAction]
        public override ActionResult InsertUserFeedback(InsertUserFeedbackViewModel para)
        {
            return base.InsertUserFeedback(para);
        }

        [NonAction]
        public override ActionResult UpdateUserFeedback(UpdateUserFeedbackViewModel para)
        {
            return UpdateUserFeedback(para);
        }

        [NonAction]
        public override ActionResult DeleteUserFeedback(DeleteUserFeedbackViewModel para)
        {
            return base.DeleteUserFeedback(para);
        }

        #endregion User Feedback
        #endregion
    }
}