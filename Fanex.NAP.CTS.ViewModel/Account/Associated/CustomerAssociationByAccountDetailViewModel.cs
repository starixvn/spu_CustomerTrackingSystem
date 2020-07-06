namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
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

        public string Subscriber { get; set; }

        public int AssociationType { get; set; }

        public string AssociationTypeName { get; set; }

        public int SharedDevices { get; set; }

        public int SharedIps { get; set; }

        public string AssociationDate { get; set; }

        public bool IsLinked { get; set; }
    }
}
