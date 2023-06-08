namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;
    public class MMTicketDetailInfoCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "MatchID")]
        public Int64 MatchID { get; set; }

        [SpParam(SqlParamName = "BetType")]
        public Int32 BetType { get; set; }

        [SpParam(SqlParamName = "IsLive")]
        public Int32 IsLive { get; set; }

        [SpParam(SqlParamName = "TotalTicket")]
        public Int64 TotalTicket { get; set; }

        [SpParam(SqlParamName = "MaxSequenceId")]
        public Int64? MaxSequenceId { get; set; }

        [SpParam(SqlParamName = "ListCurrency")]
        public string ListCurrency { get; set; }

        [SpParam(SqlParamName = "ListHDP")]
        public string ListHDP { get; set; }

        [SpParam(SqlParamName = "ListScore")]
        public string ListScore { get; set; }

        public override string GetSettingKey()
            => "CTS_TicketDetails_Info";

        public override bool IsValid()
            => true;
    }
}
