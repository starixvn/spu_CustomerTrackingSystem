namespace Fanex.NAP.CTS.Criteria.ProbationManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAccummulatedInfoCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "CustomersXML")]
        public string CustomersXML { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_GetAccummulatedInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
