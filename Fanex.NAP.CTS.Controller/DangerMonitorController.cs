namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using Fanex.NAP.CTS.Model.DangerMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.AssociationDetection;
    using Fanex.NAP.CTS.ViewModel.Common;
    using Fanex.NAP.CTS.ViewModel.DangerMonitor;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.DetectGroup;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    //[NapSuperAuth]
    //[NapAuth(FunctionKey = FSFunctionKeys.DangerMonitor, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class DangerMonitorController : BaseController
    {
        private readonly IDangerMonitorService _dangerMonitorService;
        private readonly IAssociationDetectionService _associationDetectionService;
        private readonly ICommonService _commonService;

        private const string Region = "DangerDetailRegion";

        private const string MMDetailDoubleGridTemplate = "~/App_Data/Excel Template/DangerMonitor/DoubleGridTemplate.xlsx";
        private const string MMDetailSingleGridTemplate = "~/App_Data/Excel Template/DangerMonitor/SingleGridTemplate.xlsx";
        private const string MMDetailBadmintonDoubleGridTemplate = "~/App_Data/Excel Template/DangerMonitor/BadmintonDoubleGridTemplate.xlsx";
        private const string MMDetailVolleyballDoubleGridTemplate = "~/App_Data/Excel Template/DangerMonitor/VolleyballDoubleGridTemplate.xlsx";
        private const string MMDetailCricketDoubleGridTemplate = "~/App_Data/Excel Template/DangerMonitor/CricketDoubleGridTemplate.xlsx";
        private const string MMDetailCricketSingleGridTemplate = "~/App_Data/Excel Template/DangerMonitor/CricketSingleGridTemplate.xlsx";

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
            var sportBettypeList = _dangerMonitorService.GetSportBettypesFromCache(isGetNew: true);

            return Json(new
            {
                data = sportBettypeList
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetLeageGroup()
        {
            var leagueGroup = _dangerMonitorService.GetLeagueGroups();

            return Json(new
            {
                data = leagueGroup
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ValidateUsername(string username)
        {
            var validateUsername = _dangerMonitorService.ValidateUsernameForMain(username);

            return Json(new
            {
                data = validateUsername
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.DangerMonitorRpt, Order = 1)]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.DangerMonitorUsernameRpt, "isSkipTrackingUsername", Order = 2)]
        public ActionResult GetDangerMonitor(DangerMonitorSearchModel filter, bool? isSkipTrackingUsername)
        {
            var data = new GetDangerMonitorResultViewModel();

            if (filter.Validate)
            {
                data = filter.IsRootUserAssociationMode
                    ? _dangerMonitorService.GetDangerMonitorWithAssociationByRootUser(
                          filter,
                          DetectAssociationCustByUserRoot(filter))
                    : _dangerMonitorService.GetDangerMonitorWithNoAssociation(filter);

                data.Items = OrderDangerMonitorItems(data.Items);
            }

            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<GetDangerMonitorItemViewModel> OrderDangerMonitorItems(IEnumerable<GetDangerMonitorItemViewModel> items)
        {
            return items?.OrderBy(x => x.MatchSection)
                .ThenByDescending(x => x.EventDate)
                .ThenByDescending(x => x.MatchID)
                .ThenBy(x => x.BettypeID)
                .ThenBy(x => x.BetID);
        }
        #endregion

        #region danger detail
        [HttpGet]
        public ActionResult Detail(DangerMonitorDetailViewModel para)
        {
            IUserService userService = new UserService();

            try
            {
                var bettypes = _dangerMonitorService.GetBettypesFromCache();

                ViewBag.GroupCCList = _commonService.GetGroupCustomerClass();
                ViewBag.ReasonGroups = this.GetReasonGroups(true);
                ViewBag.EventDateToString = para?.EventDateToString;
                ViewBag.MatchID = para?.MatchID;
                ViewBag.BetTypeID = para?.BetTypeID;
                ViewBag.BettypeName = bettypes.FirstOrDefault(x => x.BetTypeID == para?.BetTypeID && x.BetID == para?.BetID)?.BetTypeName;
                ViewBag.BetIDToString = para?.BetID.ToString();
                ViewBag.BetTypeSettings = bettypes?.Where(x => x.SportType == para.SportType);
                ViewBag.MapX = para?.MX;
                ViewBag.MapYY = para?.MY;
                ViewBag.HomeName = para?.HomeName.ToUpper() ?? string.Empty;
                ViewBag.AwayName = para?.AwayName.ToUpper() ?? string.Empty;
                ViewBag.MatchName = ViewBag.HomeName + "_" + ViewBag.AwayName;
                ViewBag.LiveIndicator = para?.LiveIndicator ?? true;
                ViewBag.Reason = para?.Reason;
                ViewBag.ReasonName = para?.ReasonName ?? string.Empty;
                ViewBag.LeagueName = para?.LeagueName.ToUpper() ?? string.Empty;
                ViewBag.ReasonGroup = para?.ReasonGroup ?? string.Empty;
                ViewBag.MarketName = para?.MarketName ?? string.Empty;
                ViewBag.BetChoice = para?.BetChoice;
                ViewBag.IsDisableScoreFilter = ConditionExcludeScoreForMM.Contains(para.SportType);
                ViewBag.SportType = para?.SportType ?? 0;
                ViewBag.SportName = bettypes.FirstOrDefault(x => x.SportType == para?.SportType)?.SportName;
                ViewBag.KOTime = para?.KOTime;
                ViewBag.IsSupportShowAllTickets = true;
                ViewBag.Danger1 = para.Danger1;
                ViewBag.Danger2 = para.Danger2;
                ViewBag.Danger3 = para.Danger3;
                ViewBag.Danger4 = para.Danger4;
                ViewBag.Danger5 = para.Danger5;
                ViewBag.UsernameAmt = para.UsernameAmt;
                ViewBag.IsHostPermission = true;
                // comment for support QC test - TODO Remove
                //ViewBag.IsHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                //                                                            NapSettings.Settings.SiteId,
                //                                                            FSFunctionKeys.MatchMonitor,
                //                                                            NAP.Core.Business.Constant.Policy.Host);
            }
            catch (Exception ex)
            {
                ex.LogError($"{Region} Match Monitor Detail Error: " + ex.Message + ex.StackTrace + "\n"
                    + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                    + ex.InnerException.StackTrace);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Detail(DangerMonitorDetailViewModel para, string hideRequest)
        {
            if (!hideRequest.IsNotNullOrEmpty())
                return Detail(para);

            try
            {
                var model = JsonConvert.DeserializeObject<DangerMonitorDetailHideRequestModel>(hideRequest);

                var usernames = string.Empty;

                if (model.IsRootUserAssociationMode && model.ListCustID.IsNotNullOrEmpty())
                {
                    var custInfos = _dangerMonitorService.GetCustInfoByCustID(model.ListCustID);

                    var usernameArr = custInfos?
                        .Select(x => x.UserName)
                        .Distinct();

                    usernames = usernameArr.IsNotNullOrEmpty()
                        ? string.Join(",", usernameArr)
                        : string.Empty;
                }
                else
                {
                    usernames = model.Username ?? string.Empty;
                }

                ViewBag.UserName = usernames;
            }
            catch (Exception ex)
            {
                ex.LogError($"{Region} Match Monitor Detail Hide Username Error: ");
            }

            return Detail(para);
        }
        #endregion

        #region Detail Filter
        [HttpPost]
        public ActionResult GetMatchMonitorFilter(MMDetailInfoViewModel para)
        {
            var result = new MMDetailFilterViewModel();

            try
            {
                para.TicketType = TicketType.Single;

                result = _dangerMonitorService.GetMatchMonitorFilter(para);
            }
            catch (Exception ex)
            {
                ex.LogError($"{Region} Get Match Monitor Filter Error: " + ex.Message + ex.StackTrace + "\n"
                   + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                   + ex.InnerException.StackTrace);
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion Detail Filter

        #region Detail Tickets
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.DangerMonitorDetailRpt)]
        public ActionResult GetMatchMonitorDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter)
        {
            var mdResult = default(MMDetailTicketListViewModel);

            try
            {
                para.TicketType = TicketType.Single;

                mdResult = _dangerMonitorService.GetMatchMonitorDetail(para, filter);
            }
            catch (Exception ex)
            {
                ex.LogError($"{Region} Get Match Monitor Detail Error: " + ex.Message + ex.StackTrace + "\n"
                    + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                    + ex.InnerException.StackTrace);
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
            var result = ConfigSettings.IsUsingFanexAPIDirect
                ? ApiHelper.PostWithApiKey<DetectGroupAssociationListViewModel>(
                                               ConfigSettings.GetCtsApiEndPoint(ApiEndpoint.DetectGroupAssWithOptions),
                                               model,
                                               ConfigSettings.ApiKeyFanexAPIDirect).Result
                : DetectGroupAssociation(model);

            var mGrList = new MGrListViewModel();
            mGrList.BuildMGrList(result.ItemList, model.RawCustIDs);

            return Json(new
            {
                data = result,
                mGrList = mGrList
            }, JsonRequestBehavior.AllowGet);
        }

        private DetectGroupAssociationListViewModel DetectGroupAssociation(DetectGroupAssFilterViewModel model)
        {
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
            }, orgUserList, false).Select(x => new DetectGroupAssociationItemViewModel(x));

            return new DetectGroupAssociationListViewModel()
            {
                ItemList = this.RearrangeAssGroup(data, true)
            };
        }

        private string DetectAssociationCustByUserRoot(DangerMonitorSearchModel filter)
        {
            var filterAss = new DetectAssociationByUserRootFilterModel
            {
                UserNames = filter.UserNameList,
                IsAI = filter.RootUserOption.IsAI,
                IsDevice = filter.RootUserOption.IsDevice,
                IsIP = filter.RootUserOption.IsIP,
            };

            var result = ConfigSettings.IsUsingFanexAPIDirect
                ? ApiHelper.PostWithApiKey<IEnumerable<DetectAssociationByUserRootModel>>(
                    ConfigSettings.GetCtsApiEndPoint(ApiEndpoint.DetectAssociationCustByUserRoot),
                    filterAss,
                    ConfigSettings.ApiKeyFanexAPIDirect, ConfigSettings.AssociationDetectionTimeout).Result
                : _associationDetectionService.DetectAssociationCustByUserRoot(filterAss);

            return JsonConvert.SerializeObject(result);
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
            int.TryParse(from["sportType"], out int sportType);

            if (!string.IsNullOrEmpty(isDisplaySingleGrid) && isDisplaySingleGrid.ToLower() == "true")
            {
                var items = serializer.Deserialize<List<ExExcelTicketInfoMMViewModel>>(from["items"]);

                var templatePath = GetExcelTemplatePath(true, sportType);

                var fileName = string.Format("DangerMM_{0}_{1}_{2}.xlsx", matchInfo.MatchName, matchInfo.BetTypeName, matchInfo.EventDateStr);

                var excelFile = _dangerMonitorService.ExportExcelMMDetailSG(items, fileName, templatePath, sportType);

                return File(excelFile, "application/vnd.ms-excel", fileName);
            }
            else
            {
                var homeMItems = serializer.Deserialize<List<ExExcelTicketInfoMMViewModel>>(from["homes"]);
                var awayMItems = serializer.Deserialize<List<ExExcelTicketInfoMMViewModel>>(from["aways"]);

                var templatePath = GetExcelTemplatePath(false, sportType);

                var fileName = string.Format("DangerMM_{0}_{1}_{2}.xlsx", matchInfo.MatchName, matchInfo.BetTypeName, matchInfo.EventDateStr);

                var excelFile = _dangerMonitorService.ExportExcelMMDetailDG(homeMItems, awayMItems, fileName, templatePath, sportType);

                return File(excelFile, "application/vnd.ms-excel", fileName);
            }
        }

        #endregion

        #region private method
        private IEnumerable<ReasonGroupStaticListViewModel> GetReasonGroups(bool isInitGroupNone)
        {
            var reasonGroupItems = _commonService.GetStaticLists((int)StaticListID.Reason, true);
            var result = new List<ReasonGroupStaticListViewModel>();

            if (isInitGroupNone)
            {
                result.Add(new ReasonGroupStaticListViewModel
                {
                    ReasonGroupName = Consts.ReasonDetectNone,
                    ReasonItems = new List<ReasonStaticListViewModel>() { new ReasonStaticListViewModel { ReasonID = "-1", ReasonName = "-", IsSelected = true } }
                });
            }

            result.Add(new ReasonGroupStaticListViewModel
            {
                ReasonGroupName = Consts.ReasonDetectBySystem,
                ReasonItems = reasonGroupItems.Where(x => x.ItemID < 100).Select(x => new ReasonStaticListViewModel { ReasonID = x.ItemValue, ReasonName = x.ItemNameDisplay, IsSelected = x.ItemDefaultValue == "1" })
                    .OrderBy(x => x.ReasonName)
            });

            result.Add(new ReasonGroupStaticListViewModel
            {
                ReasonGroupName = Consts.ReasonDetectByAI,
                ReasonItems = reasonGroupItems.Where(x => x.ItemID >= 100).Select(x => new ReasonStaticListViewModel { ReasonID = x.ItemValue, ReasonName = x.ItemNameDisplay, IsSelected = false })
                    .OrderBy(x => x.ReasonName)
            });

            return result;
        }

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

        private string GetExcelTemplatePath(bool isDisplaySingleGrid, int sportType)
        {
            var templatePath = string.Empty;

            switch (isDisplaySingleGrid)
            {
                case true when (sportType == (int)MatchMonitorSportType.Cricket):
                    templatePath = MMDetailCricketSingleGridTemplate; break;
                case true:
                    templatePath = MMDetailSingleGridTemplate; break;
                case false when (sportType == (int)MatchMonitorSportType.Badminton):
                    templatePath = MMDetailBadmintonDoubleGridTemplate; break;
                case false when (sportType == (int)MatchMonitorSportType.Volleyball):
                case false when (sportType == (int)MatchMonitorSportType.Tennis):
                    templatePath = MMDetailVolleyballDoubleGridTemplate; break;
                case false when (sportType == (int)MatchMonitorSportType.Cricket):
                    templatePath = MMDetailCricketDoubleGridTemplate; break;
                case false:
                    templatePath = MMDetailDoubleGridTemplate; break;
            }

            return Server.MapPath(templatePath);
        }
        #endregion
    }
}