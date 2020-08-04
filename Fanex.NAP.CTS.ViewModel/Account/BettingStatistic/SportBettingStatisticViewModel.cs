using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.ViewModel.Account.BettingStatistic
{
    public class SportBettingStatisticViewModel
    {
        public int SportID { get; set; }

        public string SportName { get; set; }

        public decimal TurnOver  { get; set; }

        public decimal WinLoss { get; set; }

        public long BetCount { get; set; }

        public decimal TurnOver14 { get; set; }

        public long BetCount14  { get; set; }

        public string StakeLevel
        {
            get
            {
                if (TurnOver14 == 0 || BetCount14 == 0)
                {
                    return "Low Stake";
                }

                decimal tempValue = TurnOver14 / BetCount14;

                if (tempValue > 5000)
                {
                    return "High Stake";
                }
                else if (tempValue<=1000)
                {
                    return "Low Stake";
                }

                return "Medium Stake";
            }
        }
        
        public decimal Margin
        {
            get
            {
                if (WinLoss == 0 || TurnOver == 0) 
                {
                    return 0;
                }

                return (WinLoss / TurnOver) * 100;
            }
        }

        public int Ranking { get; set; }
    }
}
