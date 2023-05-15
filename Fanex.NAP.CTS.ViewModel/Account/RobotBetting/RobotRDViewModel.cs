namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    using System.Collections.Generic;

    public class RobotRDViewModel
    {
        public RobotRDViewModel()
        {
            Detail = new List<RobotRDTicketViewModel>();
        }

        public long CustID { get; set; }

        public int RangeType { get; set; }

        public string RangeName { get; set; }

        public List<RobotRDTicketViewModel> Detail { get; set; }
    }
}
