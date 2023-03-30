using Newtonsoft.Json;

namespace Fanex.NAP.CTS.Model.AssociationDetection
{   
    public class SharedIPInfoModel
    {
        [JsonProperty(PropertyName = "F1")]
        public int FromCustID { get; set; }

        [JsonProperty(PropertyName = "F2")]
        public long FromCTSCustID { get; set; }

        [JsonProperty(PropertyName = "F3")]
        public string FromUserName { get; set; }

        [JsonProperty(PropertyName = "T1")]
        public int ToCustID { get; set; }

        [JsonProperty(PropertyName = "T2")]
        public long ToCTSCustID { get; set; }

        [JsonProperty(PropertyName = "T3")]
        public string ToUserName { get; set; }

        [JsonProperty(PropertyName = "I")]
        public string IP { get; set; }
    }
}
