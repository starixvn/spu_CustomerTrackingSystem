namespace Fanex.NAP.CTS.Criteria.MatchMonitor.Parlay
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class ParlayTicketInfoGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "MatchID")]
        public long MatchID { get; set; }

        [SpParam(SqlParamName = "EventDate")]
        public DateTime EventDate { get; set; }

        [SpParam(SqlParamName = "IsLive")]
        public bool IsLive { get; set; }

        [SpParam(SqlParamName = "ListSuspiciousTransId")]
        public string ListSuspiciousTransId { get; set; }

        [SpParam(SqlParamName = "ViewMode")]
        public int ViewMode { get; set; }

        [SpParam(SqlParamName = "IsExclTestCurr")]
        public bool IsExclTestCurr { get; set; }

        [SpParam(SqlParamName = "MaxSequenceId")]
        public long? MaxSequenceId { get; set; }

        [SpParam(SqlParamName = "MaxTransDate")]
        public DateTime? MaxTransDate { get; set; }

        public override string GetSettingKey()
            => "CTS_MatchMonitorParlay_Details_GetFilter";

        public override bool IsValid()
            => true;
    }
}
