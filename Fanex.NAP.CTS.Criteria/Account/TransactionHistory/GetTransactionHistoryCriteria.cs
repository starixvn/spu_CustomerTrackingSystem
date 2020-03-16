namespace Fanex.NAP.CTS.Criteria.Account.TransactionHistory
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetTransactionHistoryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberId")]
        public int SubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_CTSCustId")]
        public long CTSCustId { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Get_HistoryTransaction";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
