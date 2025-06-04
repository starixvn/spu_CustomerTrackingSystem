namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DetectAssociationByPrimaryCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustIDs", AllowNull = true)]
        public string CTSCustIDs { get; set; }

        [SpParam(SqlParamName = "ip_HasDevice", AllowNull = true)]
        public bool IsDevice { get; set; }

        [SpParam(SqlParamName = "ip_HasAI", AllowNull = true)]
        public bool IsAI { get; set; }       

        [SpParam(SqlParamName = "ip_HasIP", AllowNull = true)]
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
