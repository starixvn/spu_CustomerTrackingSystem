namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetExistedTWRobotUserCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "custids")]
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Customer_RobotCheckExist";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
