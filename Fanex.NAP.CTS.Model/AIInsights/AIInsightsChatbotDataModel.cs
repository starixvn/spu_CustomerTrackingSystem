namespace Fanex.NAP.CTS.Model.AIInsights
{
    using System.Collections.Generic;

    public class AIInsightsChatbotDataModel
    {
        public string answer { get; set; }

        public List<ChartInfoModel> chart_infos { get; set; }
    }
}