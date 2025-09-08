namespace Fanex.NAP.CTS.Model.AIInsights.Api
{
    public class AIInsightsFileUploadDataModel
    {
        public string cache_key { get; set; }

        public string session_id { get; set; }

        public string file_name { get; set; }

        public int file_size { get; set; }

        public long execution_time { get; set; }
    }
}
