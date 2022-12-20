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

        public string AssociationType1 { get; set; }

        public string AssociationType2 { get; set; }

        public string AgentAssociationType1 { get; set; }

        public string AgentAssociationType2 { get; set; }

        public long TransitiveCTSCustID { get; set; }

        public string TransitiveUsername { get; set; }

    }
}
