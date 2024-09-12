namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.Parlay
{
    using Fanex.NAP.CTS.Model.MatchMonitor.Parlay;
    using System;

    public class ExExcelParlayTicketInfoViewModel
    {
        public ExExcelParlayTicketInfoViewModel() { }

        public ExExcelParlayTicketInfoViewModel(ExExcelTicketInfoViewModel data)
        {
            this.SubscriberType = data.SubscriberType;
            this.Username = data.Username;
            this.CustomerClass = data.CustomerClass;
            this.IsRobot = data.IsRobot;
            this.DangerToString = data.DangerToString;
            this.Odds = data.Odds;
            this.MalayOdds = data.MalayOdds;
            this.BetID = data.BetID;
            this.MatchName = null;
            this.Currency = data.Currency;
            this.Amount = data.Amount;
            this.Choice = data.Choice;
            this.Score = data.Score;
            this.TransTimeToString = data.TransTimeToString;
            this.BetFrom = data.BetFrom;
            this.ADetectGroup = data.ADetectGroup;
            this.MDetectGroup = data.MDetectGroup;
            this.ReasonList = data.ReasonList;
            this.PICVerified = data.PICVerified;
            this.IsParlay = false;
        }

        public ExExcelParlayTicketInfoViewModel(ExExcelTicketInfoViewModel single, ParlayTicketInfoModel parlay)
        {
            this.SubscriberType = single.SubscriberType;
            this.Username = single.Username;
            this.CustomerClass = single.CustomerClass;
            this.IsRobot = single.IsRobot;
            this.DangerToString = single.DangerToString;
            this.BetID = null;
            this.Odds = parlay.Odds;
            this.MalayOdds = parlay.MalayOdds.ToString();
            this.MatchName = parlay.HomeName + " vs " + parlay.AwayName;
            this.Amount = parlay.Amount;
            this.Choice = parlay.Choice;
            this.Score = parlay.Score;
            this.TransTimeToString = null;
            this.BetFrom = null;
            this.ADetectGroup = null;
            this.MDetectGroup = null;
            this.ReasonList = null;
            this.PICVerified = null;
            this.IsParlay = true;
        }

        public string SubscriberType { get; set; }

        public string Username { get; set; }

        public int CustomerClass { get; set; }

        public bool IsRobot { get; set; }

        public string DangerToString { get; set; }

        public string BetID { get; set; }

        public string Currency { get; set; }

        public string TransTimeToString { get; set; }

        public string BetFrom { get; set; }

        public string ADetectGroup { get; set; }

        public string MDetectGroup { get; set; }

        public string ReasonList { get; set; }

        public string PICVerified { get; set; }

        public string MatchName { get; set; }
     
        public int BetTypeID { get; set; }

        public string BetTypeName { get; set; }

        public string BetTeam { get; set; }

        public string SportName { get; set; }

        public string LeagueName { get; set; }

        public string GlobalShowTime { get; set; }

        public decimal Odds { get; set; }

        public string MalayOdds { get; set; }

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
