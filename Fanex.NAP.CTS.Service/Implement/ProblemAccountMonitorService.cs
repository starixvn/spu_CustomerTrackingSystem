namespace Fanex.NAP.CTS.Service
{
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.Entities;
    using Fanex.BetList.Core.TicketHelper;
    using Fanex.Data;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Criteria.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;
    using Newtonsoft.Json;
    using System;    
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ProblemAccountMonitorService : IProblemAccountMonitorService
    {
        public DateTime GetMoveBetBalanceUpDate()
        {
            var criteria = new MoveBetBalanceUpDateGetCriteria()
            {
                IsGetMoveBetDate = true
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DateTime>(criteria).FirstOrDefault();
            }
        }

        public IEnumerable<SportModel> GetSports()
        {
            var criteria = new SportListGetCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<SportModel>(criteria);
            }
        }

        public IEnumerable<LeagueModel> GetLeagues(DateTime eventDate, string sportIds)
        {
            var criteria = new LeagueListGetCriteria()
            {
                EventDate = eventDate,
                SportIds = sportIds
            };
            
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<LeagueModel>(criteria);
            }
        }

        public IEnumerable<SubscriberGroupModel> GetAllSubscriberGroups()
        {
            var criteria = new GetAllSubscriberGroupsCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<SubscriberGroupModel>(criteria);
            }
        }

        public ProblemAccountMonitorModel GetProblemAccountMonitor(DateTime eventDate, string leagueIds, string matchStatus, string siteIds)
        {
            var criteria = new ProblemAccountMonitorGetCriteria
            {
                EventDate = eventDate,
                LeagueIds = leagueIds,
                SiteIds = siteIds,
                MatchStatus = matchStatus
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var query = db.QueryMultiple(criteria))
                {
                    return new ProblemAccountMonitorModel
                    {
                        SuspiciousTicketInfos = query.Read<SuspiciousTicketInfo>(),
                        MatchInfos = query.Read<MatchInfo>()
                    };
                }
            }
        }

        public int CountTicketFraud(IEnumerable<SuspiciousTicketInfo> suspiciousTicketInfos, int matchId, int fraudCaseId)
        {
            if (suspiciousTicketInfos != null && suspiciousTicketInfos.Any())
            {
                return suspiciousTicketInfos
                    .Where(x => x.MatchId.Equals(matchId) && x.FraudCaseId.Equals(fraudCaseId))
                    .Sum(x => x.SuspiciousTicketsCount);
            }

            return 0;
        }

        public DetailMatchInfoModel GetDetailMatchInfo(int matchId, int fraudCaseId)
        {
            var criteria = new DetailMatchInfoGetCriteria
            {
                MatchId = matchId,
                FraudCaseId = fraudCaseId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DetailMatchInfoModel>(criteria).FirstOrDefault();
            }
        }

        public IEnumerable<DetailTicketsModel> GetDetailTickets(int matchId, int fraudCaseId, string stakeFrom, string stakeTo, string siteIds)
        {
            decimal numStakeFrom = 0;
            decimal numStakeTo = 0;
            decimal.TryParse(stakeFrom, out numStakeFrom);
            decimal.TryParse(stakeTo, out numStakeTo);

            var criteria = new DetailTicketsGetCriteria
            {
                MatchId = matchId,
                FraudCaseId = fraudCaseId,
                SiteIds = siteIds,
                StakeFrom = numStakeFrom,
                StakeTo = numStakeTo
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DetailTicketsModel>(criteria);
            }
        }

        public IEnumerable<DetailCustomerGroupsModel> GetCustomerGroup(List<DetailTicketsModel> detailTicketsModels)
        {
            var custLists = detailTicketsModels.Select(c => new { c.CustID, c.TransID, c.TransDate });

            var jsonData = JsonConvert.SerializeObject(custLists);

            var criteria = new DetailCustomerGroupsGetCriteria
            {
                CustomerTransDateList = jsonData
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DetailCustomerGroupsModel>(criteria);
            }
        }

        public IEnumerable<DetailAssociatedGroupModel> GetAssociatedGroup(IEnumerable<int> custIds, string apiUrl)
        {
            var data = ApiHelper.Post<IEnumerable<DetailAssociatedGroupModel>>(apiUrl, custIds);

            Task.WhenAll(data);

            return data.Result != null
                ? data.Result
                : Enumerable.Empty<DetailAssociatedGroupModel>();
        }

        public IEnumerable<DetailTicketVoidingInRequestModel> GetTicketVoidingInRequest(int matchId, string tranIdList)
        {
            var criteria = new DetailTicketVoidingInRequestGetCriteria
            {
                 MatchId = matchId,
                 TransIds = tranIdList
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DetailTicketVoidingInRequestModel>(criteria);
            }
        }

        public string GetHtmlChoice(DetailTicketsModel ticket)
        {          
            var htmlBuilder = new BetListHTMLBuilder();
            var ticketHelper = new TicketHelper();

            var coreTicket = new Ticket
            {
                BetTypeId = ticket.BettypeId,
                TransId = ticket.TransID,
                TransDate = ticket.TransDate,
                BetTeam = ticket.BetTeam,
                Handicap1 = ticket.Handicap1,
                Handicap2 = ticket.Handicap2,
                LiveHomeScore = ticket.LiveHomeScore,
                LiveAwayScore = ticket.LiveAwayScore,
                Odds = ticket.Odds,
                OddsType = ticket.OddsType.ToString(),
                Stake = ticket.Stake,
                Winlost = ticket.Winlost,
                IP = ticket.IP
            };

            return htmlBuilder.BuildChoice(coreTicket, ticketHelper, null, true);           
        }       
    }
}