namespace Fanex.NAP.CTS.Model.ProbationManagement
{
    public class AccumulatedInfoModel
    {
        public int CustId { get; set; }

        public int SportId { get; set; }

        public decimal Winloss { get; set; }

        public decimal Margin { get; set; }

        public long BetCount { get; set; }

        public int ActiveDays { get; set; }
    }
}
