namespace Fanex.NAP.CTS.Model.AIInsights.Api
{
    public class AIInsightsStartAnalyzeChartsDataModel
    {
        public string status { get; set; }

        public string stream_token { get; set; }

        public string stream_url { get; set; }

        public string session_id { get; set; }

        public int session_batch_id { get; set; }

        public string message { get; set; }
    }
}
