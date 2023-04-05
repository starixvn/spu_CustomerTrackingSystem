namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    using System.Collections.Generic;

    public class RobotOcrdViewModel
    {
        public RobotOcrdViewModel()
        {
            Detail = new List<RobotOcrdTicketViewModel>();
        }

        public long CustID { get; set; }

        public int RangeType { get; set; }

        public string RangeName { get; set; }

        public List<RobotOcrdTicketViewModel> Detail { get; set; }
    }
}
