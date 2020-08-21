namespace Fanex.NAP.CTS.Model.Account
{
    using System;

    public class CustomerAssociationByAccountDetailModel
    {
        public int AffectedTag { get; set; }

        public int FlaggedTag { get; set; }

        public long CTSCustId { get; set; }

        public long CustId { get; set; }

        public int SubscriberId { get; set; }

        public string Username { get; set; }

        public string LoginId { get; set; }

        public string Status { get; set; }

        public int StatusId { get; set; }

        public string Subscriber { get; set; }

        public int AssociationType { get; set; }

        public string AssociationTypeName { get; set; }

        public int SharedDevices { get; set; }

        public int SharedIps { get; set; }

        public DateTime AssociationDate { get; set; }
    }
}
