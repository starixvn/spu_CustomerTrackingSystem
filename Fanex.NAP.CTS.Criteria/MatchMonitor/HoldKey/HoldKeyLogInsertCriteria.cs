namespace Fanex.NAP.CTS.Criteria.MatchMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class HoldKeyLogInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_UserID", AllowNull = true)]
        public int UserID { get; set; }

        [SpParam(SqlParamName = "ip_MatchID", AllowNull = true)]
        public int MatchID { get; set; }

        [SpParam(SqlParamName = "ip_Status", AllowNull = true)]
        public bool Status { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_MatchMonitor_HoldKey_InsertLog";

        public override bool IsValid()
            => true;
    }
}
