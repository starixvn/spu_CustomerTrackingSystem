namespace Fanex.NAP.CTS.Model.AIInsights
{
    public class AIInsightsAnalyzeResponseModel : AIInsightResponseModel
    {
        public AIInsightsAnalyzeResponseModel()
        {
            response = new AIInsightsAnalyzeDataModel();
        }

        public AIInsightsAnalyzeDataModel response { get; set; }

    }
}
