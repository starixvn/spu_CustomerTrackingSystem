namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    using System.Collections.Generic;

    public class RobotOcrdViewModel
    {
        public RobotOcrdViewModel()
        {
            Detail = new List<RobotOcrdDetailViewModel>();            
            RobotImpervas = new List<RobotImpervaViewModel>();
            IsRobotImperva = false;
        }

        public long CustID { get; set; }

        public int RangeType { get; set; }

        public string RangeName { get; set; }

        public bool IsRobotImperva { get; set; }

        public IEnumerable<RobotImpervaViewModel> RobotImpervas { get; set; }

        public List<RobotOcrdDetailViewModel> Detail { get; set; }
    }
}
