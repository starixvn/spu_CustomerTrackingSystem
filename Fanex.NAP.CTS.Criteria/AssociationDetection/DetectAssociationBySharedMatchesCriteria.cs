namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DetectAssociationBySharedMatchesCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIDs { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Association_BySharedMatches";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
