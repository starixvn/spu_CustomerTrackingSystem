namespace Fanex.NAP.CTS.Model.AIInsights
{
    public class AIInsightsAnalyzeDataModel
    {
        public string data_definition_html { get; set; }

        public string statistics_html { get; set; }

        public VisualizationModel visualization_response { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
