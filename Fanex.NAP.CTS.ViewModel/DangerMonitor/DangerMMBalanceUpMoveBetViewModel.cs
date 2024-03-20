namespace Fanex.NAP.CTS.ViewModel.DangerMonitor
{
    using System;

    public class DangerMMBalanceUpMoveBetViewModel
    {
        public string BalanceUpDay { get; set; }
        public string MoveBetsDay { get; set; }
        public bool IsGetDataFromBK { get; set; }
        public DateTime? FromDateBK { get; set; }
        public DateTime? ToDateBK { get; set; }
        public bool IsGetDataFrom14 { get; set; }
        public DateTime? FromDate14 { get; set; }
        public DateTime? ToDate14 { get; set; }
        public bool IsGetDataFromOrigin { get; set; }
        public DateTime? FromDateOrigin { get; set; }
        public DateTime? ToDateOrigin { get; set; }

    }
}