namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Criteria.ProblemMatchMonitor;
    using Fanex.NAP.CTS.Model.ProblemMatchMonitor;    
    using System;    
    using System.Collections.Generic;
    using System.Linq;

    public class ProblemMatchMonitorService : IProblemMatchMonitorService
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

        public ProblemMatchMonitorModel GetProblemMatchMonitor(DateTime eventDate, string leagueIds, string matchStatus)
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
                    return new ProblemMatchMonitorModel
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
            var criteria = new DetailMatchInfoGetCriteria()
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
            var criteria = new DetailTicketsGetCriteria()
            {
                MatchId = matchId,
                FraudCaseId = fraudCaseId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<DetailTicketsModel>(criteria);
            }
        }
    }
}