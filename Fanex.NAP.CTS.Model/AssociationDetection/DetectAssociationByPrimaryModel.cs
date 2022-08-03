namespace Fanex.NAP.CTS.Model.AssociationDetection
{
    using Newtonsoft.Json;

    public class DetectAssociationByPrimaryModel
    {
        [JsonProperty(PropertyName = "F")]
        public long FromCTSCustID { get; set; }

        [JsonProperty(PropertyName = "T")]
        public long ToCTSCustID { get; set; }

        [JsonProperty(PropertyName = "AT")]
        public int AssociationType { get; set; }
    }
}
