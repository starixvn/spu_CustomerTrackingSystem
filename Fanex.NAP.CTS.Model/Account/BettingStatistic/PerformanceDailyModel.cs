using System;

namespace Fanex.NAP.CTS.Model.Account
{
   public class PerformanceDailyModel
    {
        public DateTime RptDate { get; set; }

        public decimal Turnover { get; set; }

        public decimal Winloss { get; set; }

        public int BetCount { get; set; }
    }
}
