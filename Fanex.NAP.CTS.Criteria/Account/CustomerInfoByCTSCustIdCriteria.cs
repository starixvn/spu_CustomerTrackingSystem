namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class CustomerInfoByCTSCustIdCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID", AllowNull = true)]
        public Int64 CTSCustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_GetByCTSCustID";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
