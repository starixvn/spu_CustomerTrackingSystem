namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Model.MatchMonitor.Parlay;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class ExExcelTicketInfoViewModel
    {
        public Int64 CTSCustID { get; set; }

        public Int64 CustID { get; set; }

        public string SubscriberType { get; set; }

        public string Username { get; set; }

        public int CustomerClass { get; set; }

        public bool IsRobot { get; set; }

        public string DangerToString { get; set; }

        public int BetIDFromDB { get; set; }

        public virtual int Game
        {
            get => this.BetIDFromDB % 10;
            set => Game = value;
        }

        public decimal Odds { get; set; }

        public string MalayOdds { get; set; }

        public string BetID { get; set; }

        public string BetTeam { get; set; }

        public string Currency { get; set; }

        public decimal Amount { get; set; }

        public decimal HDP { get; set; }

        public string Choice { get; set; }

        public string Score { get; set; }

        public string TransTimeToString { get; set; }

        public string BetFrom { get; set; }

        public string ADetectGroup { get; set; }

        public string MDetectGroup { get; set; }

        public string ReasonList { get; set; }

        public string PICVerified { get; set; }

        public List<ParlayTicketInfoModel> ParlayTickets { get; set; }
    }
}
