namespace Fanex.NAP.CTS.Service.Interface.AIInsights
{
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Model.AIInsights.Api;

    public interface IAIInsightsService
    {
        Task<AIInsightsFileUploadResponseModel> UploadFile(HttpPostedFileBase uploadFile, string session_id);

        Task<AIInsightsAnalyzeSummaryDataModel> AnalyzeSummary(AIInsightsFileUploadDataModel model);

        Task<AIInsightsAnalyzeChartsDataModel> AnalyzeCharts(AIInsightsFileUploadDataModel model);

        Task<AIInsightsChatbotDataModel> AskChatbot(AIInsightsFileUploadDataModel model, string question);
    }
}
