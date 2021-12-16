namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Criteria.MatchMonitor;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using System;
    using System.Collections.Generic;

    public class MatchMonitorService : IMatchMonitorService
    {
        private readonly IDynamicRepository repository;

        public MatchMonitorService()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<MatchMonitorModel> GetMatchMonitor(MatchMonitorFilterViewModel filter)
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

            return repository.Fetch<MatchMonitorModel>(criteria);
        }

        public DBResult VerifyTransMatchMonitorDetail(MatchMonitorDetailVerifyTransParaModel para)
        {
            var criteria = new MatchMonitorDetailVerifyCriteria()
            {
                MatchID = para.MatchId,
                LiveIndicator = para.LiveIndicator,
                BettypeID = para.BettypeID,
                TranIds = para.TranIds,
                VerifiedBy = para.VerifiedBy
            };

            repository.Fetch<DBResult>(criteria);

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }

        public IEnumerable<MatchGroupModel> GetMatchGroup(Int64 matchID, int betTypeID, bool liveIndicator, bool isHedging)
        {
            var criteria = new MatchGroupGetCriteria()
            {
                BettypeID = betTypeID,
                MatchID = matchID,
                LiveIndicator = liveIndicator,
                IsHedging = isHedging
            };

            return repository.Fetch<MatchGroupModel>(criteria);
        }

        public IEnumerable<MatchScoreModel> GetMatchScore(Int64 matchID, int betTypeID, bool liveIndicator, bool isHedging)
        {
            var criteria = new MatchScoreGetCriteria()
            {
                BettypeID = betTypeID,
                MatchID = matchID,
                LiveIndicator = liveIndicator,
                IsHedging = isHedging
            };

            return repository.Fetch<MatchScoreModel>(criteria);
        }

        public MatchMonitorDetailResultModel GetMatchMonitorDetail(MatchMonitorDetailFilterViewModel filter)
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
                var result = db.QueryMultiple(criteria);

                return new MatchMonitorDetailResultModel
                {
                    MatchDetails = result?.Read<MatchMonitorDetailModel>() ?? new List<MatchMonitorDetailModel>(),
                    VerifiedTrans = result?.Read<MatchMonitorDetailVerifyTransResultModel>() ?? new List<MatchMonitorDetailVerifyTransResultModel>()
                };
            }
        }

        public IEnumerable<CustInfoModel> GetCustInfoByCustID(string custIDs)
        {
            var criteria = new CustInfoByCustIdGetCriteria()
            {
                CustIDs = custIDs
            };

            return repository.Fetch<CustInfoModel>(criteria);
        }

        public IEnumerable<TicketDetailModel> GetTicketDetail(DateTime eventDate, string transIds)
        {
            var criteria = new TicketDetailGetCriteria()
            {
                QueryType = Consts.MatchMonitor_ReportQueryType,
                EventDate = eventDate,
                ListTransId = transIds
            };

            return repository.Fetch<TicketDetailModel>(criteria);
        }
    }
}