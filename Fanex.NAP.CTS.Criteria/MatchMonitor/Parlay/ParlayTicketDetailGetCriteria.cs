namespace Fanex.NAP.CTS.Criteria.MatchMonitor.Parlay
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class ParlayTicketDetailGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ViewMode")]
        public int ViewMode { get; set; } // (0:All, 1: Suspicious)

        [SpParam(SqlParamName = "EventDate")]
        public DateTime EventDate { get; set; }

        [SpParam(SqlParamName = "ListRefno")]
        public string ListRefno { get; set; } 

        [SpParam(SqlParamName = "MatchID")]
        public long MatchID { get; set; }

        [SpParam(SqlParamName = "IsLive")]
        public int IsLive { get; set; }

        [SpParam(SqlParamName = "ListBetType")]
        public string ListBetType { get; set; }

        [SpParam(SqlParamName = "FromTransDate")]
        public DateTime? FromTransDate { get; set; }

        [SpParam(SqlParamName = "ToTransDate")]
        public DateTime? ToTransDate { get; set; }

        [SpParam(SqlParamName = "ListScore")]
        public string ListScore { get; set; }

        [SpParam(SqlParamName = "IsLicensee")]
        public int? IsLicensee { get; set; }

        [SpParam(SqlParamName = "ListCustomerClass")]
        public string ListCustomerClass { get; set; }

        [SpParam(SqlParamName = "ListDanger")]
        public string ListDanger { get; set; }

        [SpParam(SqlParamName = "ListCurrency")]
        public string ListCurrency { get; set; }

        [SpParam(SqlParamName = "ListReason")]
        public string ListReason { get; set; }

        [SpParam(SqlParamName = "ListAGroup")]
        public string ListAGroup { get; set; }

        [SpParam(SqlParamName = "ListMGroup")]
        public string ListMGroup { get; set; }

        [SpParam(SqlParamName = "ListHDPBetTeam")]
        public string ListHDPBetTeam { get; set; }

        [SpParam(SqlParamName = "CustAmountRM")]
        public decimal? CustAmountRM { get; set; }

        [SpParam(SqlParamName = "ListStatus")]
        public string ListStatus { get; set; }

        [SpParam(SqlParamName = "LastScannedSequenceID")]
        public Int64? LastScannedSequenceID { get; set; }

        [SpParam(SqlParamName = "LastTransDate")]
        public string LastTransDate { get; set; }

        [SpParam(SqlParamName = "BatchSize")]
        public int? BatchSize { get; set; }

        [SpParam(SqlParamName = "IsExclTestCurr")]
        public bool IsExclTestCurr { get; set; }    

        [SpParam(SqlParamName = "MinSequenceId")]
        public long? MinSequenceId { get; set; }

        [SpParam(SqlParamName = "MinTransDate")]
        public DateTime? MinTransDate { get; set; }

        [SpParam(SqlParamName = "TotalTicket")]
        public int? TotalTicket { get; set; }
       
        public override string GetSettingKey()
            => "CTS_MatchMonitorParlay_Details_GetTicket";

        public override bool IsValid()
            => true;
    }
}
