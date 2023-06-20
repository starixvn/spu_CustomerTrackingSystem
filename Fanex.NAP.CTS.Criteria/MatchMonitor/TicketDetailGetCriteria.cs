namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class TicketDetailGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "QueryType")]
        public int QueryType { get; set; }

        [SpParam(SqlParamName = "LastScannedSequenceID")]
        public Int64? LastScannedSequenceID { get; set; }

        [SpParam(SqlParamName = "BatchSize")]
        public int BatchSize { get; set; }

        [SpParam(SqlParamName = "ListTransId")]
        public string ListTransId { get; set; }

        [SpParam(SqlParamName = "EventDate")]
        public DateTime EventDate { get; set; }

        [SpParam(SqlParamName = "IsLive")]
        public int IsLive { get; set; }

        [SpParam(SqlParamName = "MatchID")]
        public long MatchID { get; set; }

        [SpParam(SqlParamName = "BetType")]
        public int BetType { get; set; }

        [SpParam(SqlParamName = "IsLiencee")]
        public int? IsLiencee { get; set; }

        [SpParam(SqlParamName = "ListCurrency")]
        public string ListCurrency { get; set; }

        [SpParam(SqlParamName = "ListHDP")]
        public string ListHDP { get; set; }

        [SpParam(SqlParamName = "ListScore")]
        public string ListScore { get; set; }

        [SpParam(SqlParamName = "FromStakeRM")]
        public decimal? FromStakeRM { get; set; }

        [SpParam(SqlParamName = "ToStakeRM")]
        public decimal? ToStakeRM { get; set; }

        [SpParam(SqlParamName = "NextScannedSequenceID")]
        public Int64? NextScannedSequenceID { get; set; }

        public override string GetSettingKey()
                => "CTS_Rpt_MatchMonitor_TicketDetails";

        public override bool IsValid()
            => true;
    }
}
