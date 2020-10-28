namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssociationWithCategoryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_GetWithCategory";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIds);
        }
    }
}
