namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using Fanex.NAP.CTS.Resource.Consts;
    using System;

    [Serializable]
    public class GetAssociationDetectDetailMatchViewModel
    {
        public GetAssociationDetectDetailMatchViewModel() { }

        public GetAssociationDetectDetailMatchViewModel(GetAssociationDetectDetailMatchModel ticket)
        {       
            this.LeagueID = ticket.LeagueID;
            //this.LeagueName = CachedRef.GetLeagueName(ticket.LeagueID); RemotingRef not working
            this.LeagueName = ticket.LeagueID.ToString(); // Assuming LeagueName is directly available in the model
            this.MatchID = ticket.MatchID;
            //this.MatchName = CachedRef.GetTeamName(ticket.HomeID) + " vs " + CachedRef.GetTeamName(ticket.AwayID); RemotingRef not working           
            this.MatchName = $"{ticket.HomeID} vs {ticket.AwayID}"; // Assuming HomeID and AwayID are team names or IDs
            this.HomeID = ticket.HomeID;
            this.AwayID = ticket.AwayID;
            this.EventDate = ticket.EventDate.ToString(Consts.FormatDateToString_MMMddyyyy);
            this.UserName = ticket.UserName;
            this.CustID = ticket.CustID;
            this.TransDate = ticket.TransDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss);
            this.TransID = ticket.TransID.ToString();
            this.BetTypeID = ticket.BetType;
            //this.BetTypeName = CachedRef.GetBettypeName(ticket.BetType); RemotingRef not working
            this.BetTypeName = ticket.BetType.ToString(); // Assuming BetType is an enum or has a string representation
            this.BetTeam = ticket.BetTeam;
            this.Stake = ticket.Stake;
            this.Odd = ticket.Odds;
            this.LiveScore = ticket.LiveHomeScore + " - " + ticket.LiveAwayScore;
            this.Result = ticket.TicketStatus;
        }

        public int LeagueID { get; set; }

        public string LeagueName { get; set; }

        public int MatchID { get; set; }

        public string MatchName { get; set; }

        public int HomeID { get; set; }

        public int AwayID { get; set; }      

        public string EventDate { get; set; }

        public string UserName { get; set; }

        public int CustID { get; set; }

        public string TransDate { get; set; }

        public string TransID { get; set; }

        public int BetTypeID { get; set; }

        public string BetTypeName { get; set; }

        public string BetTeam { get; set; }

        public decimal Stake { get; set; }

        public decimal Odd { get; set; }

        public string LiveScore { get; set; }

        public string Result { get; set; }
    }
}
