using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    public class GetCustomerByUsernameCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_Username")]
        public string Username { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_GetByUsername";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
