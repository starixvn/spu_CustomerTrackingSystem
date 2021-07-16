namespace Fanex.NAP.CTS.Criteria.ProblemCaseMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProblemCaseFilterGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ListID")]
        public int ListId { get; set; }

        [SpParam(SqlParamName = "ip_Status")]
        public bool Status { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Common_GetStaticList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
