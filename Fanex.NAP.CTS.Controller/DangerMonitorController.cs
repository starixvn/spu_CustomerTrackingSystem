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
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.AssociationDetection;
    using Fanex.NAP.CTS.ViewModel.DangerMonitor;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.DetectGroup;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.DangerMonitor, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class DangerMonitorController : BaseController
    {
        private readonly IDangerMonitorService _dangerMonitorService;
        private readonly IAssociationDetectionService _associationDetectionService;
        private readonly ICommonService _commonService;

        private const string Region = "DangerDetailRegion";

        private const string MMDetailDoubleGridTemplate = "~/App_Data/Excel Template/DangerMonitor/DoubleGridTemplate.xlsx";
        private const string MMDetailSingleGridTemplate = "~/App_Data/Excel Template/DangerMonitor/SingleGridTemplate.xlsx";

        public override string BaseControllerName { get => Consts.MenuItemMonitor; }

        public DangerMonitorController()
        {
            _dangerMonitorService = new DangerMonitorService(Region, CurrentSession.UserId.ToString());
            _associationDetectionService = new AssociationDetectionService();
            _commonService = new CommonService();
        }

        #region danger main
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetSportBettype()
        {
            var sportBettypeList = _dangerMonitorService.GetSportBettypesFromCache();

            return Json(new
            {
                data = sportBettypeList
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.DangerMonitorRpt, "isSkipTracking")]
        public ActionResult GetDangerMonitor(DangerMonitorSearchModel filter, bool? isSkipTracking)
        {
            var data = default(GetDangerMonitorViewModel);

            var count = 10;
            while ((data?.DangerMonitors == null || data.DangerMonitors.Count() == 0) && filter.LastMatchSection != null)
            {
                if (count-- <= 0)
                {
                    break;
                }

                data = _dangerMonitorService.GetDangerMonitor(filter);
            }

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
                ViewBag.BetTypeSettings = _dangerMonitorService.GetSportBettypesFromCache().Where(x => x.SportType == para.SportType);
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
                   .AddMessage($"{Region} Match Monitor Detail Error: " + ex.Message + ex.StackTrace + "\n"
                    + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                    + ex.InnerException.StackTrace)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return View();
        }
        #endregion

        #region Detail Filter
        [HttpPost]
        public ActionResult GetMatchMonitorFilter(MMDetailInfoViewModel para)
        {
            var result = new MMDetailFilterViewModel();

            try
            {
                result = _dangerMonitorService.GetMatchMonitorFilter(para);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                  .AddMessage($"{Region} Get Match Monitor Filter Error: " + ex.Message + ex.StackTrace + "\n"
                   + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                   + ex.InnerException.StackTrace)
                  .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                  .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion Detail Filter

        #region Detail Total Tickets
        [HttpPost]
        public ActionResult GetMatchMonitorTotalTicket(MMDetailInfoViewModel info, MMDetailFilterViewModel filter)
        {
            long recordsTotal = 0;

            try
            {
                recordsTotal = _dangerMonitorService.GetMatchMonitorTotalTicket(info, filter);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                   .AddMessage($"{Region} Get Match Monitor Total Ticket Error: " + ex.Message + ex.StackTrace + "\n"
                    + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                    + ex.InnerException.StackTrace)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return Json(new
            {
                recordsTotal = recordsTotal,
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion Detail Total Tickets

        #region Detail Tickets
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.DangerMonitorDetailRpt)]
        public ActionResult GetMatchMonitorDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter)
        {
            var mdResult = default(MMDetailTicketListViewModel);

            try
            {
                mdResult = _dangerMonitorService.GetMatchMonitorDetail(para, filter);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                   .AddMessage($"{Region} Get Match Monitor Detail Error: " + ex.Message + ex.StackTrace + "\n"
                    + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                    + ex.InnerException.StackTrace)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);
            }

            return Json(new
            {
                data = mdResult,
                recordsFiltered = (mdResult?.RecordsTotal ?? 0)
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Association Detect

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.DangerMonitorDetailManualDetectRpt)]
        public ActionResult DetectGroupAssWithOptions(DetectGroupAssFilterViewModel model)
        {
            var result = new DetectGroupAssociationListViewModel();

            var orgUserList = _associationDetectionService.ValidateUserName(model.RawUserNames);

            var data = _associationDetectionService.DetectAssociation(new DetectAssociationByUserNamesViewModel
            {
                UserNames = model.RawUserNames,
                IsBP = model.IsOptBP,
                IsDevice = model.IsOptDevice,
                IsIP = model.IsOptIP,
                IsAgent = model.IsOptAgent,
                IsIPLast3Days = model.IsOptIPLast3Days,
                Is3MatchesLast7Days = model.IsOpt3MatchesLast7Days
            }, orgUserList, false, false).Select(x => new DetectGroupAssociationItemViewModel(x)); ;

            result.ItemList = this.RearrangeAssGroup(data, true);

            var mGrList = new MGrListViewModel();
            mGrList.BuildMGrList(result.ItemList, model.RawCustIDs);

            return Json(new
            {
                data = result,
                mGrList = mGrList
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion Association Detect

        #region Export to Excel
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.DangerMonitorDetailExportToExcelRpt)]
        public ActionResult ExportToExcelMMDetail(FormCollection from)
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

        #region private method
        private IEnumerable<DetectGroupAssociationItemViewModel> RearrangeAssGroup(IEnumerable<DetectGroupAssociationItemViewModel> assGroupList, bool isDisableNone)
        {
            var result = new List<DetectGroupAssociationItemViewModel>();

            if (isDisableNone)
                assGroupList = assGroupList.Where(x => x.GroupID != 0);

            if (assGroupList.Any())
            {
                var grouping = assGroupList
                    .GroupBy(x => x.GroupID, (key, g) => new { GroupID = key, Items = g.ToList() })
                    .OrderBy(x => x.GroupID);

                var idx = 1;

                foreach (var group in grouping)
                {
                    var groupID = group.GroupID;
                    var groupName = group.GroupID.ToString();

                    if (groupID.HasValue)
                    {
                        if (groupID == 0)
                        {
                            groupID = -1;
                            groupName = "None";
                        }
                        else
                        {
                            groupID = idx;
                            groupName = idx.ToString();
                            idx += 1;
                        }
                    }

                    result.AddRange(group.Items.Select(x =>
                    {
                        x.GroupID = groupID;
                        x.GroupName = groupName;

                        return x;
                    }));
                }
            }

            return result;
        }
        #endregion
    }
}