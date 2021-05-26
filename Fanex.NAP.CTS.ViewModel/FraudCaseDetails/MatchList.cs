namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using System.Collections.Generic;

    public class MatchList
    {
        public IEnumerable<Match> Matches { get; set; }

        public IEnumerable<string> FraudCases { get; set; }
    }
}