namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using System;

    public class MatchesAssociationDetailViewModel
    {
        public string EventDateString { get; set; }

        public DateTime EventDate { get; set; }

        public Int64 CustId1 { get; set; }

        public Int64 CustId2 { get; set; }

        public int MatchId { get; set; }

        public string Bettypes { get; set; }        
    }
}