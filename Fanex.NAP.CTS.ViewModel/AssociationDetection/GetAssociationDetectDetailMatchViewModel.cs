namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Model.AssociationDetection;
    using Fanex.NAP.CTS.Resource.Consts;

    public class GetAssociationDetectDetailMatchViewModel
    {
        public GetAssociationDetectDetailMatchViewModel() { }

        public GetAssociationDetectDetailMatchViewModel(GetAssociationDetectDetailMatchModel match)
        {       
            this.LeagueID = match.LeagueID;
            this.LeagueName = CachedRef.GetLeagueName(match.LeagueID);
            this.MatchID = match.MatchID;
            this.MatchName = CachedRef.GetTeamName(match.HomeID) + " vs " + CachedRef.GetTeamName(match.AwayID);            
            this.HomeID = match.HomeID;
            this.AwayID = match.AwayID;
            this.EventDate = match.EventDate.ToString(Consts.FormatDateToString_MMMddyyyy);
            this.UserName = match.UserName;
            this.CustID = match.CustID;
            this.TransDate = match.TransDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss);
            this.TransID = match.TransID;
            this.BetTypeID = match.BetTypeID;
            this.BetTypeName = CachedRef.GetBettypeName(match.BetTypeID);
            this.BetTeam = match.BetTeam;
            this.Stake = match.Stake;
            this.Odd = match.Odd;
            this.LiveScore = match.LiveScore;
            this.Result = match.Result;
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

        public long TransID { get; set; }

        public int BetTypeID { get; set; }

        public string BetTypeName { get; set; }

        public string BetTeam { get; set; }

        public decimal Stake { get; set; }

        public decimal Odd { get; set; }

        public string LiveScore { get; set; }

        public string Result { get; set; }
    }
}
