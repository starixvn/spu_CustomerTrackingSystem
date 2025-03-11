namespace Fanex.NAP.CTS.Model.CustomerClassificationAgency.ProblemAccountAgency
{
    using System;   

    public class ProblemAccountLosingAgencyModel
    {
        public long CustId { get; set; }

        public int WinlossStatus { get; set; }

        public decimal TurnoverRM { get; set; }

        public decimal WinlossRM { get; set; }

        public long BetCount { get; set; }        

        public DateTime? PerformanceTime { get; set; }
    }
}
