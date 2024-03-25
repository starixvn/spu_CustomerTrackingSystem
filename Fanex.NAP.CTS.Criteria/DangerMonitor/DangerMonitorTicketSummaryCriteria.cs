namespace Fanex.NAP.CTS.Criteria.DangerMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class DangerMonitorTicketSummaryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime? FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime? ToDate { get; set; }

        [SpParam(SqlParamName = "ip_SportList")]
        public string SportList { get; set; }

        [SpParam(SqlParamName = "ip_LeagueEvent")]
        public string LeagueEvent { get; set; }

        [SpParam(SqlParamName = "ip_BettypeList")]
        public string BettypeList { get; set; }

        [SpParam(SqlParamName = "ip_QueryDanger")]
        public string QueryDanger { get; set; }

        [SpParam(SqlParamName = "ip_MarketList")]
        public string MarketList { get; set; }

        [SpParam(SqlParamName = "ip_SportSabaLgGroup")]
        public string SportSabaLgGroup { get; set; }

        [SpParam(SqlParamName = "ip_MatchSection")]
        public string MatchSection { get; set; }

        public override string GetSettingKey() => "CTS_MatchTicketInfo_Get";

        public override bool IsValid() => true;
    }
}
