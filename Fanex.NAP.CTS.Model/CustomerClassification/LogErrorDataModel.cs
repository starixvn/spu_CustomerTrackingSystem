namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using Newtonsoft.Json;

    public class LogErrorDataModel
    {
        [JsonProperty("SpName")]
        public string SPName { get; set; }

        [JsonProperty("Parameters")]
        public object Parameters { get; set; }

        [JsonProperty("Message")]
        public object Message { get; set; }

        [JsonProperty("FromFlow")]
        public string FromFlow { get; set; }
    }
}