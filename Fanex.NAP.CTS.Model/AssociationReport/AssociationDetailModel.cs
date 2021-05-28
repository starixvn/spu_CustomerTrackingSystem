namespace Fanex.NAP.CTS.Model.AssociationReport
{
    using System;
    using System.Collections.Generic;

    public class AssociationDetailModel
    {
        public int CTSCustId { get; set; }

        public string UserName { get; set; }

        public int SubscriberId { get; set; }

        public string SubscriberName { get; set; }

        public string StatusName { get; set; }

        public DateTime AssociationDateFromGraphDB { get; set; }

        public string AssociationDateString { get; set; }

        public int AssociationStatus { get; set; }

        public List<int> AssociationType { get; set; }
    }
}
