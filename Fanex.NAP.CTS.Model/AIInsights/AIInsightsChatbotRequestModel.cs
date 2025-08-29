namespace Fanex.NAP.CTS.Model.AIInsights
{
    using System.Text;

    public class AIInsightsChatbotRequestModel
    {
        public string session_id { get; set; }

        public string question { get; set; }

        public string file_name { get; set; }

        public StringBuilder file_content { get; set; }
    }
}