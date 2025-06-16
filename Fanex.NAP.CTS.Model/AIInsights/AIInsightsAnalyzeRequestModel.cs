namespace Fanex.NAP.CTS.Model.AIInsights
{
    public class AIInsightsAnalyzeRequestModel
    {
        public string file_name { get; set; }

        public string file_content { get; set; }

        public int mode_type { get; set; }

        public string changing_column_definition { get; set; }

        public string session_id { get; set; }

        public string language { get; set; }
    }
}
