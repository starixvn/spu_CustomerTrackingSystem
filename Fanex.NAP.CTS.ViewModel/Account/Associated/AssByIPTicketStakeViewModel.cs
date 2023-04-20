namespace Fanex.NAP.CTS.ViewModel.Account
{
    using System;
  
    public class AssByIPTicketStakeViewModel
    {
        public AssByIPTicketStakeViewModel()
        {
            //this.LiveScore = ticket.LiveIndicator ? (ticket.LiveHomeScore + " - " + ticket.LiveAwayScore) : "-";
        }

        public string IP { get; set; }

        public string UserName { get; set; }

        public int MatchID { get; set; }
        
        public string EventDate { get; set; }

        public string TransDate { get; set; }

        public string TransID { get; set; }        

        public string BetType { get; set; }

        public string BetTeam { get; set; }

        public decimal Stake { get; set; }

        public string Currency { get; set; }

        public decimal Odds { get; set; }

        public string LiveScore { get; set; }
    }
}
