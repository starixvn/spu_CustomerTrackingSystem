using System.Collections.Generic;

namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    public class RobotBettingViewModel
    {
        public RobotBettingViewModel()
        {
            Detail = new List<RobotBettingDetailViewModel>();
            IsRobotImperva = false;
            RobotImpervas = new List<RobotBettingImpervaViewModel>();
        }

        public long CustID { get; set; }

        public int RangeType { get; set; }

        public string RangeName { get; set; }

        public bool IsRobotImperva { get; set; }

        public IEnumerable<RobotBettingImpervaViewModel> RobotImpervas { get; set; }

        public List<RobotBettingDetailViewModel> Detail { get; set; }
        
    }
}
