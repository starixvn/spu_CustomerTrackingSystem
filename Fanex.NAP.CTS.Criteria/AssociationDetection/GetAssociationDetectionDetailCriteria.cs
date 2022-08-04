namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssociationDetectionDetailCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_ListCTSCustID")]
        public string ListCTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_HasPrimary")]
        public bool HasPrimary { get; set; }

        [SpParam(SqlParamName = "ip_HasAgent")]
        public bool HasAgent { get; set; }     

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociationDetection_GetDetail";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
