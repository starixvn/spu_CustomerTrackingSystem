using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.Account
{
    public class CustomerLastTicketDateCriteria : CriteriaBase
    {
        public long CustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_CustomerLastTicketDate_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
