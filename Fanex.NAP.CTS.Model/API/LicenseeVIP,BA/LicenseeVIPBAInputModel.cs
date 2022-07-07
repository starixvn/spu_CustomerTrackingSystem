namespace Fanex.NAP.CTS.Model.API
{
    using Newtonsoft.Json;

    public class LicenseeVIPBAInputModel
    {
        [JsonProperty(PropertyName = "CustIDs")]
        public string CustIDs { get; set; }

        [JsonProperty(PropertyName = "CreatedBy")]
        public long CreatedBy { get; set; }
    }
}