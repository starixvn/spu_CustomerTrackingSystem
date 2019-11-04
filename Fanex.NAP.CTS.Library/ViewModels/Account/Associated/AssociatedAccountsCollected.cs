namespace Fanex.NAP.CTS.Library.ViewModels.Account.Associated
{
    using Fanex.NAP.CTS.Library.Models.Account;
    using System.Collections.Generic;

    public class AssociatedAccountsCollected
    {
        public IEnumerable<CustomerByGraphDB> CustomersFromAPI { get; set; }

        public IEnumerable<AssociatedAccountByCustomer> AccountsByCustomers { get; set; }

        public CustomerFraudSuspicious CustomerFraudSuspicious { get; set; }
    }
}