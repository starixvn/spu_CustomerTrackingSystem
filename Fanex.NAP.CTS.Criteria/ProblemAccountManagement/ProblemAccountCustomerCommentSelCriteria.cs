namespace Fanex.NAP.CTS.Criteria.ProblemAccountManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProblemAccountCustomerCommentSelCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "CustIdList")]
        public string CustIdList { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_ProblemAccount_CustomerCommentSel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
