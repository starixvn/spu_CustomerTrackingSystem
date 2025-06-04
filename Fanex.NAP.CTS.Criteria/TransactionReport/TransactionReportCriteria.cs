namespace Fanex.NAP.CTS.Criteria.TransactionReport
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class TransactionReportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberList", AllowNull = true)]
        public string SubscriberIDList { get; set; }

        [SpParam(SqlParamName = "ip_FromDate", AllowNull = true)]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate", AllowNull = true)]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "ip_Take", AllowNull = true)]
        public int Take { get; set; }

        [SpParam(SqlParamName = "ip_Skip", AllowNull = true)]
        public int Skip { get; set; }

        [SpParam(SqlParamName = "op_TotalRow", AllowNull = true)]
        public int TotalItem { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_RPT_Transaction";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
