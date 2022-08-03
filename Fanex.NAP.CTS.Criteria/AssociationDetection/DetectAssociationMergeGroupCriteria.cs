namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DetectAssociationMergeGroupCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustIDs")]
        public string CTSCustIDs { get; set; }

        [SpParam(SqlParamName = "ip_Primary")]
        public string PrimaryJson { get; set; }

        [SpParam(SqlParamName = "ip_Agent")]
        public string AgentJson { get; set; }

        [SpParam(SqlParamName = "ip_SharedIP")]
        public string SharedIPJson { get; set; }

        [SpParam(SqlParamName = "ip_SharedMatches")]
        public string SharedMatches { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociationDetection_MergeGroup";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(this.CTSCustIDs);
        }
    }
}
