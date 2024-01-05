namespace Fanex.NAP.CTS.Criteria.DangerMonitor
{
    using Fanex.Data.Repository;

    public class DangerMonitorTicketSummaryCriteria : CriteriaBase
    {
        public string MatchIds { get; set; }

        public string BettypeList { get; set; }

        public string DangerList { get; set; }

        public string MarketList { get; set; }

        public override string GetSettingKey() => "CTS_MatchTicketInfo_Get";

        public override bool IsValid() => true;
    }
}
