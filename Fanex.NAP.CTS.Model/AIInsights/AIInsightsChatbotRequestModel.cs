namespace Fanex.NAP.CTS.Model.AIInsights
{
    public class AIInsightsChatbotRequestModel
    {
        public string session_id { get; set; }

        public string question { get; set; }

        public string file_name { get; set; }

        public string file_content { get; set; }
    }
}