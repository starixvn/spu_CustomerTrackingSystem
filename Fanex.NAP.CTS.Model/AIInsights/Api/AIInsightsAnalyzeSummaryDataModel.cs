namespace Fanex.NAP.CTS.Model.AIInsights.Api
{
    using System.Collections.Generic;

    public class AIInsightsAnalyzeSummaryDataModel
    {
        public List<string> summary { get; set; }

        public string statistics_html { get; set; }

        public string status { get; set; }
    }
}
