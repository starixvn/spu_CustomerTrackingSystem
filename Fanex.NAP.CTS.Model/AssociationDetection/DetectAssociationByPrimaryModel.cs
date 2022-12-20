namespace Fanex.NAP.CTS.Model.AssociationDetection
{
    using Newtonsoft.Json;

    public class DetectAssociationByPrimaryModel
    {
        [JsonProperty(PropertyName = "O")]
        public long OrigID { get; set; }

        [JsonProperty(PropertyName = "D")]
        public long DestID { get; set; }

        [JsonProperty(PropertyName = "A")]
        public int AssociationType { get; set; }
    }
}
