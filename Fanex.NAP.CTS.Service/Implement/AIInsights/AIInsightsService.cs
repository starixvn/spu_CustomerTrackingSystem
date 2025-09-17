namespace Fanex.NAP.CTS.Service.Implement.AIInsights
{
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Model.AIInsights.Api;
    using Fanex.NAP.CTS.Service.AIInsights;
    using Fanex.NAP.CTS.Service.Interface.AIInsights;
    using Newtonsoft.Json;

    public class AIInsightsService : IAIInsightsService
    {
        private readonly IAIInsightsApiForwarderService _apiForwarderService;
        private AIInsightsFileUploadModel _aiInsightsFileUploadModel;

        public AIInsightsService()
        {
            _apiForwarderService = new AIInsightsApiForwarderService();
            _aiInsightsFileUploadModel = new AIInsightsFileUploadModel();
        }

        public async Task<AIInsightsAnalyzeChartsDataModel> AnalyzeCharts(AIInsightsFileUploadDataModel model)
        {
            var request = new AIInsightsAnalyzeRequestModel
            {
                cache_key = model?.cache_key,
            };

            WriteLog($"Request: {nameof(AnalyzeCharts)}", request);

            var result = await _apiForwarderService.AnalyzeCharts(request, ConfigSettings.AIInsightsAnalyzeChartsUrl);

            WriteLog($"Response: {nameof(AnalyzeCharts)}", result);

            ValidateResponse(result);

            return result.response;
        }

        public async Task<AIInsightsAnalyzeSummaryDataModel> AnalyzeSummary(AIInsightsFileUploadDataModel model)
        {
            var request = new AIInsightsAnalyzeRequestModel
            {
                cache_key = model?.cache_key,
            };

            WriteLog($"Request: {nameof(AnalyzeSummary)}", request);

            var result = await _apiForwarderService.AnalyzeSummary(request, ConfigSettings.AIInsightsAnalyzeSummaryUrl);

            WriteLog($"Response: {nameof(AnalyzeSummary)}", result);

            ValidateResponse(result);
            return result.response;
        }

        public async Task<AIInsightsChatbotDataModel> AskChatbot(AIInsightsFileUploadDataModel model, string question)
        {
            var request = new AIInsightsChatbotRequestModel
            {
                cache_key = model?.cache_key,
                question = question
            };

            WriteLog($"Request: {nameof(AskChatbot)}", request);

            var result = await _apiForwarderService.AskChatbot(request, ConfigSettings.AIInsightsChatBotUrl);

            WriteLog($"Response: {nameof(AskChatbot)}", result);

            ValidateResponse(result);
            return result.response;
        }

        public async Task<AIInsightsFileUploadResponseModel> UploadFile(HttpPostedFileBase uploadFile, string session_id)
        {
            if (uploadFile == null || uploadFile.ContentLength == 0)
            {
                throw new InvalidDataException($"Content cannot be empty.");
            }

            if (uploadFile.ContentLength > _aiInsightsFileUploadModel.MaxFileSizeBytes)
            {
                throw new InvalidDataException($"Exceeds the allowed limit of {_aiInsightsFileUploadModel.MaxFileSizeBytes / (1024 * 1024)} MB.");
            }

            var request = new AIInsightsFileUploadRequestModel
            {
                file_base64image = uploadFile.FileBaseToBase64StringChunked(),
                file_name = Path.GetFileName(uploadFile.FileName),
                session_id = session_id
            };

            WriteLog($"Request: {nameof(UploadFile)}", $"{request.session_id}_{request.file_name}");

            var result = await _apiForwarderService.UploadFile(request, ConfigSettings.AIInsightsUploadUrl);

            WriteLog($"Response: {nameof(UploadFile)}", result);

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

        private void WriteLog(string functionName, object data)
        {
            if (ConfigSettings.IsWriteLogInfoAIInsight)
            {
                var jsonData = data != null ? JsonConvert.SerializeObject(data) : "null";
                LogExtension.LogInfo($"CTS - AI Insights {functionName}: {jsonData}");
            }
        }
    }
}
