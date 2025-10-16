namespace Fanex.NAP.CTS.Model.AIInsights
{
    using Fanex.NAP.CTS.Model.AIInsights.Api;
    using System.Collections.Generic;

    public class ChartInfoModel
    {
        public ChartInfoModel() { }
        public ChartInfoModel(AIInsightProcessAnalyzeChartsChartInfoModel model)
        {
            analysis = model.insights;
            title = model.general_idea;
            suggestions = model.suggestions.Split('\n');
            base64_image = model.base64_image;
        }

        public string analysis { get; set; }

        public object base64_image { get; set; }

        public string chart_path { get; set; }

        public string code { get; set; }

        public string name { get; set; }

        public IEnumerable<string> suggestions { get; set; }

        public string title { get; set; }
    }
}
