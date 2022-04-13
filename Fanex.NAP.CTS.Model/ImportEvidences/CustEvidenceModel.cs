namespace Fanex.NAP.CTS.Model.ImportEvidences
{
    public class CustEvidenceModel
    {
        public long CustId { get; set; }

        public long CTSCustId { get; set; }

        public int SubscriberId { get; set; }

        public int EvidenceId { get; set; }

        public int ActionType { get; set; }
    }
}