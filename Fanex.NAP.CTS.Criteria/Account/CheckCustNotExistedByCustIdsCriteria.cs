namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class CheckCustNotExistedByCustIdsCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_CheckCustNotExistedByCustIds";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIds);
        }
    }
}
