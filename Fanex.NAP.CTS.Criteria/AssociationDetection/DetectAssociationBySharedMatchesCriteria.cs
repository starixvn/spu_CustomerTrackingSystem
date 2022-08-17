namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DetectAssociationBySharedMatchesCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ListCustId")]
        public string CustIDList { get; set; }

        [SpParam(SqlParamName = "RootCustId")]
        public long RootCustID { get; set; }

        [SpParam(SqlParamName = "QueryType")]
        public int QueryType { get; set; } /* QueryType: 1 ass detect, 2 get fraud ticket detail, 3 get ass by group */

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
