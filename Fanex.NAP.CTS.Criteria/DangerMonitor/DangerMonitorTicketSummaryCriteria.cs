namespace Fanex.NAP.CTS.Criteria.DangerMonitor
{
    using Fanex.Data.Repository;
    using System;

    public class DangerMonitorTicketSummaryCriteria : CriteriaBase
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string SportList { get; set; }

        public string LeagueEvent { get; set; }

        public string MatchSection { get; set; }

        public string BettypeList { get; set; }

        public string DangerList { get; set; }

        public string MarketList { get; set; }

        public int BatchSize { get; set; }

        public DateTime? LastEventDate { get; set; }

        public long? LastMatchID { get; set; }

        public override string GetSettingKey() => "CTS_MatchTicketInfo_Get";

        public override bool IsValid() => true;
    }
}
