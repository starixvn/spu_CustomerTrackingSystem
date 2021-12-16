using System;

namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    public class MatchMonitorVerifiedTransDetailModel
    {
        public Int64 TransID { get; set; }

        public DateTime VerifiedDate { get; set; }

        public string VerifiedBy { get; set; }              
    }
}
