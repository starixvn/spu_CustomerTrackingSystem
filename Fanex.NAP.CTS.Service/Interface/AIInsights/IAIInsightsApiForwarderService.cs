namespace Fanex.NAP.CTS.Service.AIInsights
{
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Model.AIInsights.Api;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IAIInsightsApiForwarderService
    {
        Task<AIInsightsFileUploadResponseModel> UploadFile(Func<HttpContent> createContent, string url);

        Task<AIInsightsAnalyzeSummaryResponseModel> AnalyzeSummary(AIInsightsAnalyzeRequestModel request, string url);

        Task<AIInsightsStartAnalyzeChartsResponseModel> StartAnalyzeChartsAsync(AIInsightsAnalyzeRequestModel request, string url);

        Task<AIInsightsChatbotResponseModel> AskChatbot(AIInsightsChatbotRequestModel request, string url);
    }
}
