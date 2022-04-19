namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using System;

    public class MatchesInfoViewModel
    {
        public string MatchDesc { get; set; }

        public Int64 MatchID { get; set; }        

        public string EventDateString { get; set; }

        public Int64 CustID1 { get; set; }

        public Int64 CustID2 { get; set; }

        public string GroupIDs { get; set; }

        public bool IsHavingSGB { get; set; }

        public string BettypeIDs { get; set; }

        public bool IsHavingGGB { get; set; }

        public string  States { get; set; }
    }
}
