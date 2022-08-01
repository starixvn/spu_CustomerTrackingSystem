namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DetectAssociationBySharedMatchesCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ListCustId")]
        public string CustIDList { get; set; }

        [SpParam(SqlParamName = "IsDetecting")]
        public bool IsDetecting { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Association_GetBySharedMatches";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
