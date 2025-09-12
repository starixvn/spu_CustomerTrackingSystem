namespace Fanex.NAP.CTS.Service.AIInsights
{
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Model.AIInsights.Api;
    using System.Threading.Tasks;

    public interface IAIInsightsApiForwarderService
    {
        Task<AIInsightsFileUploadResponseModel> UploadFile(AIInsightsFileUploadRequestModel request, string url);

        Task<AIInsightsAnalyzeSummaryResponseModel> AnalyzeSummary(AIInsightsAnalyzeRequestModel request, string url);

        Task<AIInsightsAnalyzeChartsResponseModel> AnalyzeCharts(AIInsightsAnalyzeRequestModel request, string url);

        Task<AIInsightsChatbotResponseModel> AskChatbot(AIInsightsChatbotRequestModel request, string url);
    }
}
