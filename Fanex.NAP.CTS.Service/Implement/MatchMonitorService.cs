namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Criteria.MatchMonitor;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MatchMonitorService : IMatchMonitorService
    {
        public List<MatchMonitorModel> GetMatchMonitor(MatchMonitorFilterViewModel filter)
        {
            var criteria = new MatchMonitorGetCriteria()
            {
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                LeagueName = string.IsNullOrWhiteSpace(filter.LeagueName) ? null : filter.LeagueName,
                Market = filter.Market,
                MatchSection = filter.MatchSection,
                BetType = filter.BetTypes,
                IsVerified = filter.IsVerified,
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MatchMonitorModel>(criteria).ToList();
            }
        }

        public DBResult UpdateVerificaton(Int64 matchID, Int64 userID)
        {
            var criteria = new MatchMonitorVerificationUpdateCriteria()
            {
                ip_ID = matchID,
                ip_UserID = userID
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.Query<DBResult>(criteria).ToList();
            }

            return new DBResult { ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? -1 : 0, ErrorMsg = criteria.ErrorMessage };
        }

        public List<MatchGroupModel> GetMatchGroup(Int64 matchID, int betTypeID, bool liveIndicator, bool isHedging)
        {
            var criteria = new MatchGroupGetCriteria()
            {
                BettypeID = betTypeID,
                MatchID = matchID,
                LiveIndicator = liveIndicator,
                IsHedging = isHedging
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MatchGroupModel>(criteria).ToList();
            }
        }

        public List<MatchScoreModel> GetMatchScore(Int64 matchID, int betTypeID, bool liveIndicator, bool isHedging)
        {
            var criteria = new MatchScoreGetCriteria()
            {
                BettypeID = betTypeID,
                MatchID = matchID,
                LiveIndicator = liveIndicator,
                IsHedging = isHedging
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MatchScoreModel>(criteria).ToList();
            }
        }

        public List<MatchMonitorDetailModel> GetMatchMonitorDetail(MatchMonitorDetailFilterViewModel filter)
        {
            var criteria = new MatchDetailMonitorGetCriteria()
            {
                BettypeID = filter.BetTypeID,
                ListGroup = filter.ListGroup,
                ListScore = filter.ListScore,                
                MatchID = filter.MatchID,
                LiveIndicator = filter.LiveIndicator,
                IsHedging = filter.IsHedging
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MatchMonitorDetailModel>(criteria).ToList();
            }
        }

        public List<CustInfoModel> GetCustInfoByCustID(string custIDs)
        {
            var criteria = new CustInfoByCustIdGetCriteria()
            {
                CustIDs = custIDs
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustInfoModel>(criteria).ToList();
            }
        }

        public List<TicketDetailModel> GetTicketDetail(DateTime eventDate, string transIds)
        {
            var criteria = new TicketDetailGetCriteria()
            {
                QueryType = Consts.MatchMonitor_ReportQueryType,
                EventDate = eventDate,
                ListTransId = transIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<TicketDetailModel>(criteria).ToList();
            }
        }
    }
}