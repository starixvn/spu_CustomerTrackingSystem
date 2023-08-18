namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class MMTicketDetailInfoCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "MatchID")]
        public Int64 MatchID { get; set; }

        [SpParam(SqlParamName = "BetType")]
        public Int32 BetType { get; set; }

        [SpParam(SqlParamName = "BetID")]
        public long BetID { get; set; }

        [SpParam(SqlParamName = "ListTransId")]
        public string ListTransId { get; set; }

        [SpParam(SqlParamName = "IsLive")]
        public Int32 IsLive { get; set; }

        [SpParam(SqlParamName = "MaxSequenceId")]
        public Int64? MaxSequenceId { get; set; }

        [SpParam(SqlParamName = "TotalTicketAll")]
        public Int64 TotalTicketAll { get; set; }

        [SpParam(SqlParamName = "TotalTicketLicensee")]
        public Int64 TotalTicketLicensee { get; set; }

        [SpParam(SqlParamName = "TotalTicketCredit")]
        public Int64 TotalTicketCredit { get; set; }

        public override string GetSettingKey()
            => "CTS_TicketDetails_Info";

        public override bool IsValid()
            => true;
    }
}
