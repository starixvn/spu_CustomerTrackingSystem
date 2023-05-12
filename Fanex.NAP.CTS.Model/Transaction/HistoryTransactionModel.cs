namespace Fanex.NAP.CTS.Model.Transaction
{
    using System;

    public class HistoryTransactionModel
    {        
        public long TransId { get; set; }

        public DateTime TransTime { get; set; }
                
        public string FirstDeviceCode { get; set; }

        public string Ip { get; set; }        

        public string Flagged { get; set; }

        public string Action { get; set; }

        public string ActionResult { get; set; }

        public string OS { get; set; }

        public string Browser { get; set; }

        public string UrlDetails { get; set; }
    }
}
