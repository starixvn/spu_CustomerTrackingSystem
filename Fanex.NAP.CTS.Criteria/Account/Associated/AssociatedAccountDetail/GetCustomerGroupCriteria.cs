namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerGroupCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_GroupList", AllowNull = true)]
        public string GroupList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociationGroupByAI_GetCustomersByGroup";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
