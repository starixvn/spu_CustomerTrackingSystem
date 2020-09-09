using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategory
{
    public class GetCustomerClassByCustIdCriteria : NonQueryCommand
    {
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_CustomerClass_GetByCustId";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
