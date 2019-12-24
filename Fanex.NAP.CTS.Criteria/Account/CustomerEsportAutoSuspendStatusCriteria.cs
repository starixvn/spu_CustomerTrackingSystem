using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.Account
{
    public class CustomerEsportAutoSuspendStatusCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_CustomerEsportAutoSuspendStatus_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
