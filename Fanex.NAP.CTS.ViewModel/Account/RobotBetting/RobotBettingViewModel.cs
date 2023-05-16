namespace Fanex.NAP.CTS.ViewModel.Account.RobotBetting
{
    using System.Collections.Generic;

    public class RobotBettingViewModel
    {
        public string FirstRobotDate { get; set; }

        public string LastRobotDate { get; set; }

        public IEnumerable<RobotBettingAIViewModel> RobotBettingAI { get; set; }

        public IEnumerable<RobotImpervaViewModel> RobotImpervas { get; set; }
    }
}
