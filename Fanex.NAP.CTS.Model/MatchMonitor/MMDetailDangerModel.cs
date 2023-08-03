namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using Newtonsoft.Json;   

    public class MMDetailDangerModel
    {
        [JsonProperty(PropertyName = "Danger1")]
        public int Danger1 { get; set; }

        [JsonProperty(PropertyName = "Danger2")]
        public int Danger2 { get; set; }

        [JsonProperty(PropertyName = "Danger3")]
        public int Danger3 { get; set; }

        [JsonProperty(PropertyName = "Danger4")]
        public int Danger4 { get; set; }

        [JsonProperty(PropertyName = "Danger5")]
        public int Danger5 { get; set; }

    }
}
