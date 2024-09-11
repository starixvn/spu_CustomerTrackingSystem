namespace Fanex.NAP.CTS.Criteria.MatchMonitor.Parlay
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class ParlayTicketInfoGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ViewMode")]
        public int ViewMode { get; set; }

        [SpParam(SqlParamName = "MatchID")]
        public long MatchID { get; set; }

        [SpParam(SqlParamName = "EventDate")]
        public DateTime EventDate { get; set; }

        [SpParam(SqlParamName = "IsLive")]
        public int IsLive { get; set; }

        [SpParam(SqlParamName = "ListSportBettype")]
        public string ListSportBettype { get; set; }

        [SpParam(SqlParamName = "ListSuspiciousRefno")]
        public string ListSuspiciousRefno { get; set; }

        [SpParam(SqlParamName = "IsExclTestCurr")]
        public bool IsExclTestCurr { get; set; }

        [SpParam(SqlParamName = "FromTransDate")]
        public DateTime? FromTransDate { get; set; }

        [SpParam(SqlParamName = "ToTransDate")]
        public DateTime? ToTransDate { get; set; }

        [SpParam(SqlParamName = "MaxSequenceId")]
        public long? MaxSequenceId { get; set; }

        [SpParam(SqlParamName = "MaxTransDate")]
        public DateTime? MaxTransDate { get; set; }

        [SpParam(SqlParamName = "TotalTicketAll")]
        public Int64 TotalTicketAll { get; set; }

        [SpParam(SqlParamName = "TotalTicketLicensee")]
        public Int64 TotalTicketLicensee { get; set; }

        [SpParam(SqlParamName = "TotalTicketCredit")]
        public Int64 TotalTicketCredit { get; set; }

        public override string GetSettingKey()
            => "CTS_MatchMonitorParlay_Details_GetFilter";

        public override bool IsValid()
            => true;
    }
}
