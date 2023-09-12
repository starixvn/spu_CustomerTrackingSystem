namespace Fanex.NAP.CTS.Model.MatchMonitor
{
    public class MMDetailFilterTicketInfoModel
    {
        public bool IsLicensee { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }
       
        public int TicketStatus { get; set; }

        public short CurrencyId { get; set; }

        public string CurrencyName { get; set; }

        public int CC { get; set; }

        public decimal Hdp { get; set; }

        public string BetTeam { get; set; }

        public int Danger1 { get; set; }

        public int Danger2 { get; set; }

        public int Danger3 { get; set; }

        public int Danger4 { get; set; }

        public int Danger5 { get; set; }

    }
}
