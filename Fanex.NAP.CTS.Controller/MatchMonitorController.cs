namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitor, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class MatchMonitorController : BaseController
    {
        private const string MMDetailTemplate = "~/App_Data/Excel Template/MatchMonitor/Template.xlsx";
        private readonly IMatchMonitorService _matchMonitorService;
        private readonly IAssociationDetectionService _associationDetectionService;

        public MatchMonitorController()
        {
            _matchMonitorService = new MatchMonitorService();
            _associationDetectionService = new AssociationDetectionService();
        }

        #region Main Page
        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.MatchMonitor,
                            NAP.Core.Business.Constant.Policy.Host);

            var userId = CurrentSession.UserId;

            var mmStatic = _matchMonitorService.GetMMStaticList();
          
            return View(new MatchMonitorViewModel
            {
                IsHostPermission = isHostPermission,
                CustId = userId,               
                Reasons = mmStatic.Reasons,
                Sports = mmStatic.Sports               
            });
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorRpt, "isSkipTracking")]
        public ActionResult GetMatchMonitor(MatchMonitorFilterViewModel para, bool? isSkipTracking)
        {
            var matchMonitors = Enumerable.Empty<GetMatchMonitorViewModel>();

            try
            {
                if (para != null
                    && !string.IsNullOrEmpty(para.BetTypes)
                    && !string.IsNullOrEmpty(para.MatchSection)
                    && !string.IsNullOrEmpty(para.Sports)
                    && !string.IsNullOrEmpty(para.Reasons)
                    && IsValidDateRange(para.FromDate, para.ToDate))
                {
                    var mmData = _matchMonitorService.GetMatchMonitor(para);
                    var mmStatic = _matchMonitorService.GetMMStaticList();

                    matchMonitors = mmData?.Select(x => new GetMatchMonitorViewModel(x, mmStatic)).OrderBy(c => c.MatchSection);

                    if (!string.IsNullOrEmpty(para.Event))
                    {
                        matchMonitors = matchMonitors.Where(x => x.HomeName.Contains(para.Event)
                        || x.AwayName.Contains(para.Event)
                        || x.LeagueName.Contains(para.Event));
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Info("Get Match Monitor Error: " + ex.Message + ex.StackTrace + "\n"
                    + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                    + ex.InnerException.StackTrace);
            }

            return Json(new
            {
                data = matchMonitors
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Detail Page
        [HttpGet]
        public ActionResult Detail(MatchMonitorDetailViewModel para)
        {
            IUserService userService = new UserService();
            var matchGroup = Enumerable.Empty<MatchGroupModel>();
            var matchScore = Enumerable.Empty<MatchScoreModel>();

            try
            {
                if (para != null)
                {
                    matchGroup = _matchMonitorService.GetMatchGroup(para.MatchID, para.BetTypeID, para.LiveIndicator, para.BetID, para.Reason)?.ToList() ?? new List<MatchGroupModel>();

                    if (para.SportType != (int)MatchMonitorSportType.Basketball)
                    {
                        matchScore = _matchMonitorService.GetMatchScore(para.MatchID, para.BetTypeID, para.LiveIndicator, para.BetID, para.Reason) ?? new List<MatchScoreModel>();
                    }

                    //if (para.Reason != (int)MatchMonitorReason.FixedGame)
                    //{
                    //    MappingReasonGroupName(ref matchGroup);
                    //}
                }
            }
            catch (Exception ex)
            {
                LogService.Info("Match Monitor Detail Error: " + ex.Message + ex.StackTrace + "\n"
                     + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                     + ex.InnerException.StackTrace);
            }

            ViewBag.EventDateToString = para?.EventDateToString;
            ViewBag.MatchID = para?.MatchID;
            ViewBag.BetTypeID = para?.BetTypeID;
            ViewBag.BettypeName = para?.BettypeName ?? string.Empty;
            ViewBag.BetIDToString = para?.BetID.ToString();
            ViewBag.MatchName = para?.MatchName ?? string.Empty;
            ViewBag.LiveIndicator = para?.LiveIndicator ?? true;
            ViewBag.Reason = para?.Reason;
            ViewBag.ReasonName = para?.ReasonName ?? string.Empty;
            ViewBag.Group = matchGroup;
            ViewBag.MatchScore = matchScore?.OrderBy(c => c.ScoreDiff).ThenBy(c => c.LiveHomeScore).ThenBy(c => c.LiveAwayScore);
            ViewBag.LeagueName = para?.LeagueName ?? string.Empty;
            ViewBag.ReasonGroup = para?.ReasonGroup ?? string.Empty;
            ViewBag.MarketName = para?.MarketName ?? string.Empty;
            ViewBag.BetChoice = para?.BetChoice;
            ViewBag.IsDisableScoreFilter = para?.SportType != null && para.SportType == (int)MatchMonitorSportType.Basketball;
            ViewBag.SportName = para?.SportName ?? string.Empty;
            ViewBag.KOTime = para?.KOTime;
            ViewBag.IsHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                                                                        NapSettings.Settings.SiteId,
                                                                        FSFunctionKeys.MatchMonitor,
                                                                        NAP.Core.Business.Constant.Policy.Host);

            return View();
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorDetailRpt)]
        public ActionResult GetMatchMonitorDetail(MatchMonitorDetailFilterViewModel para)
        {
            var mdResult = new MatchMonitorDetailListViewModel();
            var mdItemList = Enumerable.Empty<MatchMonitorDetailItemViewModel>();

            try
            {
                if (para != null)
                {
                    var matchDetailInfo = _matchMonitorService.GetMatchMonitorDetail(para);

                    if (matchDetailInfo != null && matchDetailInfo.MatchDetails.Any())
                    {
                        var custInfo = Enumerable.Empty<CustInfoModel>();
                        var mappingTransInfo = Enumerable.Empty<TransGroupModel>();
                        var ticketDetail = Enumerable.Empty<TicketDetailModel>();

                        var tasks = new List<Task>
                        {
                            Task.Run(() =>
                            {
                                var mmStatic = _matchMonitorService.GetMMStaticList();
                                mappingTransInfo = MappingTransInfo(matchDetailInfo, mmStatic.Reasons);
                            }),
                            Task.Run(() =>
                            {
                                var custIdsOfTotalMatch  =  matchDetailInfo.Custs.Select(c => c.CustID).Distinct();
                                custInfo = _matchMonitorService.GetCustInfoByCustID(string.Join(",", custIdsOfTotalMatch));
                            }),
                            Task.Run(() =>
                            {
                                var tranIdsOfTotalMatch =  matchDetailInfo.MatchDetails.Select(c => c.TransID).Distinct();
                                ticketDetail = _matchMonitorService.GetTicketDetail(para.EventDate, string.Join(",",tranIdsOfTotalMatch));
                            })
                        };

                        Task.WhenAll(tasks.ToArray()).Wait();

                        var mergeInfo = MergeMatchMonitorDetail(mappingTransInfo, custInfo, ticketDetail);

                        mdItemList = GetRobotStatus(mergeInfo, para.BetTypeID);

                        mdItemList = FilterSite(mdItemList, para.Sites);

                        var custs = mdItemList.Select(x => x.CTSCustID).Distinct();

                        mdResult = new MatchMonitorDetailListViewModel
                        {
                            CTSCustIDList = string.Join(",", custs),
                            TotalCust = custs.Count(),
                            HomeMatchItemList = mdItemList.Where(c => string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.H), StringComparison.OrdinalIgnoreCase)),
                            AwayMatchItemList = mdItemList.Where(c => string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.A), StringComparison.OrdinalIgnoreCase)),                           
                            ScoreList = mdItemList.Select(x => x.Score).Distinct().OrderBy(x => x)
                        };
                        mdResult.BindReasons(mdItemList.Select(x => x.ReasonList));
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Info("Get Match Monitor Detail Error: " + ex.Message + ex.StackTrace + "\n"
                      + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                      + ex.InnerException.StackTrace);
            }

            return Json(new { data = mdResult }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DetectGroupAssDefault(string ctsCustIDList)
        {
            var result = new DetectGroupAssociationListViewModel();
            var mergeGroupInfo = new MergeGroupInfoModel { CTSCustIDs = ctsCustIDList };

            var assByPrimary = _associationDetectionService.DetectAssociationByPrimary(new DetectAssociationByPrimaryFilterModel
            {
                CTSCustIDs = ctsCustIDList,
                IsBP = true,
                IsDevice = true,
                IsIP = true
            });

            if (assByPrimary.Any())
            {
                mergeGroupInfo.AssociationByPrimaryJson = JsonConvert.SerializeObject(assByPrimary);

                var data = _associationDetectionService.DetectGroupDetectAssociation(mergeGroupInfo).Select(x => new DetectGroupAssociationItemViewModel(x));

                result = new DetectGroupAssociationListViewModel
                {
                    ItemList = data,
                    GroupList = data.Select(x => new GroupInfoViewModel { GroupID = x.GroupID, GroupName = x.GroupName })
                                    .Distinct(x => x.GroupID).OrderBy(x => x.GroupID)
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DetectGroupAssWithOptions(DetectGroupAssFilterViewModel model)
        {
            var result = new DetectGroupAssociationListViewModel();
            var mergeGroupInfo = new MergeGroupInfoModel { CTSCustIDs = model.CTSCustIDs };

            Parallel.Invoke(() =>
            {
                var detectAssByIP = model.IsOptIPLast3Days ? _associationDetectionService.DetectAssociationByIP(model.CustIDs) : Enumerable.Empty<DetectAssociationBySharedIPMatchesModel>();

                mergeGroupInfo.AssociationByIPJson = this.GetAssociationCust(detectAssByIP);
            }, () => {
                var detectAssByAssMatches = model.IsOpt3MatchesLast7Days ? _associationDetectionService.DetectAssociationBySharedMatches(model.CustIDs) : Enumerable.Empty<DetectAssociationBySharedIPMatchesModel>();

                mergeGroupInfo.AssociationByMatchesJson = this.GetAssociationCust(detectAssByAssMatches);
            },
            () => {
                if (model.IsOptBP || model.IsOptDevice || model.IsOptIP)
                {
                    var assByPrimary = _associationDetectionService.DetectAssociationByPrimary(new DetectAssociationByPrimaryFilterModel
                    {
                        CTSCustIDs = model.CTSCustIDs,
                        IsBP = model.IsOptBP,
                        IsDevice = model.IsOptDevice,
                        IsIP = model.IsOptIP
                    });
                    mergeGroupInfo.AssociationByPrimaryJson = JsonConvert.SerializeObject(assByPrimary);
                }
            }, () => {
                var assByAgent = Enumerable.Empty<DetectAssociationByPrimaryModel>();

                if (model.IsOptAgent)
                    assByAgent = _associationDetectionService.DetectAssociationByAgent(model.CTSCustIDs);

                mergeGroupInfo.AssociationByAgentJson = JsonConvert.SerializeObject(assByAgent);
            });

            var data = _associationDetectionService.DetectGroupDetectAssociation(mergeGroupInfo).Select(x => new DetectGroupAssociationItemViewModel(x));

            result = new DetectGroupAssociationListViewModel
            {
                ItemList = data,
                GroupList = data.Select(x => new GroupInfoViewModel { GroupID = x.GroupID, GroupName = x.GroupName })
                                    .Distinct(x => x.GroupID).OrderBy(x => x.GroupID)
            };

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorDetailRpt)]
        public ActionResult VerifyTransMMDetail(MatchMonitorDetailVerifyTransParaModel para)
        {
            var result = new DBResult();

            try
            {
                if (para != null && !string.IsNullOrEmpty(para.TranIds))
                {
                    para.VerifiedBy = CurrentSession.UserId;

                    _matchMonitorService.VerifyTransMatchMonitorDetail(para);
                }
            }
            catch (Exception ex)
            {
                LogService.Info("Verify Trans MM Detail Error: " + ex.Message + ex.StackTrace + "\n"
                      + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                      + ex.InnerException.StackTrace);

                result.ErrorCode = -1;
                result.ErrorMsg = ex.Message;
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetMMFilterBetTypeBySportID(string sportIDs)
        {
            var result = Enumerable.Empty<MatchMonitorBetTypeViewModel>();
            var sportIds = sportIDs.Split(',').Select(x => Convert.ToInt32(x));

            if (sportIds.Any())
            {
                var data = _matchMonitorService.GetMMStaticList();

                if (data.BetTypes != null && data.BetTypes.Any())
                {
                    result = data.BetTypes.Join(sportIds,
                      x1 => x1.SportID,
                      x2 => x2,
                      (x1, x2) => new MatchMonitorBetTypeViewModel
                      {
                          BetTypeID = x1.BetTypeID,
                          BetTypeName = x1.BetTypeName,
                          BetID = x1.BetID,
                          SportID = x1.SportID,
                          IsSelected = true
                      }).GroupBy(g => new { g.BetTypeID, g.BetID })
                      .Select(g => g.First());
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Export to Excel
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSWeb, TrackingCtsFunctionId.MatchMonitorDetailRpt)]
        public ActionResult ExportToExcelMMDetail(FormCollection from)
        {
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
           
            var matchInfo = serializer.Deserialize<ExExlMatchInfoViewModel>(from["match"]);
            var homeMItems = serializer.Deserialize<List<ExExcelTicketInfoViewModel>>(from["homes"]);
            var awayMItems = serializer.Deserialize<List<ExExcelTicketInfoViewModel>>(from["aways"]);           

            var templatePath = Server.MapPath(MMDetailTemplate);

            var fileName = string.Format("MM_{0}_{1}_{2}.xlsx", matchInfo.MatchName, matchInfo.BetTypeName, matchInfo.EventDateStr);

            var excelFile = _matchMonitorService.ExportExcelMMDetail(homeMItems, awayMItems, fileName, templatePath);

            return File(excelFile, "application/vnd.ms-excel", fileName);
        }
        
#if DEBUG
        private MatchMonitorDetailListViewModel Generate_DataTest()
        {
            var mdResult = new MatchMonitorDetailListViewModel();

            mdResult.HomeMatchItemList = this.GenerateMMDetail_DataTest(5);
            mdResult.AwayMatchItemList = this.GenerateMMDetail_DataTest(9);

            return mdResult;
        }

        private IEnumerable<MatchMonitorDetailItemViewModel> GenerateMMDetail_DataTest(int length)
        {
            var list = new List<MatchMonitorDetailItemViewModel>();

            for (int i = 0; i < length; i++)
            {
                list.Add(new MatchMonitorDetailItemViewModel
                {
                    CTSCustID = 1 + i,
                    CustID = 1 + i,
                    SubscriberType = "SubscriberType",
                    Username = "Member10" + i,
                    CustomerClass = 200 + i,
                    Danger1 = 1,
                    //Danger2 = 2,
                    Odds = new decimal(-0.81),
                    BetID = 54026075788804200.ToString(),
                    TransId = 54026075788804200,
                    Currency = "HKK",
                    Amount = new decimal(1200),
                    HDP1 = new decimal(0.75),
                    HDP2 = new decimal(1.25),
                    Score = i.ToString() + "-0",
                    TransTimeToString = DateTime.Now.ToString(Consts.FormatDateToString_ddMMhhmmss),
                    Group = (i + 1).ToString(),
                    GroupID = -1,
                    ReasonList = "HEG,FG",
                    BetTeam = "h"
                });
            }

            return list;
        }
#endif

#endregion

        #region Private Methods

        private bool IsValidDateRange(DateTime? fromDate, DateTime? toDate)
        {
            if (toDate != null && fromDate != null)
            {
                TimeSpan t = (toDate?.Date - fromDate?.Date) ?? TimeSpan.MaxValue;
                return t.TotalDays <= 6;
            }

            return false;
        }
                
        //private MatchMonitorDetailResultModel GetMatchMonitorDetail(MatchMonitorDetailFilterViewModel filter, List<MatchGroupKeyModel> groupKeys)
        //{
        //    if (groupKeys != null && groupKeys.Any())
        //    {
        //        var matchDetails = _matchMonitorService.GetMatchMonitorDetail(filter);

        //        if (matchDetails != null && matchDetails.MatchDetails.Any())
        //        {
        //            var matchs = matchDetails.MatchDetails.Join(groupKeys, m => (m.ScoreDiff)
        //                                                    , r => (r.ScoreDiff)
        //                                                    , (m, r) => m);

        //            return new MatchMonitorDetailResultModel
        //            {
        //                MatchDetails = matchs ?? new List<MatchMonitorDetailModel>(),
        //                VerifiedTrans = matchDetails.VerifiedTrans ?? new List<MatchMonitorDetailVerifyTransResultModel>()
        //            };
        //        }
        //    }

        //    return new MatchMonitorDetailResultModel { MatchDetails = new List<MatchMonitorDetailModel>(), VerifiedTrans = new List<MatchMonitorDetailVerifyTransResultModel>() };
        //}

        private IEnumerable<TransGroupModel> MappingTransInfo(MatchMonitorDetailResultModel matchDetail, IEnumerable<MatchMonitorReasonModel> reasons)
        {
            var transGroupBetting = new List<TransGroupModel>();

            if (matchDetail != null && matchDetail.MatchDetails.Any())
            {
                matchDetail.MatchDetails.ToList().ForEach(i =>
                {
                    var verifyTranInfo = matchDetail.VerifiedTrans?.FirstOrDefault(c => c.VerifiedTransID == i.TransID);
                    var transGroup = new TransGroupModel
                    {
                        TransID = i.TransID,
                        ReasonList = i.ReasonList,
                        VerifiedTransID = verifyTranInfo?.VerifiedTransID.ToString() ?? string.Empty,
                        VerifiedDate = verifyTranInfo?.VerifiedDate.ToString(FormatDateToString_MMMddhhmmss) ?? string.Empty
                    };

                    transGroup.ReasonNameList = transGroup.GetReasonName(reasons, i.ReasonList);

                    transGroupBetting.Add(transGroup);
                });                
            }

            return transGroupBetting;
        }              
        
        private IEnumerable<MatchMonitorDetailItemViewModel> MergeMatchMonitorDetail(IEnumerable<TransGroupModel> mappingTransInfo, IEnumerable<CustInfoModel> custInfo, IEnumerable<TicketDetailModel> ticketDetail)
        {
            var matchDetailResult = new List<MatchMonitorDetailItemViewModel>();

            if (custInfo == null || ticketDetail == null)
            {
                return matchDetailResult;
            }

            matchDetailResult = ticketDetail.Join(custInfo, ticket => ticket.CustId,
                                                            cust => cust.CustID,
                                                            (ticket, cust) => new { ticket, cust })
                                                            .Select(c => {
                                                                var transGroup = mappingTransInfo.FirstOrDefault(x => x.TransID == c.ticket.TransId);
                                                                var verifiedBy = transGroup?.VerifiedTransID;
                                                                
                                                                return new MatchMonitorDetailItemViewModel
                                                                {
                                                                    CTSCustID = c.cust.CTSCustID,
                                                                    CustID = c.cust.CustID,
                                                                    SubscriberType = c.cust.SubscriberType,
                                                                    Username = c.cust.UserName,
                                                                    CustomerClass = c.cust.CustomerClass,
                                                                    PICVerified = transGroup?.VerifiedTransID,
                                                                    Danger1 = c.ticket.Danger1,
                                                                    Danger2 = c.ticket.Danger2,
                                                                    Danger3 = c.ticket.Danger3,
                                                                    Danger4 = c.ticket.Danger4,
                                                                    Danger5 = c.ticket.Danger5,
                                                                    Odds = c.ticket.Odds,
                                                                    BetID = c.ticket.TransId.ToString(),
                                                                    BetTeam = c.ticket.BetTeam,

                                                                    Currency = c.cust.Currency,
                                                                    Amount = c.ticket.Stake,
                                                                    HDP1 = c.ticket.Hdp1,
                                                                    HDP2 = c.ticket.Hdp2,
                                                                    Score = c.ticket.LiveIndicator ? (c.ticket.LiveHomeScore + " - " + c.ticket.LiveAwayScore) : "-",
                                                                    TicketStatus = c.ticket.TicketStatus,
                                                                    TransTime = c.ticket.TransDate,
                                                                    TransTimeToString = c.ticket.TransDate.ToString(Consts.FormatDateToString_ddMMhhmmss),
                                                                    Group = "",
                                                                    GroupID = -1,
                                                                    ReasonList = transGroup?.ReasonNameList,
                                                                    TransId = c.ticket.TransId,
                                                                    RobotType = null,
                                                                    IsRobot = false,
                                                                    IsNormalLinkedPA = c.cust.IsNormalLinkedPA
                                                                };
                                                            })?.OrderByDescending(c => c.TransTime)?.Distinct()?.ToList();

            return matchDetailResult;
        }

        private IEnumerable<MatchMonitorDetailItemViewModel> GetRobotStatus(IEnumerable<MatchMonitorDetailItemViewModel> matchDetailInfo, int betTypeID)
        {
            if (matchDetailInfo != null && matchDetailInfo.Any())
            {
                var para = matchDetailInfo.Select(c => new { TransID = c.TransId, TransDate = c.TransTime, CustID = c.CustID });

                var robotStatus = _matchMonitorService.GetRobotStatus(JsonConvert.SerializeObject(para));

                if (robotStatus != null && robotStatus.Any())
                {
                    var result = matchDetailInfo.GroupJoin(robotStatus,
                               m => m.TransId,
                               r => r.TransID,
                               (m, r) => new { m, r })
                               .SelectMany(
                                    group => group.r.DefaultIfEmpty(),
                                    (group, temp) => new MatchMonitorDetailItemViewModel
                                    {
                                        CTSCustID = group.m.CTSCustID,
                                        CustID = group.m.CustID,                                        
                                        SubscriberType = group.m.SubscriberType,
                                        Username = group.m.Username,
                                        CustomerClass = group.m.CustomerClass,
                                        PICVerified = group.m.PICVerified,
                                        Danger1 = group.m.Danger1,
                                        Danger2 = group.m.Danger2,
                                        Danger3 = group.m.Danger3,
                                        Danger4 = group.m.Danger4,
                                        Danger5 = group.m.Danger5,
                                        Odds = group.m.Odds,
                                        BetID = group.m.TransId.ToString(),
                                        BetTeam = group.m.BetTeam,
                                        BetTypeID = betTypeID,
                                        Currency = group.m.Currency,
                                        Amount = group.m.Amount,
                                        HDP1 = group.m.HDP1,
                                        HDP2 = group.m.HDP2,
                                        Score = group.m.Score,
                                        TicketStatus = group.m.TicketStatus,
                                        TransTime = group.m.TransTime,
                                        TransTimeToString = group.m.TransTimeToString,
                                        Group = group.m.Group,
                                        GroupID = group.m.GroupID,
                                        ReasonList = group.m.ReasonList,
                                        TransId = group.m.TransId,
                                        RobotType = (temp != null) ? temp?.RobotType : null,
                                        IsRobot = (temp != null) && temp.IsRobot,
                                        IsNormalLinkedPA = group.m.IsNormalLinkedPA
                                    })?.OrderByDescending(c => c.TransTime)?.Distinct()?.ToList();

                    return result;
                }
            }

            return matchDetailInfo;
        }
        
        private IEnumerable<MatchMonitorDetailItemViewModel> FilterSite(IEnumerable<MatchMonitorDetailItemViewModel> matchDetails, string site)
        {
            if (matchDetails != null && matchDetails.Any() && site != "A")
            {
                return matchDetails.Where(x => x.SubscriberType == site);
            }

            return matchDetails;
        }
       
        private string GetAssociationCust(IEnumerable<DetectAssociationBySharedIPMatchesModel> detectAss)
        {
            var assCustBySharedIP = detectAss.Select(x => new AssociationCustModel
            {
                FromCustID = x.CustID1,             
                ToCustID = x.CustID2,                        
            });

            return JsonConvert.SerializeObject(assCustBySharedIP);
        }

        #endregion Private Methods

    }
}