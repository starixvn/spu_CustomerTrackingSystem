namespace Fanex.NAP.CTS.Criteria.DangerMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class DangerMonitorSearchMatchInfoWithAssociationRootUserCriteria : CriteriaBase
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

        [SpParam(SqlParamName = "LeagueGroupList")]
        public string LeagueGroupList { get; set; }

        [SpParam(SqlParamName = "MarketList")]
        public string MarketList { get; set; }

        [SpParam(SqlParamName = "SportSabaLeagueGroup")]
        public string SportSabaLgGroup { get; set; }

        [SpParam(SqlParamName = "MatchSection")]
        public string MatchSection { get; set; }

        [SpParam(SqlParamName = "UserAmount")]
        public decimal UserAmount { get; set; }

        [SpParam(SqlParamName = "TotalAmount")]
        public decimal TotalAmount { get; set; }

        [SpParam(SqlParamName = "AssociationsList")]
        public string AssociationRootUser { get; set; }

        public override string GetSettingKey() => "CTS_MatchTicketInfo_Associations_Get";

        public override bool IsValid() => true;
    }
}
