namespace Fanex.NAP.CTS.Criteria.CustomerClassify
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetExistedRobotUserCriteria : CriteriaBase
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
