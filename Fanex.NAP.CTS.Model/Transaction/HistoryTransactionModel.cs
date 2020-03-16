namespace Fanex.NAP.CTS.Model.Transaction
{
    using System;

    public class HistoryTransactionModel
    {
        public long TransId { get; set; }

        public string TransTime { get; set; }

        public string DeviceId { get; set; }

        public string Ip { get; set; }

        public long IpId { get; set; }

        public int Flagged { get; set; }

        public string Action { get; set; }

        public string ActionResult { get; set; }

        public string OSName { get; set; }

        public string BrowserName { get; set; }

        public string Url { get; set; }
    }
}
