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

            return View(new MatchMonitorViewModel
            {
                IsHostPermission = isHostPermission,
                CustId = userId
            });
        }

        [HttpPost]
        public ActionResult GetMatchMonitor(MatchMonitorFilterViewModel para)
        {
            var matchMonitors = new List<MatchMonitorModel>();

            try
            {
                if (para != null && !string.IsNullOrEmpty(para.BetTypes) && !string.IsNullOrEmpty(para.MatchSection) && !string.IsNullOrEmpty(para.Sports))
                {
                    matchMonitors = _matchMonitorService.GetMatchMonitor(para)?.OrderBy(c => c.MatchSection).ToList();
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
                    matchGroup = _matchMonitorService.GetMatchGroup(para.MatchID, para.BetTypeID, para.LiveIndicator, para.IsHedging)?.ToList() ?? new List<MatchGroupModel>();

                    matchScore = _matchMonitorService.GetMatchScore(para.MatchID, para.BetTypeID, para.LiveIndicator, para.IsHedging) ?? new List<MatchScoreModel>();

                    MappingReasonGroupName(ref matchGroup);
                }
            }
            catch (Exception e)
            {
                LogService.Error("Match Monitor Error", e);
            }

            ViewBag.EventDateToString = para?.EventDateToString;
            ViewBag.MatchID = para?.MatchID;
            ViewBag.BetTypeID = para?.BetTypeID;
            ViewBag.MatchName = para?.MatchName ?? string.Empty;
            ViewBag.LiveIndicator = para?.LiveIndicator ?? true;
            ViewBag.IsHedging = para?.IsHedging ?? true;
            ViewBag.Group = matchGroup;
            ViewBag.MatchScore = matchScore?.OrderBy(c => c.TotalScore).ThenBy(c => c.LiveHomeScore).ThenBy(c => c.LiveAwayScore);
            ViewBag.LeagueName = para?.LeagueName ?? string.Empty;
            ViewBag.ReasonName = para?.ReasonName ?? string.Empty;
            ViewBag.MarketName = para?.MarketName ?? string.Empty;
            ViewBag.IsHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                                                                        NapSettings.Settings.SiteId,
                                                                        FSFunctionKeys.MatchMonitor,
                                                                        NAP.Core.Business.Constant.Policy.Host);

            return View();
        }

        [HttpPost]
        public ActionResult GetMatchMonitorDetail(MatchMonitorDetailFilterViewModel para)
        {
            var matchDetailResult = Enumerable.Empty<MatchMonitorDetailResultViewModel>();

            try
            {
                if (para != null && !string.IsNullOrEmpty(para.ListGroup) && !string.IsNullOrEmpty(para.ListScore))
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
                            })
                        };

                        Task.WhenAll(tasks.ToArray()).Wait();

                        var mergeInfo = MergeMatchMonitorDetail(mappingTransInfo, custInfo, ticketDetail);

                        matchDetailResult = GetRobotStatus(mergeInfo);

                        if (para.Sites != "A")
                        {
                            matchDetailResult = matchDetailResult?.Where(x => x.SubscriberType == para.Sites);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.Error("Match Monitor Error", e);
            }

            return Json(new
            {
                homeTableData = matchDetailResult?.Where(c => c.IsHomeSide) ?? new List<MatchMonitorDetailResultViewModel>(),
                awayTableData = matchDetailResult?.Where(c => !c.IsHomeSide) ?? new List<MatchMonitorDetailResultViewModel>()
            }, JsonRequestBehavior.AllowGet);
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
                                                                IsRobot = false
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
                    var matchs = matchDetails.MatchDetails.Join(groupKeys, m => (m.GroupID, m.TotalScore)
                                                            , r => (r.GroupID, r.TotalScore)
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
                            GroupName = groupKey.FirstOrDefault(x => x.GroupID == i.GroupID && x.TotalScore == i.TotalScore).GroupName,
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

        private IEnumerable<MatchMonitorDetailResultViewModel> GetRobotStatus(IEnumerable<MatchMonitorDetailResultViewModel> matchDetailInfo)
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
                                        PICVerified = group.m.PICVerified,
                                        Danger1 = group.m.Danger1,
                                        Danger2 = group.m.Danger2,
                                        Danger3 = group.m.Danger3,
                                        Danger4 = group.m.Danger4,
                                        Danger5 = group.m.Danger5,
                                        Odds = group.m.Odds,
                                        BetID = group.m.TransId,
                                        BetTeam = group.m.BetTeam,
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
    }
}