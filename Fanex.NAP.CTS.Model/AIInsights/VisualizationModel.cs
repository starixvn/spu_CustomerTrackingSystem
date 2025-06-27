namespace Fanex.NAP.CTS.Model.AIInsights
{
    using System.Collections.Generic;

    public class VisualizationModel
    {
        public List<ChartInfoModel> chart_infos { get; set; }

        public Dictionary<string, string> column_definition_string { get; set; }

        public List<string> summary_list { get; set; }
    }
}
