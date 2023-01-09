namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitor, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class MatchMonitorController : BaseController
    {
        private readonly IMatchMonitorService _matchMonitorService;

        public MatchMonitorController()
        {
            _matchMonitorService = new MatchMonitorService();
        }

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
                    var data = _matchMonitorService.GetMatchMonitor(para);

                    matchMonitors = data?.Select(x => new GetMatchMonitorViewModel(x)).OrderBy(c => c.MatchSection);

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

                    if (para.Reason != (int)MatchMonitorReason.FixedGame)
                    {
                        MappingReasonGroupName(ref matchGroup);
                    }
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
            ViewBag.BetIDToString = para?.BetID.ToString();
            ViewBag.MatchName = para?.MatchName ?? string.Empty;
            ViewBag.LiveIndicator = para?.LiveIndicator ?? true;
            ViewBag.Reason = para?.Reason;
            ViewBag.Group = matchGroup;
            ViewBag.MatchScore = matchScore?.OrderBy(c => c.ScoreDiff).ThenBy(c => c.LiveHomeScore).ThenBy(c => c.LiveAwayScore);
            ViewBag.LeagueName = para?.LeagueName ?? string.Empty;
            ViewBag.ReasonGroup = para?.ReasonGroup ?? string.Empty;
            ViewBag.MarketName = para?.MarketName ?? string.Empty;
            ViewBag.BetChoice = para?.BetChoice;
            ViewBag.IsDisableScoreFilter = para?.SportType != null && para.SportType == (int)MatchMonitorSportType.Basketball;
            ViewBag.IsDisableGroup = para?.Reason != null && para.Reason == (int)MatchMonitorReason.FixedGame;
            ViewBag.SportName = para?.SportName ?? string.Empty;
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
            var matchDetailResult = Enumerable.Empty<MatchMonitorDetailResultViewModel>();
            var custIDList = string.Empty;

            try
            {
                if (para != null && !string.IsNullOrEmpty(para.ListGroup))
                {
                    var groupKeysFilter = JsonConvert.DeserializeObject<List<MatchGroupKeyModel>>(para.ListGroup);

                    var matchDetailInfo = GetMatchMonitorDetail(para, groupKeysFilter);

                    if (matchDetailInfo != null && matchDetailInfo.MatchDetails.Any())
                    {
                        var custInfo = Enumerable.Empty<CustInfoModel>();
                        var mappingTransInfo = Enumerable.Empty<TransGroupModel>();
                        var ticketDetail = Enumerable.Empty<TicketDetailModel>();

                        var tasks = new List<Task>
                        {
                            Task.Run(() =>
                            {
                                mappingTransInfo = MappingTransInfo(matchDetailInfo, groupKeysFilter);
                            }),
                            Task.Run(() =>
                            {
                                var custIdsOfEachMatch =  matchDetailInfo.MatchDetails.Select(c => c.ListCustID).Distinct();
                                var custIdsOfTotalMatch = string.Join(",",custIdsOfEachMatch).Split(',').Distinct();
                                custInfo = _matchMonitorService.GetCustInfoByCustID(string.Join(",", custIdsOfTotalMatch));
                               
                            }),
                            Task.Run(() =>
                            {
                                var tranIdsOfEachMatch =  matchDetailInfo.MatchDetails.Select(c => c.ListTransID).Distinct();
                                var tranIdsOfTotalMatch = string.Join(",",tranIdsOfEachMatch).Split(',').Distinct();
                                ticketDetail = _matchMonitorService.GetTicketDetail(para.EventDate, string.Join(",",tranIdsOfTotalMatch));
                                ticketDetail = FilterBetChoice(ticketDetail, para.BetChoice);
                            })
                        };

                        Task.WhenAll(tasks.ToArray()).Wait();

                        var mergeInfo = MergeMatchMonitorDetail(mappingTransInfo, custInfo, ticketDetail);                        

                        matchDetailResult = GetRobotStatus(mergeInfo, para.BetTypeID);

                        matchDetailResult = FilterSite(matchDetailResult, para.Sites);                    

                        custIDList = string.Join(",", matchDetailResult.Select(x => x.CustID).Distinct());
                    }
                }
            }
            catch (Exception ex)
            {               
                LogService.Info("Get Match Monitor Detail Error: " + ex.Message + ex.StackTrace + "\n"
                      + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                      + ex.InnerException.StackTrace);
            }

            return Json(new {
                custIDList = custIDList,
                data = matchDetailResult
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAssociationIrrigation(string custIDList, string categoryIDList)
        {
            var result = Enumerable.Empty<AssociationIrrigationViewModel>();

            var data = _matchMonitorService.GetAssociationIrrigation(custIDList, categoryIDList);

            if (data != null && data.Any())
            {
                result = data.Select(x => new AssociationIrrigationViewModel
                {
                    CustID = x.CustID,
                    TotalAssociation = x.Total
                });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
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

        private IEnumerable<MatchMonitorDetailResultViewModel> MergeMatchMonitorDetail(IEnumerable<TransGroupModel> mappingTransInfo, IEnumerable<CustInfoModel> custInfo, IEnumerable<TicketDetailModel> ticketDetail)
        {
            var matchDetailResult = new List<MatchMonitorDetailResultViewModel>();

            if (custInfo == null || ticketDetail == null)
            {
                return matchDetailResult;
            }

            matchDetailResult = ticketDetail.Join(custInfo, ticket => ticket.CustId,
                                                            cust => cust.CustID,
                                                            (ticket, cust) => new { ticket, cust })
                                                            .Select(c => new MatchMonitorDetailResultViewModel
                                                            {
                                                                CTSCustID = c.cust.CTSCustID,
                                                                CustID = c.cust.CustID,
                                                                SubscriberType = c.cust.SubscriberType,
                                                                Username = c.cust.UserName,
                                                                CustomerClass = c.cust.CustomerClass,
                                                                PICVerified = GetVerifiedTrans(mappingTransInfo, c.ticket),
                                                                Danger1 = c.ticket.Danger1,
                                                                Danger2 = c.ticket.Danger2,
                                                                Danger3 = c.ticket.Danger3,
                                                                Danger4 = c.ticket.Danger4,
                                                                Danger5 = c.ticket.Danger5,
                                                                Odds = c.ticket.Odds,
                                                                BetID = c.ticket.TransId,
                                                                BetTeam = c.ticket.BetTeam,

                                                                Currency = c.cust.Currency,
                                                                Amount = c.ticket.Stake,
                                                                HDP1 = c.ticket.Hdp1,
                                                                HDP2 = c.ticket.Hdp2,
                                                                Score = c.ticket.LiveIndicator ? (c.ticket.LiveHomeScore + " - " + c.ticket.LiveAwayScore) : "-",
                                                                TicketStatus = c.ticket.TicketStatus,
                                                                TransTime = c.ticket.TransDate,
                                                                TransTimeToString = c.ticket.TransDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                                                                Group = GetGroup(mappingTransInfo, c.ticket),
                                                                TransId = c.ticket.TransId,
                                                                RobotType = null,
                                                                IsRobot = false,
                                                                AssociationIrrigation = -1
                                                            })?.OrderByDescending(c => c.TransTime)?.ThenBy(c => c.Group)?.Distinct()?.ToList();

            return matchDetailResult;
        }

        private MatchMonitorDetailResultModel GetMatchMonitorDetail(MatchMonitorDetailFilterViewModel filter, List<MatchGroupKeyModel> groupKeys)
        {
            if (groupKeys != null && groupKeys.Any())
            {
                var para = filter;

                para.ListGroup = string.Join(",", groupKeys.Select(c => c.GroupID)?.Distinct());

                var matchDetails = _matchMonitorService.GetMatchMonitorDetail(para);

                if (matchDetails != null && matchDetails.MatchDetails.Any())
                {
                    var matchs = matchDetails.MatchDetails.Join(groupKeys, m => (m.GroupID, m.ScoreDiff)
                                                            , r => (r.GroupID, r.ScoreDiff)
                                                            , (m, r) => m);

                    return new MatchMonitorDetailResultModel
                    {
                        MatchDetails = matchs ?? new List<MatchMonitorDetailModel>(),
                        VerifiedTrans = matchDetails.VerifiedTrans ?? new List<MatchMonitorDetailVerifyTransResultModel>()
                    };
                }
            }

            return new MatchMonitorDetailResultModel { MatchDetails = new List<MatchMonitorDetailModel>(), VerifiedTrans = new List<MatchMonitorDetailVerifyTransResultModel>() };
        }

        private IEnumerable<TransGroupModel> MappingTransInfo(MatchMonitorDetailResultModel matchDetail, List<MatchGroupKeyModel> groupKey)
        {
            var transGroupBetting = new List<TransGroupModel>();

            if (matchDetail != null && matchDetail.MatchDetails.Any())
            {
                matchDetail.MatchDetails.ToList().ForEach(i =>
                {
                    i.ListTransID.Split(',').Select(Int64.Parse)?.ToList()?.ForEach(tranID =>
                    {
                        var verifyTranInfo = matchDetail.VerifiedTrans?.FirstOrDefault(c => c.TransID == tranID);

                        transGroupBetting.Add(new TransGroupModel
                        {
                            TransId = tranID,
                            GroupName = groupKey.FirstOrDefault(x => x.GroupID == i.GroupID && x.ScoreDiff == i.ScoreDiff).GroupName,
                            VerifiedBy = verifyTranInfo?.VerifiedBy ?? string.Empty,
                            VerifiedDate = verifyTranInfo?.VerifiedDate.ToString(FormatDateToString_MMMddhhmmss) ?? string.Empty
                        });
                    });
                });
            }

            return transGroupBetting;
        }

        private string GetGroup(IEnumerable<TransGroupModel> mappingTransInfo, TicketDetailModel ticketDetail)
        {
            string reasonGroup = "-";

            if (mappingTransInfo != null && mappingTransInfo.Any())
            {
                var transGroup = mappingTransInfo.Where(c => c.TransId == ticketDetail.TransId);

                var groupName = transGroup?.FirstOrDefault()?.GroupName.ToString();

                return string.IsNullOrEmpty(groupName) ? reasonGroup : groupName;
            }

            return reasonGroup;
        }

        private string GetVerifiedTrans(IEnumerable<TransGroupModel> mappingTransInfo, TicketDetailModel ticketDetail)
        {
            string verifiedTran = "-";

            if (mappingTransInfo != null && mappingTransInfo.Any())
            {
                var transGroup = mappingTransInfo.Where(c => c.TransId == ticketDetail.TransId);

                var verifiedBy = transGroup?.FirstOrDefault()?.VerifiedBy;

                var verifiedDate = transGroup?.FirstOrDefault()?.VerifiedDate;

                return string.IsNullOrEmpty(verifiedBy) && string.IsNullOrEmpty(verifiedDate) ? verifiedTran : string.Concat(verifiedBy, ", ", verifiedDate);
            }

            return verifiedTran;
        }

        private void MappingReasonGroupName(ref IEnumerable<MatchGroupModel> matchReasonGroup)
        {
            int preIncrement = 1;

            if (matchReasonGroup != null && matchReasonGroup.Any())
            {
                foreach (var item in matchReasonGroup)
                {
                    item.GroupName = preIncrement++;
                }
            }
        }

        private IEnumerable<MatchMonitorDetailResultViewModel> GetRobotStatus(IEnumerable<MatchMonitorDetailResultViewModel> matchDetailInfo, int betTypeID)
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
                                    (group, temp) => new MatchMonitorDetailResultViewModel
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
                                        BetID = group.m.TransId,
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
                                        TransId = group.m.TransId,
                                        RobotType = (temp != null) ? temp?.RobotType : null,
                                        IsRobot = (temp != null) && temp.IsRobot,
                                    })?.OrderByDescending(c => c.TransTime)?.ThenBy(c => c.Group)?.Distinct()?.ToList();

                    return result;
                }
            }

            return matchDetailInfo;
        }

        private IEnumerable<TicketDetailModel> FilterBetChoice(IEnumerable<TicketDetailModel> ticketDetails, string betChoice)
        {
            if (ticketDetails != null && ticketDetails.Any())
            {
                var result = Enumerable.Empty<TicketDetailModel>();

                switch (betChoice)
                {
                    case nameof(MatchMonitorBetChoiceId.All):
                        return ticketDetails;

                    case nameof(MatchMonitorBetChoiceId.A):
                    case nameof(MatchMonitorBetChoiceId.U):
                        return ticketDetails.Where(c => string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.A), StringComparison.OrdinalIgnoreCase)) ?? result;

                    case nameof(MatchMonitorBetChoiceId.H):
                    case nameof(MatchMonitorBetChoiceId.O):
                        return ticketDetails.Where(c => string.Equals(c.BetTeam, nameof(MatchMonitorBetChoiceId.H), StringComparison.OrdinalIgnoreCase)) ?? result;
                }
            }

            return Enumerable.Empty<TicketDetailModel>();
        }

        private IEnumerable<MatchMonitorDetailResultViewModel> FilterSite(IEnumerable<MatchMonitorDetailResultViewModel> matchDetails, string site)
        {
            if (matchDetails != null && matchDetails.Any() && site != "A")
            {
                return matchDetails.Where(x => x.SubscriberType == site);
            }

            return matchDetails;
        }

        #endregion Private Methods
    }
}