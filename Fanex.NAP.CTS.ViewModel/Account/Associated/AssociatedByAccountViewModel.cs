namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.ViewModel.Common;
    using System.Collections.Generic;

    public class AssociatedByAccountViewModel
    {
        public List<CustomerStaticStatusModel> AssociationStatus { get; set; }

        public List<CustomerStaticStatusModel> AssociationTypeList { get; set; }

        public List<AccountStaticStatusModel> AccountStatusList { get; set; }

        public List<SubscriberGroupViewModel> SubscribersList { get; set; }

        public List<CustomerRoleViewModel> RoleList { get; set; }

        public string Username { get; set; }
    }
}