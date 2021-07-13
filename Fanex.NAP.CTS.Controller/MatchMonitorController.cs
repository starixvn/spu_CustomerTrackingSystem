namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Logging;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core;
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

            return View(new MatchMonitorViewModel
            {
                IsHostPermission = isHostPermission,
                CustId = userId
            });
        }

        [HttpPost]
        public ActionResult GetMatchMonitor(MatchMonitorFilterViewModel matchMonitorFilterViewModel)
        {
            var matchMonitors = new List<MatchMonitorModel>();

            try
            {
                if (matchMonitorFilterViewModel != null && !string.IsNullOrEmpty(matchMonitorFilterViewModel.BetTypes))
                {
                    matchMonitors = _matchMonitorService.GetMatchMonitor(matchMonitorFilterViewModel)?.OrderBy(c => c.EventGroup).ToList();
                }
            }
            catch (Exception e)
            {
                LogService.Error("Match Monitor Error", e);
            }

            return Json(new
            {
                data = matchMonitors
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateVerification(Int64 matchID)
        {
            var result = new DBResult();

            try
            {
                result = _matchMonitorService.UpdateVerificaton(matchID, CurrentSession.UserId);
            }
            catch (Exception e)
            {
                LogService.Error("Match Monitor Error", e);
                result.ErrorCode = -1;
                result.ErrorMsg = e.Message;
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Detail(MatchMonitorDetailViewModel para)
        {
            IUserService userService = new UserService();
            var matchReasonGroup = new List<MatchReasonGroupModel>();
            var matchScore = new List<MatchScoreModel>();

            try
            {
                if (para != null)
                {
                    matchReasonGroup = _matchMonitorService.GetMatchReasonGroup(para.EventDate, para.MatchID, para.BetTypeID)?.OrderBy(c => c.IsHedging)?.ThenBy(c => c.GroupID)?.ToList();

                    matchScore = _matchMonitorService.GetMatchScore(para.EventDate, para.MatchID, para.BetTypeID);
                    MappingReasonGroupName(ref matchReasonGroup);

                    ViewBag.EventDateToString = para.EventDateToString;
                    ViewBag.MatchID = para.MatchID;
                    ViewBag.BetTypeID = para.BetTypeID;
                    ViewBag.MatchName = para.MatchName;
                    ViewBag.IsLiveDefault = (para.EventGroup == 2 || para.EventGroup == 4);
                    ViewBag.ReasonGroup = matchReasonGroup;
                    ViewBag.MatchScore = matchScore;
                    ViewBag.IsHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                                    NapSettings.Settings.SiteId,
                                    FSFunctionKeys.MatchMonitor,
                                    NAP.Core.Business.Constant.Policy.Host);
                }
            }
            catch (Exception e)
            {
                LogService.Error("Match Monitor Error", e);
            }

            return View();
        }

        [HttpPost]
        public ActionResult GetMatchMonitorDetail(MatchMonitorDetailFilterViewModel para)
        {
            var matchDetailResult = new List<MatchMonitorDetailResultViewModel>();

            try
            {
                if (para != null && !string.IsNullOrEmpty(para.ListReasonGroup) && !string.IsNullOrEmpty(para.ListScore))
                {
                    var reasonGroupKeysFilter = JsonConvert.DeserializeObject<List<MatchReasonGroupKeyModel>>(para.ListReasonGroup);

                    var matchReasonGroups = _matchMonitorService.GetMatchReasonGroup(para.EventDate, para.MatchID, para.BetTypeID)?.OrderBy(c => c.IsHedging)?.ThenBy(c => c.GroupID)?.ToList();

                    MappingReasonGroupName(ref matchReasonGroups);

                    var matchDetail = GetMatchMonitorDetail(para, reasonGroupKeysFilter);

                    if (matchDetail != null && matchDetail.Any())
                    {
                        var custInfo = new List<CustInfoModel>();
                        var mappingTransGroupReason = new List<TransGroupBettingModel>();
                        var mappingCustGroupReason = new List<CustReasonGroupModel>();
                        var ticketDetail = new List<TicketDetailModel>();

                        var tasks = new List<Task>
                        {
                            Task.Run(() =>
                            {
                                mappingTransGroupReason = MappingTransGroupReason(matchDetail);
                                mappingCustGroupReason = MappingCustGroupReason(matchReasonGroups, reasonGroupKeysFilter);
                            }),
                            Task.Run(() =>
                            {
                                var custIdsOfEachMatch =  matchDetail.Select(c => c.ListCustID).Distinct();
                                var custIdsOfTotalMatch = string.Join(",",custIdsOfEachMatch).Split(',').Distinct();
                                custInfo = _matchMonitorService.GetCustInfoByCustID(string.Join(",", custIdsOfTotalMatch));
                            }),
                            Task.Run(() =>
                            {
                                var tranIdsOfEachMatch =  matchDetail.Select(c => c.ListTransID).Distinct();
                                var tranIdsOfTotalMatch = string.Join(",",tranIdsOfEachMatch).Split(',').Distinct();
                                ticketDetail = _matchMonitorService.GetTicketDetail(para.EventDate, string.Join(",",tranIdsOfTotalMatch));
                            })
                        };

                        Task.WhenAll(tasks.ToArray()).Wait();

                        matchDetailResult = MergeMatchMonitorDetail(mappingTransGroupReason, mappingCustGroupReason, custInfo, ticketDetail);
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Error("Match Monitor Error", e);
            }

            return Json(new
            {
                homeTableData = matchDetailResult?.Where(c => c.IsHomeSide),
                awayTableData = matchDetailResult?.Where(c => !c.IsHomeSide)
            }, JsonRequestBehavior.AllowGet);
        }

        private List<MatchMonitorDetailResultViewModel> MergeMatchMonitorDetail(List<TransGroupBettingModel> mappingTransGroupReason, List<CustReasonGroupModel> mappingCustGroupReason, List<CustInfoModel> custInfo, List<TicketDetailModel> ticketDetail)
        {
            var matchDetailResult = new List<MatchMonitorDetailResultViewModel>();

            if (custInfo == null || mappingCustGroupReason == null || ticketDetail == null)
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
                                                                Danger1 = c.cust.Danger1,
                                                                Danger2 = c.cust.Danger2,
                                                                Danger3 = c.cust.Danger3,
                                                                Odds = c.ticket.Odds,
                                                                BetID = c.ticket.TransId,
                                                                BetTeam = c.ticket.BetTeam,
                                                                Currency = c.cust.Currency,
                                                                LiveIndicator = c.ticket.LiveIndicator,
                                                                Amount = c.ticket.Stake,
                                                                HDP = c.ticket.Hdp1 - c.ticket.Hdp2,
                                                                Score = c.ticket.LiveIndicator ? (c.ticket.LiveAwayScore + " - " + c.ticket.LiveHomeScore) : "-",
                                                                TicketStatus = c.ticket.TicketStatus,
                                                                TransTime = c.ticket.TransDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                                                                ReasonGroup = GetReasonGroup(mappingTransGroupReason, mappingCustGroupReason, c.ticket)
                                                            })?.OrderBy(c => c.Username)?.ThenBy(c => c.ReasonGroup)?.Distinct()?.ToList();

            return matchDetailResult;
        }

        private void MappingReasonGroupName(ref List<MatchReasonGroupModel> matchReasonGroup)
        {
            int preIncrement1 = 0;
            int preIncrement2 = 0;

            if (matchReasonGroup != null && matchReasonGroup.Any())
            {
                foreach (var item in matchReasonGroup)
                {
                    item.ReasonGroupName = item.IsHedging ? ("Hedging " + ++preIncrement1) : ("Group Betting " + ++preIncrement2);
                }
            }
        }

        private List<MatchMonitorDetailModel> GetMatchMonitorDetail(MatchMonitorDetailFilterViewModel filter, List<MatchReasonGroupKeyModel> reasonGroupKeys)
        {
            if (reasonGroupKeys != null && reasonGroupKeys.Any())
            {
                var para = filter;
                para.ListReasonGroup = string.Join(",", reasonGroupKeys.Select(c => c.GroupID)?.Distinct());

                var matchDetails = _matchMonitorService.GetMatchMonitorDetail(para);

                if (matchDetails != null && matchDetails.Any())
                {
                    var matchs = matchDetails.Join(reasonGroupKeys, m => (m.GroupID, m.TotalScore)
                                                                  , r => (r.GroupID, r.TotalScore)
                                                                  , (m, r) => m);

                    return matchs.ToList() ?? new List<MatchMonitorDetailModel>();
                }
            }

            return new List<MatchMonitorDetailModel>();
        }

        private List<CustReasonGroupModel> MappingCustGroupReason(List<MatchReasonGroupModel> matchReasonGroups, List<MatchReasonGroupKeyModel> reasonGroupKeysFilter)
        {
            var custReasonGroup = new List<CustReasonGroupModel>();

            if (matchReasonGroups == null && reasonGroupKeysFilter == null)
            {
                return custReasonGroup;
            }

            var reasonGroupFilter = reasonGroupKeysFilter.Join(matchReasonGroups, filter => (filter.GroupID, filter.TotalScore),
                                                                             reasonGroup => (reasonGroup.GroupID, reasonGroup.TotalScore), (filter, reasonGroup) => reasonGroup);

            if (reasonGroupFilter != null && reasonGroupFilter.Any())
            {
                reasonGroupFilter.ToList().ForEach(i =>
                 {
                     var custIds = i.ListCustID.Split(',').Select(Int64.Parse)?.Distinct();

                     if (custIds != null)
                     {
                         foreach (var j in i.ListCustID.Split(',').Select(Int64.Parse).Distinct())
                         {
                             custReasonGroup.Add(new CustReasonGroupModel
                             {
                                 GroupID = i.GroupID,
                                 CustID = j,
                                 ReasonGroupName = i.ReasonGroupName,
                                 TotalScore = i.TotalScore
                             });
                         }
                     }
                 });
            }

            return custReasonGroup;
        }

        private List<TransGroupBettingModel> MappingTransGroupReason(List<MatchMonitorDetailModel> matchDetail)
        {
            var transGroupBetting = new List<TransGroupBettingModel>();

            if (matchDetail != null && matchDetail.Any())
            {
                matchDetail.ForEach(i =>
                {
                    i.ListTransID.Split(',').Select(Int64.Parse)?.ToList()?.ForEach(j =>
                    {
                        transGroupBetting.Add(new TransGroupBettingModel
                        {
                            TransId = j,
                            GroupID = i.GroupID,
                            TotalScore = i.TotalScore
                        });
                    });
                });
            }

            return transGroupBetting;
        }

        private string GetReasonGroup(List<TransGroupBettingModel> mappingTransGroupReason, List<CustReasonGroupModel> mappingCustGroupReason, TicketDetailModel ticketDetail)
        {
            string reasonGroup = "-";

            if (mappingTransGroupReason != null && mappingTransGroupReason.Any() && mappingCustGroupReason != null && mappingCustGroupReason.Any())
            {
                var transGroup = mappingTransGroupReason.Where(c => c.TransId == ticketDetail.TransId);
                var custGroupReason = mappingCustGroupReason.Where(c => c.CustID == ticketDetail.CustId);
                var reasonGroupName = custGroupReason.Join(transGroup, c => (c.GroupID, c.TotalScore), 
                                                                       t => (t.GroupID, t.TotalScore), (c, t) => c).FirstOrDefault().ReasonGroupName;

                return string.IsNullOrEmpty(reasonGroupName) ? reasonGroup : reasonGroupName;
            }

            return reasonGroup;
        }
    }
}