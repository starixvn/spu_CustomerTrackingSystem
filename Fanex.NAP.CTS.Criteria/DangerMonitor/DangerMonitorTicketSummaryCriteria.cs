namespace Fanex.NAP.CTS.Criteria.DangerMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class DangerMonitorTicketSummaryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "FromDate")]
        public DateTime? FromDate { get; set; }

        [SpParam(SqlParamName = "ToDate")]
        public DateTime? ToDate { get; set; }

        [SpParam(SqlParamName = "SportList")]
        public string SportList { get; set; }

        [SpParam(SqlParamName = "LeagueEvent")]
        public string LeagueEvent { get; set; }

        [SpParam(SqlParamName = "BettypeList")]
        public string BettypeList { get; set; }

        [SpParam(SqlParamName = "DangerLevels")]
        public string QueryDanger { get; set; }

        [SpParam(SqlParamName = "MarketList")]
        public string MarketList { get; set; }

        [SpParam(SqlParamName = "SportSabaLeagueGroup")]
        public string SportSabaLgGroup { get; set; }

        [SpParam(SqlParamName = "MatchSection")]
        public string MatchSection { get; set; }

        public override string GetSettingKey() => "CTS_MatchTicketInfo_Get";

        public override bool IsValid() => true;
    }
}
