namespace Fanex.NAP.CTS.Criteria.ProbationManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetProbationCategoryCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_Username")]
        public string UserName { get; set; }

        [SpParam(SqlParamName = "ip_FromDaysLeft")]
        public int FromDaysLeft { get; set; }

        [SpParam(SqlParamName = "ip_ToDaysLeft")]
        public int ToDaysLeft { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_StatusIDs")]
        public string StatusIDs { get; set; }       

        [SpParam(SqlParamName = "ip_isAssociatedPA")]
        public int IsAssociatedPA { get; set; }

        [SpParam(SqlParamName = "ip_CategoryMode")]
        public int CategoryMode { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Probation_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
