namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Model.AIInsights;
    using System.Configuration;
    using System.Threading.Tasks;

    public class AIInsightsService : IAIInsightsService
    {
        public AIInsightsService()
        {
        }

        public AIInsightsAnalyzeResponseModel Analyze(AIInsightsAnalyzeRequestModel param, string url)
        {           
            var result = ApiHelper.Post<AIInsightsAnalyzeResponseModel>(url, param, 120);

            Task.WhenAll(result);

            return result.Result;
        }

        public AIInsightsChatbotResponseModel AskChatbot(AIInsightsChatbotRequestModel request, string url)
        {
            var chatbotRequestTimeout = ConfigurationManager.AppSettings["ChatbotRequestTimeout"];

            var timeout = int.TryParse(chatbotRequestTimeout, out var parsed) ? parsed : 30;

            var result = ApiHelper.Post<AIInsightsChatbotResponseModel>(url, request, timeout);

            Task.WhenAll(result);

            return result.Result;
        }
    }
}