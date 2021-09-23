namespace Fanex.NAP.CTS.Criteria.TransactionReport
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class TransactionReportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberIDs")]
        public string SubscriberIDs { get; set; }

        [SpParam(SqlParamName = "ip_FromAssDate")]
        public DateTime FromAssDate { get; set; }

        [SpParam(SqlParamName = "ip_ToAssDate")]
        public DateTime ToAssDate { get; set; }

        public override string GetSettingKey()
        {
            return "";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
