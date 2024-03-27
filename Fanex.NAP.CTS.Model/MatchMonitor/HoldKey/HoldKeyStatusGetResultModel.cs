namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System.Collections.Generic;
    using System.Linq;

    public class HoldKeyStatusGetResultModel
    {
        public HoldKeyStatusGetResultModel()
        {
            isSuccess = false;
            msg = string.Empty;
            holdKeyInfo = Enumerable.Empty<HoldKeyStatusGetModel>();
        }

        public bool isSuccess { get; set; }

        public string msg { get; set; }

        public IEnumerable<HoldKeyStatusGetModel> holdKeyInfo { get; set; }
    }
}
