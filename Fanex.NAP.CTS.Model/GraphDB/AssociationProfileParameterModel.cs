namespace Fanex.NAP.CTS.Model.GraphDB
{
    public class AssociationProfileParameterModel
    {
        public long CTSCustId { get; set; }

        public int AssociationType { get; set; }

        public int AssociationStatus { get; set; }

        public string AccountStatusIds { get; set; }

        public string SubscriberIDs { get; set; }

        public string CategoryIDs { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}