using System;

namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{   
    public class DetectGroupAssFilterViewModel
    {
        public string CTSCustIDs { get; set; }

        public string CustIDs { get; set; }

        public bool IsOptBP { get; set; }

        public bool IsOptDevice { get; set; }

        public bool IsOptIP { get; set; }

        public bool IsOptAgent { get; set; }

        public bool IsOpt3MatchesLast7Days { get; set; }

        public bool IsOptIPLast3Days { get; set; }
       
    }
}
