namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DetectAssociationByPrimaryCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustIDs")]
        public string CTSCustIDs { get; set; }

        [SpParam(SqlParamName = "ip_HasDevice")]
        public bool IsDevice { get; set; }

        [SpParam(SqlParamName = "ip_HasAI")]
        public bool IsAI { get; set; }       

        [SpParam(SqlParamName = "ip_HasIP")]
        public bool IsIP { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_Primary_GetCustEdge";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(this.CTSCustIDs);
        }
    }
}
