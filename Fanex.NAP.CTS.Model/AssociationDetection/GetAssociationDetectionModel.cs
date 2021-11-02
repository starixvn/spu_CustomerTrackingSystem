namespace Fanex.NAP.CTS.Model.AssociationDetection
{
    public class GetAssociationDetectionModel
    {
        public int GroupId { get; set; }

        public string UserName { get; set; }

        public string CustStatus { get; set; }

        public string Evidence { get; set; }

        public string CustomerCategory { get; set; }

        public string CategoryIds { get; set; }

        public long CTSCustId { get; set; }

        public long CustId { get; set; }

        public int CustSubId { get; set; }

        public string ParentIds { get; set; }
    }
}
