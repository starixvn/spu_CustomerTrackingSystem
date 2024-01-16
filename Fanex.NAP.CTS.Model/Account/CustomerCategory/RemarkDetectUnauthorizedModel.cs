namespace Fanex.NAP.CTS.Model.Account
{
    using System;

    public class RemarkDetectUnauthorizedModel
    {
        public DateTime FirstBetDate { get; set; }

        public DateTime DetectionDate { get; set; }

        public int Bot { get; set; }

        public int InvalidBrowser { get; set; }

        public double BotPCT { get; set; }

        public double InvalidBrowserPCT { get; set; }
    }
}
