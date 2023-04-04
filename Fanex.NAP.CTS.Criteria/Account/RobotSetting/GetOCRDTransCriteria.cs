namespace Fanex.NAP.CTS.Criteria.Account.RobotSetting
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetOCRDTransCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID")]
        public long CustID { get; set; }

        [SpParam(SqlParamName = "ip_MatchID")]
        public long MatchID { get; set; }     

        public override string GetSettingKey()
        {
            return "SPU_AIML_OCRD_GetDetails";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
