namespace Fanex.NAP.CTS.Model.AIInsights
{
    using System.Text;

    public class AIInsightsAnalyzeRequestModel
    {
        public string file_name { get; set; }

        public StringBuilder file_content { get; set; }

        public int mode_type { get; set; }

        public string changing_column_definition { get; set; }

        public string session_id { get; set; }

        public string language { get; set; }
    }
}
