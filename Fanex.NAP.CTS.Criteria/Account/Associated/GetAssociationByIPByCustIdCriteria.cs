namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetAssociationByIPByCustIdCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIDs { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_IP_GetByCustId";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIDs);
        }
    }
}
