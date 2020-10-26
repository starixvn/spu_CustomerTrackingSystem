namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.ManageSubscriber;
    using System.Collections.Generic;

    public class AssociatedByAccountViewModel
    {
        public List<CustomerStaticStatusModel> AssociationStatus { get; set; }

        public List<CustomerStaticStatusModel> AssociationTypeList { get; set; }

        public List<AccountStaticStatusModel> AccountStatusList { get; set; }

        public List<SubscriberModel> SubscribersList { get; set; }

        public string Username { get; set; }

    }
}
