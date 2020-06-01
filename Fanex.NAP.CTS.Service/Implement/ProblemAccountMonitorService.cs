namespace Fanex.NAP.CTS.Service
{
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.Entities;
    using Fanex.BetList.Core.TicketHelper;
    using Fanex.Data;
    using Fanex.NAP.CTS.Criteria.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;    
    using System;    
    using System.Collections.Generic;
    using System.Linq;

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

        public IEnumerable<LeagueModel> GetLeagues(DateTime eventDate, int sportId)
        {
            var criteria = new LeagueListGetCriteria()
            {
                EventDate = eventDate,
                SportId = sportId
            };
            
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<LeagueModel>(criteria);
            }
        }

        public ProblemAccountMonitorModel GetProblemAccountMonitor(DateTime eventDate, string leagueIds, string matchStatus)
        {
            var criteria = new ProblemAccountMonitorGetCriteria
            {
                EventDate = eventDate,
                LeagueIds = leagueIds,
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

        public IEnumerable<DetailTicketsModel> GetDetailTickets(int matchId, int fraudCaseId)
        {
            var criteria = new DetailTicketsGetCriteria
            {
                MatchId = matchId,
                FraudCaseId = fraudCaseId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DetailTicketsModel>(criteria);
            }
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
                TransId = ticket.TransId,
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

        public string GetVoidRequestId(IEnumerable<DetailTicketVoidingInRequestModel> ticketList, long tranId)
        {
            var voidTicket = ticketList.FirstOrDefault(x => x.transid.Equals(tranId));

            return voidTicket != null
                ? voidTicket.RequestId.ToString()
                : string.Empty;
        }

    }
}