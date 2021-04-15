namespace Fanex.NAP.CTS.Criteria.CustomerClassify
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustByRobotSiteRuleCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_RobotUser_CheckSiteByCustIds";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
