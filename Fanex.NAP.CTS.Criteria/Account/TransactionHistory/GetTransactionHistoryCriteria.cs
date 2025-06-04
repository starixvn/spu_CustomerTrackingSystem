namespace Fanex.NAP.CTS.Criteria.Account.TransactionHistory
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetTransactionHistoryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_DateRange", AllowNull = true)]
        public int DateRange { get; set; }             

        [SpParam(SqlParamName = "ip_CTSCustId", AllowNull = true)]
        public long CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_Take", AllowNull = true)]
        public int Take { get; set; }

        [SpParam(SqlParamName = "ip_Skip", AllowNull = true)]
        public int Skip { get; set; }

        [SpParam(SqlParamName = "op_TotalItem", AllowNull = true)]
        public int TotalItem { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transaction_GetHistory";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
