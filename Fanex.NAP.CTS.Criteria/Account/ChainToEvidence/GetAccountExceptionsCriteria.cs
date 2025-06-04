namespace Fanex.NAP.CTS.Criteria.Account.ChainToEvidence
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;
    public class GetAccountExceptionsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID", AllowNull = true)]
        public Int64 CTSCustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_GetException";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
