namespace Fanex.NAP.CTS.Criteria.Api.ProblemCaseMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class ProblemCaseMonitorCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberIDList")]
        public string SubscriberIDList { get; set; }

        [SpParam(SqlParamName = "ip_ProblemCaseIDList")]
        public string ProblemCaseIDList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_RPT_ProblemCaseMonitor";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
