namespace Fanex.NAP.CTS.Criteria.ProbationManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetAccountStaticStatusCriteria : CriteriaBase
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