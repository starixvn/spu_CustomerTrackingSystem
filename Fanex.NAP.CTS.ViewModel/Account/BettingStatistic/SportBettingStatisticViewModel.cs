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

        public string StakeLevel
        {
            get
            {
                decimal tempValue = (BetCount == 0) ? 0 : (TurnOver / BetCount);

                if (tempValue > 5000)
                {
                    return "High Stake";
                }
                else if (tempValue <= 1000)
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
