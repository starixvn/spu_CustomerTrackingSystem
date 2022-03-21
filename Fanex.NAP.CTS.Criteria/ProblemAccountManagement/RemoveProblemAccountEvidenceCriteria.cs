namespace Fanex.NAP.CTS.Criteria.ProblemAccountManagement
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RemoveProblemAccountEvidenceCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIDs { get; set; }    

        [SpParam(SqlParamName = "ip_SourceType")] /*1: NAP, 0: Others*/
        public int SourceType { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_ProblemAccountManagement_Evidence_Remove";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
