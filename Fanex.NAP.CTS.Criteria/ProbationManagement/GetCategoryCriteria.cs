namespace Fanex.NAP.CTS.Criteria.ProbationManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCategoryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CategoryMode")]
        public int CategoryMode { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Probation_GetCategory";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
