namespace Fanex.NAP.CTS.Model.Association
{
    using System;
    using System.Collections.Generic;

    public class CoupleFraudDetailModel
    {
        public long StateID { get; set; }

        public decimal SumStake { get; set; }

        public int BetCount { get; set; }

        public long MatchID { get; set; }

        public DateTime EventDate { get; set; }

        public string FraudTransList { get; set; }

        public IEnumerable<string> FraudTransListSperation
        {
            get
            {
                if (string.IsNullOrEmpty(FraudTransList))
                {
                    return new List<string>();
                }
                char[] sperator = new char[] { ',' };
                return FraudTransList.Split(sperator, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}
