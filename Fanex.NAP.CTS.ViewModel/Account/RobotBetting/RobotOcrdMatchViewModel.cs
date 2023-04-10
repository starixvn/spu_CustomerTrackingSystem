namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Model.Common;
    using Fanex.NAP.CTS.Resource.Consts;
    using System;

    public class RobotOcrdMatchViewModel
    {
        public RobotOcrdMatchViewModel(MainMatchModel match, int totalTicket)
        {
            this.MatchID = match.MatchID;
            this.MatchName = CachedRef.GetTeamName(match.HomeId) + " vs " + CachedRef.GetTeamName(match.AwayId);
            this.EventDate = match.EventDate;
            this.EventDateStr = match.EventDate.ToString(Consts.FormatDateToString_MMMddyyyy);
            this.TotalTicket = totalTicket;
        }

        public long MatchID { get; set; }

        public string MatchName { get; set; }

        public DateTime EventDate { get; set; }

        public string EventDateStr { get; set; }

        public int TotalTicket { get; set; }
    }
}
