using System;

namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    public class MatchMonitorDetailModel
    {
        public string ListCustID { get; set; }

        public string ListTransID { get; set; }

        public Int64 GroupID { get; set; }

        public bool IsHedging { get; set; }

        public int TotalScore { get; set; }                
    }
}
