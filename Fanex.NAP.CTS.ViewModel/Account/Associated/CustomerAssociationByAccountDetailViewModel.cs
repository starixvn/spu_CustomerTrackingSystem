namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using System.Collections.Generic;

    public class CustomerAssociationByAccountDetailViewModel
    {       
        public long CTSCustId { get; set; }

        public long CustId { get; set; }

        public int SubscriberId { get; set; }

        public string Username { get; set; }

        public string LoginId { get; set; }

        public bool IsAffectedTag { get; set; }

        public bool IsFlaggedTag { get; set; }

        public string Status { get; set; }

        public int StatusId { get; set; }

        public int SiteId { get; set; }

        public string SiteName { get; set; }

        public string SubscriberName { get; set; }

        public List<int> AssociationType { get; set; }

        public string AssociationTypeName { get; set; }

        public bool IsProblemAccount { get; set; }

        public string CategoryIds { get; set; }

        public string CategoryNames { get; set; }

        public int CategoryType { get; set; }

        public string CustomerClasses { get; set; }

        public string AssociationDate { get; set; }

        public bool IsLinked { get; set; }

        public bool IsPAProbation { get; set; }
        
        public string GroupIDs { get; set; }

        public string OriginGroupIDs { get; set; }
    }
}
