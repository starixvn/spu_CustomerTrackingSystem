namespace Fanex.NAP.CTS.Model.AIInsights
{
    using System.Collections.Generic;

    public class ChartInfoModel
    {
        public string analysis { get; set; }

        public object base64_image { get; set; }

        public string chart_path { get; set; }

        public string code { get; set; }

        public string name { get; set; }

        public List<string> suggestions { get; set; }

        public string title { get; set; }
    }
}
