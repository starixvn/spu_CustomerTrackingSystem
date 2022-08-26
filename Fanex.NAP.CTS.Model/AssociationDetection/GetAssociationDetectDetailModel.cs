namespace Fanex.NAP.CTS.Model.AssociationDetection
{
    public class GetAssociationDetectDetailModel
    {
        public long FromCTSCustID { get; set; }

        public int FromCustID { get; set; }

        public string FromUsername { get; set; }

        public string FromAgentUsername { get; set; }

        public long ToCTSCustID { get; set; }

        public int ToCustID { get; set; }

        public string ToUsername { get; set; }

        public string ToAgentUsername { get; set; }

        public string AssociationType { get; set; }

        public string AgentAssociationType { get; set; }

    }
}
