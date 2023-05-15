namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    using System.Collections.Generic;

    public class RobotBettingViewModel
    {
        public RobotBettingViewModel()
        {
            this.FirstRobotDate = string.Empty;
            this.LastRobotDate = string.Empty;
            this.RobotSCE = new List<RobotSceViewModel>();
            this.RobotOCRD = new List<RobotRDViewModel>();
            this.RobotTD = new List<RobotRDViewModel>();
            this.RobotImpervas = new List<RobotImpervaViewModel>();
        }

        public string FirstRobotDate { get; set; }

        public string LastRobotDate { get; set; }

        public IEnumerable<RobotSceViewModel> RobotSCE { get; set; }

        public IEnumerable<RobotRDViewModel> RobotOCRD { get; set; }

        public IEnumerable<RobotRDViewModel> RobotTD { get; set; }

        public IEnumerable<RobotImpervaViewModel> RobotImpervas { get; set; }
    }
}
