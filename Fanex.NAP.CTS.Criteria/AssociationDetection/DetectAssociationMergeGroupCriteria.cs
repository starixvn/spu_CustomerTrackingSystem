namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DetectAssociationMergeGroupCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustIDs")]
        public string CTSCustIDs { get; set; }

        [SpParam(SqlParamName = "ip_Device")]
        public string DeviceJson { get; set; }

        [SpParam(SqlParamName = "ip_AI")]
        public string AIJson { get; set; }

        [SpParam(SqlParamName = "ip_IP")]
        public string IPJson { get; set; }

        [SpParam(SqlParamName = "ip_Agent")]
        public string AgentJson { get; set; }

        [SpParam(SqlParamName = "ip_SharedIP")]
        public string SharedIPJson { get; set; }

        [SpParam(SqlParamName = "ip_SharedMatches")]
        public string SharedMatchesJson { get; set; }

        [SpParam(SqlParamName = "ip_IsGetLogTransInfo")]
        public bool IsGetLogTransInfo { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_DetectGroup";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(this.CTSCustIDs);
        }
    }
}
