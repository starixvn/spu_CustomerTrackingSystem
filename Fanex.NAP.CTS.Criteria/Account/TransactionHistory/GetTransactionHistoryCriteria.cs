namespace Fanex.NAP.CTS.Criteria.Account.TransactionHistory
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetTransactionHistoryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_DateRange")]
        public int DateRange { get; set; }       

        [SpParam(SqlParamName = "ip_SubscriberId")]
        public int SubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_CTSCustId")]
        public long CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_PageSize")]
        public int PageSize { get; set; }

        [SpParam(SqlParamName = "ip_PageNumber")]
        public int PageNumber { get; set; }

        [SpParam(SqlParamName = "op_TotalItem")]
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
