namespace Fanex.NAP.CTS.ViewModel.AssociationReport
{
    using System;

    public class AssociationDetailViewModel
    {
        public int CTSCustId { get; set; }

        public string UserName { get; set; }

        public int SubscriberId { get; set; }        

        public string SubscriberName { get; set; }

        public string AccountStatus { get; set; }

        public DateTime AssociatedDate { get; set; }

        public string AssociatedDateString { get; set; }

        public bool IsLinked { get; set; }

        public string AssociationTypeName { get; set; }

        public int TotalAssociationAccount { get; set; }
    }
}
