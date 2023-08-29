namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Model.MatchMonitor;  

    public class MMDetailFilterTicketInfoViewModel
    {
        public MMDetailFilterTicketInfoViewModel(MMDetailFilterTicketInfoModel ticketInfo, bool liveIndicator)
        {
            this.LiveIndicator = liveIndicator;
            this.IsLicensee = ticketInfo.IsLicensee;
            this.SubscriberType = ticketInfo.IsLicensee ? "L" : "C";
            this.Score = liveIndicator ? (ticketInfo.LiveHomeScore + " - " + ticketInfo.LiveAwayScore) : "-";
            this.TicketStatus = ticketInfo.TicketStatus;
            this.CurrencyId = ticketInfo.CurrencyId;
            this.CurrencyName = ticketInfo.CurrencyName;
            this.Hdp = ticketInfo.Hdp;
            this.BetTeam = ticketInfo.BetTeam;
            this.Danger1 = ticketInfo.Danger1;
            this.Danger2 = ticketInfo.Danger2;
            this.Danger3 = ticketInfo.Danger3;
            this.Danger4 = ticketInfo.Danger4;
            this.Danger5 = ticketInfo.Danger5;
            this.IsDanger = ticketInfo.Danger1 > 0 ||
                ticketInfo.Danger2 > 0 ||
                ticketInfo.Danger3 > 0 ||
                ticketInfo.Danger4 > 0 ||
                ticketInfo.Danger5 > 0;
        }

        public string SubscriberType { get; set; }

        public bool LiveIndicator { get; set; }

        public bool IsLicensee { get; set; }

        public string Score { get; set; }

        public int TicketStatus { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyName { get; set; }

        public decimal Hdp { get; set; }

        public string BetTeam { get; set; }

        public int Danger1 { get; set; }

        public int Danger2 { get; set; }

        public int Danger3 { get; set; }

        public int Danger4 { get; set; }

        public int Danger5 { get; set; }

        public bool IsDanger { get; set; }
    }
}
