namespace Fanex.NAP.CTS.Service.AIInsights
{
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Model.AIInsights.Api;
    using System.Threading.Tasks;

    public class AIInsightsApiForwarderService : IAIInsightsApiForwarderService
    {
        public AIInsightsAnalyzeChartsResponseModel AnalyzeCharts(AIInsightsAnalyzeRequestModel request, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new AIInsightsAnalyzeChartsResponseModel();
            }

            var response = ApiHelper.Post<AIInsightsAnalyzeChartsResponseModel>(url, request, ConfigSettings.AIAnalyzeRequestTimeout);

            Task.WhenAll(response);

            return response.Result;
        }

        public AIInsightsAnalyzeSummaryResponseModel AnalyzeSummary(AIInsightsAnalyzeRequestModel request, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new AIInsightsAnalyzeSummaryResponseModel();
            }

            var response = ApiHelper.Post<AIInsightsAnalyzeSummaryResponseModel>(url, request, ConfigSettings.AIAnalyzeRequestTimeout);

            Task.WhenAll(response);

            return response.Result;
        }

        public AIInsightsChatbotResponseModel AskChatbot(AIInsightsChatbotRequestModel request, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new AIInsightsChatbotResponseModel();
            }

            var response = ApiHelper.Post<AIInsightsChatbotResponseModel>(url, request, ConfigSettings.ChatbotRequestTimeout);

            Task.WhenAll(response);

            return response.Result;
        }

        public AIInsightsFileUploadResponseModel UploadFile(AIInsightsFileUploadRequestModel request, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new AIInsightsFileUploadResponseModel();
            }

            var response = ApiHelper.PostWithStreamWritter<AIInsightsFileUploadResponseModel>(url, request, ConfigSettings.AIAnalyzeRequestTimeout);

            Task.WhenAll(response);

            return response.Result;
        }
    }
}