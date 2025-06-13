namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Model.AIInsights;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using System;
    using System.Collections.Generic;
    using System.Security.Policy;
    using System.Threading.Tasks;

    public class AIInsightsService : IAIInsightsService
    {
        public AIInsightsService()
        {
        }

        public AIInsightsAnalyzeResponseModel Analyze(AIInsightsAnalyzeRequestModel request, string url)
        {
            var param = new AIInsightsAnalyzeRequestModel
            {
                data_path = request.data_path,
                file_content = request.file_content,
                mode_type = request.mode_type,
                changing_column_definition = request.changing_column_definition
            };

            var result = ApiHelper.Post<AIInsightsAnalyzeResponseModel>(url, param, 120);

            Task.WhenAll(result);

            return result.Result;
        }
    }
}