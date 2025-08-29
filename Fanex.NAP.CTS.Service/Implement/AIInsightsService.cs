namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model.AIInsights;
    using System.Configuration;
    using System.Net;
    using System.Threading.Tasks;

    public class AIInsightsService : IAIInsightsService
    {
        public AIInsightsAnalyzeResponseModel Analyze(AIInsightsAnalyzeRequestModel param, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new AIInsightsAnalyzeResponseModel();
            }

            var response = ApiHelper.PostWithStreamWritter<AIInsightsAnalyzeResponseModel>(url, param, ConfigSettings.AIAnalyzeRequestTimeout);

            Task.WhenAll(response);

            var result = response.Result;
            ValidateResponse(result);

            return result;
        }

        public AIInsightsChatbotResponseModel AskChatbot(AIInsightsChatbotRequestModel request, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new AIInsightsChatbotResponseModel();
            }

            var response = ApiHelper.PostWithStreamWritter<AIInsightsChatbotResponseModel>(url, request, ConfigSettings.ChatbotRequestTimeout);

            Task.WhenAll(response);

            var result = response.Result;
            ValidateResponse(result);

            return result;
        }

        private void ValidateResponse(AIInsightResponseModel response)
        {
            if (response != null && response.status_code > 299)
            {
                throw new HttpListenerException(response.status_code, response.error);
            }
        }
    }
}