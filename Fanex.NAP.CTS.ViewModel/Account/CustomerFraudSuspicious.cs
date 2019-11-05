namespace Fanex.NAP.CTS.ViewModel.Account
{
    using System.Collections.Generic;

    public class CustomerFraudSuspicious
    {
        public IEnumerable<CustomerFraud> CustomerFrauds { get; set; }

        public IEnumerable<CustomerSuspicious> CustomerSuspiciouses { get; set; }        
    }
}