namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using System;

    public class GraphCustomerItemViewModel
    {
        public GraphCustomerItemViewModel() { }

        public long CTSCustId { get; set; }

        public long CustId { get; set; }

        public string Username { get; set; }

        public string DisplayName { get; set; }
      
        public string ItemCode { get; set; }

        public int AssociationType { get; set; }

        public string AssociationTypeName { get; set; }

        public int AssociationStatus { get; set; }

        public string AccountStatusIds { get; set; }

        public string CategoryIds { get; set; }

        public string SubscriberIds { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }        

        public GraphCustomerItemViewModel[] children { get; set; }     
    }
}