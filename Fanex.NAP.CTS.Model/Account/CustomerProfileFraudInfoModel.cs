using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Model.Account
{
   public class CustomerProfileFraudInfoModel
    {
        public SuspiciousCustomerModel SuspiciouseCustomer { get; set; }

        public List<FraudCaseModel> FraudCaseList { get; set; }
    }
}
