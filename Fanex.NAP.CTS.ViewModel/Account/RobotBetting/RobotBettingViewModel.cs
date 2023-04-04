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
            this.RobotOCRD = new List<RobotOcrdViewModel>();
            this.RobotImpervas = new List<RobotImpervaViewModel>();
        }

        public string FirstRobotDate { get; set; }

        public string LastRobotDate { get; set; }

        public IEnumerable<RobotSceViewModel> RobotSCE { get; set; }

        public IEnumerable<RobotOcrdViewModel> RobotOCRD { get; set; }

        public IEnumerable<RobotImpervaViewModel> RobotImpervas { get; set; }
    }
}
