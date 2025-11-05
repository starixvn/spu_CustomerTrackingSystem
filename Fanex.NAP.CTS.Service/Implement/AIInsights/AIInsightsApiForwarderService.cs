namespace Fanex.NAP.CTS.Service.AIInsights
{
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Model.AIInsights.Api;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class AIInsightsApiForwarderService : IAIInsightsApiForwarderService
    {
        public async Task<AIInsightsStartAnalyzeChartsResponseModel> StartAnalyzeChartsAsync(AIInsightsAnalyzeRequestModel request, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new AIInsightsStartAnalyzeChartsResponseModel();
            }

            var response = await ApiWithHttpClientRotatorHelper.PostAsync<AIInsightsStartAnalyzeChartsResponseModel>(url, request, ConfigSettings.AIAnalyzeRequestTimeout);

            return response;
        }

        public async Task<AIInsightsAnalyzeSummaryResponseModel> AnalyzeSummary(AIInsightsAnalyzeRequestModel request, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new AIInsightsAnalyzeSummaryResponseModel();
            }

            var response = await ApiWithHttpClientRotatorHelper.PostAsync<AIInsightsAnalyzeSummaryResponseModel>(url, request, ConfigSettings.AIAnalyzeRequestTimeout);

            return response;
        }

        public async Task<AIInsightsChatbotResponseModel> AskChatbot(AIInsightsChatbotRequestModel request, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new AIInsightsChatbotResponseModel();
            }

            var response = await ApiWithHttpClientRotatorHelper.PostAsync<AIInsightsChatbotResponseModel>(url, request, ConfigSettings.ChatbotRequestTimeout);

            return response;
        }

        public async Task<AIInsightsFileUploadResponseModel> UploadFile(Func<HttpContent> createContent, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new AIInsightsFileUploadResponseModel();
            }

            var response = await ApiWithHttpClientRotatorHelper.PostWithContentAsync<AIInsightsFileUploadResponseModel>(url, createContent, ConfigSettings.AIAnalyzeRequestTimeout);

            return response;
        }
    }
}