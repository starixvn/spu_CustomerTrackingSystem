namespace Fanex.NAP.CTS.Criteria.Account.RobotSetting
{
    using Fanex.Data;
    using Fanex.Data.Repository;   

    public class GetOCRDMatchByCustCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID")]
        public long CustID { get; set; }       

        public override string GetSettingKey()
        {
            return "SPU_AIML_OCRD_GetMatchByCust";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
