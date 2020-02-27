using System;

namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    public class GraphCustomerItemViewModel
    {
        public GraphCustomerItemViewModel() { }

        public long CTSCustId { get; set; }

        public Int64 CustId { get; set; }

        public string Username { get; set; }

        public string DisplayName { get; set; }
      
        public string ItemCode { get; set; }

        public int AssociationType { get; set; }

        public string AssociationTypeName { get; set; }

        public GraphCustomerItemViewModel[] children { get; set; }     
    }
}