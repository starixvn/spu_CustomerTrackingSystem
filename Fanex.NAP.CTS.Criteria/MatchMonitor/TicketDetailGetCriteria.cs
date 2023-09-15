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

        [SpParam(SqlParamName = "IsLive")]
        public int IsLive { get; set; }

        [SpParam(SqlParamName = "BatchSize")]
        public int? BatchSize { get; set; }

        [SpParam(SqlParamName = "ListTransId")]
        public string ListTransId { get; set; }

        [SpParam(SqlParamName = "EventDate")]
        public DateTime EventDate { get; set; }       

        [SpParam(SqlParamName = "MatchID")]
        public long MatchID { get; set; }

        [SpParam(SqlParamName = "BetType")]
        public int BetType { get; set; }

        [SpParam(SqlParamName = "IsLicensee")]
        public int? IsLicensee { get; set; }

        [SpParam(SqlParamName = "ListCurrency")]
        public string ListCurrency { get; set; }

        [SpParam(SqlParamName = "ListHDPBetTeam")]
        public string ListHDPBetTeam { get; set; }

        [SpParam(SqlParamName = "ListScore")]
        public string ListScore { get; set; }

        [SpParam(SqlParamName = "CustAmountRM")]
        public decimal? CustAmountRM { get; set; }

        [SpParam(SqlParamName = "BetID")]
        public long BetID { get; set; }

        [SpParam(SqlParamName = "ListDanger")]
        public string ListDanger { get; set; }

        [SpParam(SqlParamName = "ListStatus")]
        public string ListStatus { get; set; }

        [SpParam(SqlParamName = "ListCustomerClass")]
        public string ListCustomerClass { get; set; }      

        [SpParam(SqlParamName = "MinSequenceId")]
        public long? MinSequenceId { get; set; }

        [SpParam(SqlParamName = "TotalTicket")]
        public int? TotalTicket { get; set; }

        public override string GetSettingKey()
                => "CTS_Rpt_MatchMonitor_TicketDetails";

        public override bool IsValid()
            => true;
    }
}
