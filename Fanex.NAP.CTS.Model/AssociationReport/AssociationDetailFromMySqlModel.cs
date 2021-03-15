namespace Fanex.NAP.CTS.Model.AssociationReport
{
    using System;

    public class AssociationDetailFromMySqlModel
    {
        public int CTSCustID { get; set; }

        public string UserName { get; set; }

        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public string AccountStatus { get; set; }

        public DateTime AssociationDate { get; set; }

        public bool AssociationStatus { get; set; }

        public int AssociationType { get; set; }        
    }
}
