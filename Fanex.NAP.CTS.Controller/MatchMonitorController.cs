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
                    matchReasonGroup = _matchMonitorService.GetMatchReasonGroup(para.EventDate, para.MatchID, para.BetTypeID)?.OrderBy(c => c.IsHedging).ToList();
                    matchScore = _matchMonitorService.GetMatchScore(para.EventDate, para.MatchID, para.BetTypeID);
                    MappingReasonGroupName(ref matchReasonGroup);

                    ViewBag.EventDateToString = para.EventDateToString;
                    ViewBag.MatchID = para.MatchID;
                    ViewBag.BetTypeID = para.BetTypeID;
                    ViewBag.MatchName = para.MatchName;
                    ViewBag.IsLiveDefault = (para.EventGroup == 2 || para.EventGroup == 4) ? true : false;
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
                    var matchDetail = _matchMonitorService.GetMatchMonitorDetail(para);
                    var matchReasonGroup = _matchMonitorService.GetMatchReasonGroup(para.EventDate, para.MatchID, para.BetTypeID)?.OrderBy(c => c.IsHedging).ToList();
                    MappingReasonGroupName(ref matchReasonGroup);


                    if (matchDetail != null && matchDetail.Any())
                    {
                        var custInfo = new List<CustInfoModel>();
                        var transGroupBetting = new List<TransGroupBettingModel>();
                        var listCustReasonGroup = new List<CustReasonGroupModel>();
                        var ticketDetail = new List<TicketDetailModel>();

                        var tasks = new List<Task>
                        {
                            Task.Run(() =>
                            {
                                transGroupBetting = GetTransGroupBetting(matchDetail);
                                listCustReasonGroup = GetCustReasonGroup(matchReasonGroup, para.ListReasonGroup);
                            }),
                            Task.Run(() =>
                            {
                                custInfo = _matchMonitorService.GetCustInfoByCustID(string.Join(",", string.Join(",", matchDetail.Select(c => c.ListCustID)).Split(',').Distinct()));
                            }),
                            Task.Run(() =>
                            {
                                ticketDetail = _matchMonitorService.GetTicketDetail(para.EventDate, string.Join(",", string.Join(", ", matchDetail.Select(c => c.ListTransID)).Split(',').Distinct()));
                            })
                        };

                        Task.WhenAll(tasks.ToArray()).Wait();

                        matchDetailResult = MergeMatchMonitorDetail(custInfo, listCustReasonGroup, ticketDetail, transGroupBetting);
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

        private List<MatchMonitorDetailResultViewModel> MergeMatchMonitorDetail(List<CustInfoModel> custInfo, List<CustReasonGroupModel> listCustReasonGroup, List<TicketDetailModel> ticketDetail, List<TransGroupBettingModel> transGroupBetting)
        {
            var matchDetailResult = new List<MatchMonitorDetailResultViewModel>();

            if (custInfo == null || listCustReasonGroup == null || ticketDetail == null)
            {
                return matchDetailResult;
            }

            matchDetailResult = custInfo.Join(ticketDetail, cust => cust.CustID,
                                                            ticket => ticket.CustId,
                                                            (cust, ticket) => new { cust, ticket })
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
                                                                ReasonGroup = GetReasonGroup(transGroupBetting, listCustReasonGroup, c.ticket)
                                                            }).ToList();

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

        private List<CustReasonGroupModel> GetCustReasonGroup(List<MatchReasonGroupModel> matchReasonGroup, string listReasonGroup)
        {
            var custReasonGroup = new List<CustReasonGroupModel>();

            if (matchReasonGroup != null && !string.IsNullOrEmpty(listReasonGroup))
            {
                var reasonGroupFilter = listReasonGroup.Split(',').Select(Int64.Parse).ToList();
                reasonGroupFilter.Join(matchReasonGroup,
                    filter => filter,
                    reasonGroup => reasonGroup.GroupID, (filter, reasonGroup) => reasonGroup).Select(e => e).ToList().ForEach(e =>
                    {
                        foreach (var i in e.ListCustID.Split(',').Select(Int64.Parse).Distinct())
                        {
                            custReasonGroup.Add(new CustReasonGroupModel
                            {
                                GroupID = e.GroupID,
                                CustID = i,
                                ReasonGroupName = e.ReasonGroupName
                            });
                        }
                    });
            }

            return custReasonGroup;
        }

        private List<TransGroupBettingModel> GetTransGroupBetting(List<MatchMonitorDetailModel> matchDetail)
        {
            var transGroupBetting = new List<TransGroupBettingModel>();

            matchDetail.ForEach(i =>
            {
                i.ListTransID.Split(',').Select(Int64.Parse).ToList().ForEach(j =>
                {
                    transGroupBetting.Add(new TransGroupBettingModel
                    {
                        TransId = j,
                        GroupID = i.GroupID
                    });
                });
            });

            return transGroupBetting;
        }

        private string GetReasonGroup(List<TransGroupBettingModel> transGroupBetting, List<CustReasonGroupModel> custReasonGroup, TicketDetailModel ticketDetail)
        {
            string reasonGroup = "-";

            if (transGroupBetting != null && transGroupBetting.Any() && custReasonGroup != null && custReasonGroup.Any())
            {
                var reasonGroupForTicket = transGroupBetting.SingleOrDefault(c => c.TransId == ticketDetail.TransId);

                reasonGroup = custReasonGroup?.SingleOrDefault(c => c.CustID == ticketDetail.CustId && c.GroupID == reasonGroupForTicket.GroupID).ReasonGroupName;
            }

            return reasonGroup;
        }
    }
}