namespace Fanex.NAP.CTS.Model.CustomerClassification.ProblemAccount
{
    using Newtonsoft.Json;

    public class PADailyScanFromQueueModel
    {
        public long CustID { get; set; }

        [JsonProperty("SportGroup")]
        public int DWSportType { get; set; }
    }
}
