namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    using System;

    public class MMDetailBetTeamModel
    {
        public MMDetailBetTeamModel(MatchMonitorDetailModel m, TicketDetailModel t)
        {
            this.TransID = m.TransID;
            this.Reason = m.Reason;
            this.GroupID = m.GroupID;
            this.Betteam = t.BetTeam;
            this.CustID = t.CustId;
        }

        public long TransID { get; set; }

        public int Reason { get; set; }

        public int GroupID { get; set; }

        public string Betteam { get; set; }

        public Int64 CustID { get; set; }

    }
}
