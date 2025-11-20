using System;
using System.Collections.Generic;

namespace Fanex.NAP.CTS.Model.AIInsights.Api
{
    public class AIInsightsProcessAnalyzeChartsResponseModel
    {
        public string type { get; set; }

        public string chart_id { get; set; }

        public string current { get; set; }

        public string total { get; set; }

        public string percentage { get; set; }

        public DateTime? timestamp { get; set; }

        public string sesion_id { get; set; }

        public string general_idea { get; set; }

        public string message { get; set; }

        public string chart_data { get; set; }
    }

    public class AIInsightProcessAnalyzeChartsChartInfoModel
    {
        public string session_id { get; set; }

        public int session_batch_id { get; set; }

        public string file_name { get; set; }

        public string trace_back { get; set; }

        public string general_idea { get; set; }

        public string insights { get; set; }

        public string suggestions { get; set; }

        public string used_cols { get; set; }

        public string python_code { get; set; }

        public string base64_image { get;set; }

        public int chart_id { get; set; }
    }

    public enum AIProcessChartType
    {
        connected,
        ready,
        chart_ready,
        complete
    }
}
