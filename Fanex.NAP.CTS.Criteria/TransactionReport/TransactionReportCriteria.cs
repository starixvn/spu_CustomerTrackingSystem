namespace Fanex.NAP.CTS.Criteria.TransactionReport
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class TransactionReportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberList")]
        public string SubscriberIDList { get; set; }

        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "ip_Take")]
        public int Take { get; set; }

        [SpParam(SqlParamName = "ip_Skip")]
        public int Skip { get; set; }

        [SpParam(SqlParamName = "op_TotalRow")]
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
