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
        public Int64 LastScannedSequenceID { get; set; }

        [SpParam(SqlParamName = "BatchSize")]
        public int BatchSize { get; set; }

        [SpParam(SqlParamName = "ListTransId")]
        public string ListTransId { get; set; }

        [SpParam(SqlParamName = "EventDate")]
        public DateTime EventDate { get; set; }

        public override string GetSettingKey()
                => "CTS_Rpt_MatchMonitor_TicketDetails";

        public override bool IsValid()
            => true;
    }
}
