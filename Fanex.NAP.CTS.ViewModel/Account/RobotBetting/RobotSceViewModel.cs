namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    using System.Collections.Generic;
    
    public class RobotSceViewModel
    {
        public RobotSceViewModel()
        {
            Detail = new List<RobotBettingDetailViewModel>();
        }

        public long CustID { get; set; }

        public int RangeType { get; set; }

        public string RangeName { get; set; }    

        public List<RobotBettingDetailViewModel> Detail { get; set; }
    }
}
