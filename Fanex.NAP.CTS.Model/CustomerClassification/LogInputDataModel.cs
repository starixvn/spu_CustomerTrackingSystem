namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using Newtonsoft.Json;

    public class LogInputDataModel
    {
        [JsonProperty("CustID")]
        public long? CustId { get; set; }

        [JsonProperty("InputValue")]
        public object InputValue { get; set; }
    }
}