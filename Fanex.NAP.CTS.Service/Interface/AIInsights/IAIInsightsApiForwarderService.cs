namespace Fanex.NAP.CTS.Service.AIInsights
{
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Model.AIInsights.Api;

    public interface IAIInsightsApiForwarderService
    {
        AIInsightsFileUploadResponseModel UploadFile(AIInsightsFileUploadRequestModel request, string url);

        AIInsightsAnalyzeSummaryResponseModel AnalyzeSummary(AIInsightsAnalyzeRequestModel request, string url);

        AIInsightsAnalyzeChartsResponseModel AnalyzeCharts(AIInsightsAnalyzeRequestModel request, string url);

        AIInsightsChatbotResponseModel AskChatbot(AIInsightsChatbotRequestModel request, string url);
    }
}
