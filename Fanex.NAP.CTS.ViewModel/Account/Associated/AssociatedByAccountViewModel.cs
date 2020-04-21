using Fanex.NAP.CTS.Model.Account;
using System.Collections.Generic;

namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    public class AssociatedByAccountViewModel
    {
        public List<CustomerStaticStatusModel> AssociationStatus { get; set; }

        public List<CustomerStaticStatusModel> AssociationTypeList { get; set; }

        public string Username { get; set; }
    }
}
