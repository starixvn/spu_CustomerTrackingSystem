namespace Fanex.NAP.CTS.Criteria.Api.CtsAssociationAPI
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DetectAssociationByTypeCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID")]
        public long CustID { get; set; }

        [SpParam(SqlParamName = "ip_HasDevice")]
        public bool IsDevice { get; set; }

        [SpParam(SqlParamName = "ip_HasAI")]
        public bool IsAI { get; set; }

        [SpParam(SqlParamName = "ip_HasIP")]
        public bool IsIP { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_GetByCustID";
        }

        public override bool IsValid()
        {
            return this.CustID > 0;
        }
    }
}
