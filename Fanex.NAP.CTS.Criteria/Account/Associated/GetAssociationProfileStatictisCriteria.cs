namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssociationProfileStatictisCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_Profile_GetStat";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
