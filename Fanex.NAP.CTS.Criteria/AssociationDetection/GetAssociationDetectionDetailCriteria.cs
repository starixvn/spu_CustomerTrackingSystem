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

        [SpParam(SqlParamName = "ip_HasDevice")]
        public bool HasDevice { get; set; }

        [SpParam(SqlParamName = "ip_HasManually")]
        public bool HasManually { get; set; }

        [SpParam(SqlParamName = "ip_HasIP")]
        public bool HasIP { get; set; }

        [SpParam(SqlParamName = "ip_HasBettingPattern")]
        public bool HasBettingPattern { get; set; }

        [SpParam(SqlParamName = "ip_HasAgent")]
        public bool HasAgent { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociationDetection_GetCust";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
