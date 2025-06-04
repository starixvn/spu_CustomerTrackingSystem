namespace Fanex.NAP.CTS.Criteria.Account.ChainToEvidence
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RemoveAccountExceptionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_UserID", AllowNull = true)]
        public int UserId { get; set; }

        [SpParam(SqlParamName = "ip_FromCTSCustID", AllowNull = true)]
        public long FromCTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_ToCTSCustID", AllowNull = true)]
        public long ToCTSCustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_RemoveException";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
