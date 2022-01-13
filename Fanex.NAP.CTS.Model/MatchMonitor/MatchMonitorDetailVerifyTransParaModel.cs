using System;

namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    public class MatchMonitorDetailVerifyTransParaModel
    {
        public Int64 MatchId { get; set; }

        public bool LiveIndicator { get; set; }

        public int BettypeID { get; set; }     

        public int? BetID { get; set; }     
        
        public string TranIds { get; set; }      
        
        public int VerifiedBy { get; set; }              
    }
}
