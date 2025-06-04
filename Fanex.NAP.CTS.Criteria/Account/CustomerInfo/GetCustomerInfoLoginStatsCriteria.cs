namespace Fanex.NAP.CTS.Criteria.Account.CustomerInfo
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetCustomerInfoLoginStatsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID", AllowNull = true)]
        public long CTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_FromDate", AllowNull = true)]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate", AllowNull = true)]
        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_Profile_GetLoginStats";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
