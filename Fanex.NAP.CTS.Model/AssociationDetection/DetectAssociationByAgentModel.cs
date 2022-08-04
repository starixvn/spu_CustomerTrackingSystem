namespace Fanex.NAP.CTS.Model.AssociationDetection
{
    using Newtonsoft.Json;

    public class DetectAssociationByAgentModel
    {
        [JsonProperty(PropertyName = "F")]
        public long FromID { get; set; }

        [JsonProperty(PropertyName = "T")]
        public long ToID { get; set; }
    }
}
