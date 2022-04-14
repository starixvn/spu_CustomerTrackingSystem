namespace Fanex.NAP.CTS.Criteria.ProblemAccountManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateProblemAccountEvidenceCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustInfo")]
        public string CustInfo { get; set; }

        [SpParam(SqlParamName = "ip_ModifiedBy")]
        public int UserID { get; set; }

        [SpParam(SqlParamName = "ip_SourceType")] /*1: NAP, 0: Others*/
        public int SourceType { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_ProblemAccountManagement_Evidence_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
