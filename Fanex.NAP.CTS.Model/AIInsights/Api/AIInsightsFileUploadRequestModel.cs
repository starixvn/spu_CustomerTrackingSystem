namespace Fanex.NAP.CTS.Model.AIInsights
{
    using System.Text;

    public class AIInsightsFileUploadRequestModel
    {
        public StringBuilder file_base64image { get; set; }

        public string file_name { get; set; }

        public string session_id { get; set; }
    }
}