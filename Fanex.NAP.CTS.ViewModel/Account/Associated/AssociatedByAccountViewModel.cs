using Fanex.NAP.CTS.Model.Account;
using System.Collections.Generic;

namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    public class AssociatedByAccountViewModel
    {
        public List<CustomerStaticStatusModel> AccountStatusList { get; set; }

        public List<CustomerStaticStatusModel> AssociationTypeList { get; set; }

        public List<CustomerInfoDetailModel> CustomerInfoDetailList { get; set; }

        public int Status { get; set; }

        public int AssociationType { get; set; }
    }
}
