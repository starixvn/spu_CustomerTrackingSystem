namespace Fanex.NAP.CTS.Criteria.AssociationDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DetectAssociationByUserRootCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_UserNameList")]
        public string UserNames { get; set; }

        [SpParam(SqlParamName = "ip_HasDevice")]
        public bool IsDevice { get; set; }

        [SpParam(SqlParamName = "ip_HasAI")]
        public bool IsAI { get; set; }

        [SpParam(SqlParamName = "ip_HasIP")]
        public bool IsIP { get; set; }

        [SpParam(SqlParamName = "ip_HasAgent")]
        public bool IsAgent { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_GetByUserNameList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
