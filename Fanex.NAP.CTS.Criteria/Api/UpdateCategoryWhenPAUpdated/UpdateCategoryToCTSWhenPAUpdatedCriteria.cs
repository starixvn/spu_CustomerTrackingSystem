namespace Fanex.NAP.CTS.Criteria.Api.UpdateCategoryWhenPAUpdated
{
    using Fanex.Data;
    using Fanex.Data.Repository;    

    public class UpdateCategoryToCTSWhenPAUpdatedCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustInfo")]
        public string CustInfo { get; set; }

        [SpParam(SqlParamName = "ip_ModifiedBy")]
        public int UserId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_UpdateProblemAccount";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
