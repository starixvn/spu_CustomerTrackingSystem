using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Criteria.CustomerClass
{
    public class SyncCustomerClassToMainDbCriteria : NonQueryCommand
    {
        public string custList { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_CustomerUpdateCustomerClass";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
