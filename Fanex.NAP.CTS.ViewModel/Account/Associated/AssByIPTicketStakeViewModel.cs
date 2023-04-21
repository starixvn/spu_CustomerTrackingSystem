namespace Fanex.NAP.CTS.ViewModel.Account
{   
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Resource.Consts;

    public class AssByIPTicketStakeViewModel
    {
        public AssByIPTicketStakeViewModel() { }

        public AssByIPTicketStakeViewModel(MainTicketDetailModel ticket)
        {
            this.IP = ticket.IP;
            this.UserName = ticket.UserName;
            this.MatchID = ticket.MatchId;
            this.EventDate = ticket.EventDate.ToString(Consts.FormatDateTimeToString_ddMMyyyy);
            this.TransDate = ticket.TransDate.ToString(Consts.FormatDateTimeToString_ddMMyyyyhhmmss);
            this.TransID = ticket.TransId.ToString();
            this.BetType = ticket.TypeName;
            this.BetTeam = ticket.BetTeam;
            this.Stake = ticket.Stake;
            this.Currency = ticket.CurrencyName;
            this.Odds = ticket.Odds;
            this.LiveScore = ticket.LiveIndicator ? (ticket.LiveHomeScore + " - " + ticket.LiveAwayScore) : "-";
        }

        public string IP { get; set; }

        public string UserName { get; set; }

        public long MatchID { get; set; }
        
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
