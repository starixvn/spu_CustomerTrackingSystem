namespace Fanex.NAP.CTS.Service.Interface.AIInsights
{
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Model.AIInsights.Api;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    public interface IAIInsightsService
    {
        Task<AIInsightsFileUploadResponseModel> UploadFile(HttpPostedFileBase uploadFile, string session_id);

        Task<AIInsightsAnalyzeSummaryResponseModel> AnalyzeSummary(AIInsightsFileUploadDataModel model);

        Task AnalyzeChartsSseStream(AIInsightsFileUploadDataModel model, HttpContextBase httpContext, CancellationToken cancellationToken);

        Task<AIInsightsChatbotDataModel> AskChatbot(AIInsightsFileUploadDataModel model, string question);
    }
}
