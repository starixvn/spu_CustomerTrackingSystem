using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.Account
{
    public class CustomerAssociationByCTSCustIdCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public Int64 CTSCustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_GetAssociationByCTSCustID";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
