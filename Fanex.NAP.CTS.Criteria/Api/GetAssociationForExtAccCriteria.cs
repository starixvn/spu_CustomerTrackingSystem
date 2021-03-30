namespace Fanex.NAP.CTS.Criteria.Api
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssociationForExtAccCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID")]
        public int CustId { get; set; }

        [SpParam(SqlParamName = "ip_AssociationType")]
        public int AssType { get; set; }

        [SpParam(SqlParamName = "ip_AssociationStatus")]
        public int AssStatus { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_GetForExtAcc";
        }

        public override bool IsValid()
        {
            return CustId > 0;
        }
    }
}
