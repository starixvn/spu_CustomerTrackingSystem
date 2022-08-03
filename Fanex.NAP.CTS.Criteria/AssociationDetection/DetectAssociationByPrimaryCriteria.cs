namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DetectAssociationByPrimaryCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustIDs")]
        public string CTSCustIDs { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociationDetection_Primary_Detect";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(this.CTSCustIDs);
        }
    }
}
