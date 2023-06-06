namespace Fanex.NAP.CTS.Model.API.FPS.Transaction
{
    using System;

    public class TransactionItemModel
    {
        public DateTime TransTime { get; set; }

        public long TransID { get; set; }

        public string LoginName { get; set; }

        public string FirstDeviceCode { get; set; }

        public string Action { get; set; }

        public string ActionResult { get; set; }

        public string OS { get; set; }

        public string Browser { get; set; }

        public string URLDetails { get; set; }

        public string IP { get; set; }
    }
}
