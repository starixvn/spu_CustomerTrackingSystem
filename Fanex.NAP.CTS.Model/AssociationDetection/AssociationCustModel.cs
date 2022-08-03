namespace Fanex.NAP.CTS.Model.AssociationDetection
{
    using Newtonsoft.Json;

    public class AssociationCustModel
    {
        [JsonProperty(PropertyName = "F")]
        public int FromtCustID { get; set; }

        [JsonProperty(PropertyName = "T")]
        public int ToCustID { get; set; }
    }
}
