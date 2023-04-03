namespace Fanex.NAP.CTS.Model.Association
{
    using Newtonsoft.Json;

    public class AssociationOTGBModel
    {
        [JsonProperty(PropertyName = "TransID")]
        public long TransID { get; set; }

        [JsonProperty(PropertyName = "MatchID")]
        public long MatchID { get; set; }

        [JsonIgnore]
        public int Round { get; set; }
    }
}
