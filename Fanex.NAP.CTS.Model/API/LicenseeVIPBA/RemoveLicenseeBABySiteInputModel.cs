namespace Fanex.NAP.CTS.Model.API
{
    using Newtonsoft.Json;

    public class RemoveLicenseeBABySiteInputModel
    {
        [JsonProperty(PropertyName = "SiteName")]
        public string SiteName { get; set; }

        [JsonProperty(PropertyName = "CreatedBy")]
        public long? CreatedBy { get; set; }
    }
}