namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DetectAssociationByAgentCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustIDs", AllowNull = true)]
        public string CTSCustIDs { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_Agent_GetCustEdge";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(this.CTSCustIDs);
        }
    }
}
