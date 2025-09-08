namespace Fanex.NAP.CTS.Service.Implement.AIInsights
{
    using System.IO;
    using System.Net;
    using System.Web;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Model.AIInsights.Api;
    using Fanex.NAP.CTS.Service.AIInsights;
    using Fanex.NAP.CTS.Service.Interface.AIInsights;

    public class AIInsightsService : IAIInsightsService
    {
        private readonly IAIInsightsApiForwarderService _apiForwarderService;
        private AIInsightsFileUploadModel _aiInsightsFileUploadModel;

        public AIInsightsService()
        {
            _apiForwarderService = new AIInsightsApiForwarderService();
            _aiInsightsFileUploadModel = new AIInsightsFileUploadModel();
        }

        public AIInsightsAnalyzeChartsDataModel AnalyzeCharts(AIInsightsFileUploadDataModel model)
        {
            var request = new AIInsightsAnalyzeRequestModel
            {
                cache_key = model?.cache_key,
            };

            var result = _apiForwarderService.AnalyzeCharts(request, ConfigSettings.AIInsightsAnalyzeChartsUrl);

            ValidateResponse(result);
            return result.response;
        }

        public AIInsightsAnalyzeSummaryDataModel AnalyzeSummary(AIInsightsFileUploadDataModel model)
        {
            var request = new AIInsightsAnalyzeRequestModel
            {
                cache_key = model?.cache_key,
            };


            var result = _apiForwarderService.AnalyzeSummary(request, ConfigSettings.AIInsightsAnalyzeSummaryUrl);

            ValidateResponse(result);
            return result.response;
        }

        public AIInsightsChatbotDataModel AskChatbot(AIInsightsFileUploadDataModel model, string question)
        {
            var request = new AIInsightsChatbotRequestModel
            {
                cache_key = model?.cache_key,
                question = question
            };

            var result = _apiForwarderService.AskChatbot(request, ConfigSettings.AIInsightsChatBotUrl);

            ValidateResponse(result);
            return result.response;
        }

        public AIInsightsFileUploadResponseModel UploadFile(HttpPostedFileBase uploadFile, string session_id)
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

            var result = _apiForwarderService.UploadFile(request, ConfigSettings.AIInsightsUploadUrl);

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
