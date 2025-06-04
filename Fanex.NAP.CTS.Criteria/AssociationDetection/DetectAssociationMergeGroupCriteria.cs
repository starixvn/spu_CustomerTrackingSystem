namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DetectAssociationMergeGroupCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustIDs", AllowNull = true)]
        public string CTSCustIDs { get; set; }

        [SpParam(SqlParamName = "ip_Device", AllowNull = true)]
        public string DeviceJson { get; set; }

        [SpParam(SqlParamName = "ip_AI", AllowNull = true)]
        public string AIJson { get; set; }

        [SpParam(SqlParamName = "ip_IP", AllowNull = true)]
        public string IPJson { get; set; }

        [SpParam(SqlParamName = "ip_Agent", AllowNull = true)]
        public string AgentJson { get; set; }

        [SpParam(SqlParamName = "ip_SharedIP", AllowNull = true)]
        public string SharedIPJson { get; set; }

        [SpParam(SqlParamName = "ip_SharedMatches", AllowNull = true)]
        public string SharedMatchesJson { get; set; }

        [SpParam(SqlParamName = "ip_IsGetLogTransInfo", AllowNull = true)]
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
