namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using Fanex.NAP.CTS.Model.Account;
    using System.Collections.Generic;

    public class AssociatedAccountsCollected
    {
        public IEnumerable<CustomerByGraphDB> CustomersFromAPI { get; set; }

        public IEnumerable<AssociatedAccountByCustomer> AccountsByCustomers { get; set; }

        public CustomerFraudSuspicious CustomerFraudSuspicious { get; set; }
    }
}