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

        [SpParam(SqlParamName = "ip_HasAI")]
        public bool IsBP { get; set; }

        [SpParam(SqlParamName = "ip_HasDevice")]
        public bool IsDevice { get; set; }       

        [SpParam(SqlParamName = "ip_HasIP")]
        public bool IsIP { get; set; }

        [SpParam(SqlParamName = "ip_HasAgent")]
        public bool IsAgent { get; set; }     

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
