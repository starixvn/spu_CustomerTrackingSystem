namespace Fanex.NAP.CTS.Criteria.ProbationManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateProbationCategoryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustidList")]
        public string CustIdList { get; set; }

        [SpParam(SqlParamName = "ip_Remark")]
        public string Remark { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy")]
        public int CreatedBy { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Probation_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
