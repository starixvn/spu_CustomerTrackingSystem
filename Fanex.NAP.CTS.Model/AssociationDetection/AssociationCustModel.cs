namespace Fanex.NAP.CTS.Model.AssociationDetection
{
    using Newtonsoft.Json;

    public class AssociationCustModel
    {
        [JsonProperty(PropertyName = "F")]
        public long FromCTSCustID { get; set; }

        [JsonProperty(PropertyName = "F1")]
        public int FromCustID { get; set; }

        [JsonProperty(PropertyName = "X")]
        public string FromUserName { get; set; }

        [JsonProperty(PropertyName = "T")]
        public long ToCTSCustID { get; set; }

        [JsonProperty(PropertyName = "T1")]
        public int ToCustID { get; set; }

        [JsonProperty(PropertyName = "Y")]
        public string ToUserName { get; set; }
    }
}
