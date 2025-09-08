namespace Fanex.NAP.CTS.Service.Interface.AIInsights
{
    using System.Web;
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Model.AIInsights.Api;

    public interface IAIInsightsService
    {
        AIInsightsFileUploadResponseModel UploadFile(HttpPostedFileBase uploadFile, string session_id);

        AIInsightsAnalyzeSummaryDataModel AnalyzeSummary(AIInsightsFileUploadDataModel model);

        AIInsightsAnalyzeChartsDataModel AnalyzeCharts(AIInsightsFileUploadDataModel model);

        AIInsightsChatbotDataModel AskChatbot(AIInsightsFileUploadDataModel model, string question);
    }
}
