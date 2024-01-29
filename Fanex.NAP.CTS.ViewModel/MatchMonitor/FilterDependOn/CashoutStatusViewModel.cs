using Fanex.NAP.CTS.Model;

namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    public class CashoutStatusViewModel
    {
        public int? CashoutStatus { get; set; }

        public string CashoutName { get; set; }

        public int StatusOrderIdx
        {
            get
            {
                switch (CashoutStatus)
                {
                    case (int)StatusCashout.All:
                        return 1;
                    case (int)StatusCashout.Cashout:
                        return 2;
                    case (int)StatusCashout.NonCashout:
                        return 3;
                }

                return 0;
            }
        }
    }
}
