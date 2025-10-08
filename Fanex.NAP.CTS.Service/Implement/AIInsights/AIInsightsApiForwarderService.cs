namespace Fanex.NAP.CTS.Service.AIInsights
{
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Model.AIInsights.Api;
    using System.Threading;
    using System.Threading.Tasks;

    public class AIInsightsApiForwarderService : IAIInsightsApiForwarderService
    {
        public async Task<AIInsightsAnalyzeChartsResponseModel> AnalyzeCharts(AIInsightsAnalyzeRequestModel request, string url, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new AIInsightsAnalyzeChartsResponseModel();
            }

            var response = await ApiWithHttpClientRotatorHelper.Post<AIInsightsAnalyzeChartsResponseModel>(url, request, ConfigSettings.AIAnalyzeRequestTimeout);

            return response;
        }

        public async Task<AIInsightsAnalyzeSummaryResponseModel> AnalyzeSummary(AIInsightsAnalyzeRequestModel request, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new AIInsightsAnalyzeSummaryResponseModel();
            }

            var response = await ApiWithHttpClientRotatorHelper.Post<AIInsightsAnalyzeSummaryResponseModel>(url, request, ConfigSettings.AIAnalyzeRequestTimeout);

            return response;
        }

        public async Task<AIInsightsChatbotResponseModel> AskChatbot(AIInsightsChatbotRequestModel request, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new AIInsightsChatbotResponseModel();
            }

            var response = await ApiWithHttpClientRotatorHelper.Post<AIInsightsChatbotResponseModel>(url, request, ConfigSettings.ChatbotRequestTimeout);

            return response;
        }

        public async Task<AIInsightsFileUploadResponseModel> UploadFile(AIInsightsFileUploadRequestModel request, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new AIInsightsFileUploadResponseModel();
            }

            var response = await ApiWithHttpClientRotatorHelper.PostWithStreamWritter<AIInsightsFileUploadResponseModel>(url, request, ConfigSettings.AIAnalyzeRequestTimeout);

            return response;
        }
    }
}