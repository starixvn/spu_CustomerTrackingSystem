using System.Collections.Generic;

namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    public class RobotBettingViewModel
    {
        public RobotBettingViewModel()
        {
            Detail = new List<RobotBettingDetailViewModel>();
        }

        public long CustID { get; set; }

        public int PeriodRangeType { get; set; }

        public string PeriodRangeName { get; set; }

        public List<RobotBettingDetailViewModel> Detail { get; set; }
        
    }
}
