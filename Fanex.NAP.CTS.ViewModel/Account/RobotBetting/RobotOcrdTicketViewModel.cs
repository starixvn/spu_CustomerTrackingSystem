namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Resource.Consts;

    public class RobotOcrdTicketViewModel
    {
        public RobotOcrdTicketViewModel(MainTicketDetailModel ticket)
        {
            this.TransDate = ticket.TransDate.ToString(Consts.FormatDatetimeToString_ddMMMyyyyHHmmssfff);
            this.TransID = ticket.TransId.ToString();
            this.IP = ticket.IP;
            this.BetType = ticket.BetType;
            this.BetTypeName = CachedRef.GetBettypeName(ticket.BetType);
            this.BetTeam = ticket.BetTeam;
            this.Stake = ticket.Stake;
            this.Odds = ticket.Odds;
            this.LiveScore = ticket.LiveIndicator ? (ticket.LiveHomeScore + " - " + ticket.LiveAwayScore) : "-";
        }

        public string TransDate { get; set; }

        public string TransID { get; set; }

        public string IP { get; set; }

        public int BetType { get; set; }

        public string BetTypeName { get; set; }

        public string BetTeam { get; set; }

        public decimal Stake { get; set; }

        public decimal Odds { get; set; }

        public string LiveScore { get; set; }

        public int HomeID { get; set; }

        public int AwayID { get; set; }
       
    }
}
