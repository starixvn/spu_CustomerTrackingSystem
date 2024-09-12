namespace Fanex.NAP.CTS.Model.MatchMonitor.Parlay
{
    using Fanex.NAP.CTS.Resource.Consts;
    using System;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class ParlayTicketInfoModel
    {
        public ParlayTicketInfoModel()
        {
        }

        public ParlayTicketInfoModel(TicketDetailModel data)
        {
            if (data != null)
            {
                this.HomeName = data.HomeName;
                this.AwayName = data.AwayName;
                this.BetTypeID = data.BetType;
                this.BetTypeName = data.BettypeName;
                this.BetTeam = data.BetTeam;
                this.LeagueName = data.LeagueName;
                this.SportName = data.SportType;
                this.GlobalShowTime = data.KickOffTime.ToString(Consts.FormatDateTimeToString_ddMMyyhhmmss);
                this.IsMainTicket = data.TicketOrder == 1;
                this.Odds = data.Odds;
                this.MalayOdds = data.MalayOdds;
                this.CurrencyName = data.CurrencyName;
                this.CurrencyId = data.CurrencyId;
                this.Amount = data.OrgStake;               
                this.Choice = data.Choice;
                this.Score = data.LiveIndicator ? (data.LiveHomeScore + " - " + data.LiveAwayScore) : "-";
                this.IsParlay = true;
                this.Combinition = data.Combinition;
            }
        }

        public string HomeName { get; set; }

        public string AwayName { get; set; }

        public int BetTypeID { get; set; }

        public string BetTypeName { get; set; }

        public string BetTeam { get; set; }

        public string SportName { get; set; }

        public string LeagueName { get; set; }

        public string GlobalShowTime { get; set; }

        public decimal Odds { get; set; }

        public decimal? MalayOdds { get; set; }

        public string CurrencyName { get; set; }

        public Int16 CurrencyId { get; set; }       

        public string Choice { get; set; }

        public Decimal Amount { get; set; }

        public string Score { get; set; }

        public bool IsParlay { get; set; }

        public int Combinition { get; set; }

        public bool IsMainTicket { get; set; }       
    }
}
