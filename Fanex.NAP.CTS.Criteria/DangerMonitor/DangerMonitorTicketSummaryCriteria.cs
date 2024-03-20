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

        [SpParam(SqlParamName = "ip_DangerType")]
        public int DangerType { get; set; }

        [SpParam(SqlParamName = "ip_DangerList")]
        public string DangerList { get; set; }

        [SpParam(SqlParamName = "ip_MarketList")]
        public string MarketList { get; set; }

        [SpParam(SqlParamName = "ip_DBSource")]
        public int DBSource { get; set; }

        [SpParam(SqlParamName = "ip_SabaLeagueGroup")]
        public int? SabaLeagueGroup { get; set; }

        [SpParam(SqlParamName = "ip_ExcludeLeagueGroup")]
        public string ExcludeLeagueGroup { get; set; }

        public override string GetSettingKey() => "CTS_MatchTicketInfo_Get";

        public override bool IsValid() => true;
    }
}
