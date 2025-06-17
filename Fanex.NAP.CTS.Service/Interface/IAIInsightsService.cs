namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.AIInsights;

    public interface IAIInsightsService
    {
        AIInsightsAnalyzeResponseModel Analyze(AIInsightsAnalyzeRequestModel request, string url);

        AIInsightsChatbotResponseModel AskChatbot(AIInsightsChatbotRequestModel request, string url, string userId);
    }
}
