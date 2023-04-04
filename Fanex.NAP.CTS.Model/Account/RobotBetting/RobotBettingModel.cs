namespace Fanex.NAP.CTS.Model.Account.RobotBetting
{
    using System.Collections.Generic;

    public class RobotBettingModel
    {
        public RobotDateAndTypeModel FirstLastRobotDate { get; set; }

        public IEnumerable<RobotBettingAIModel> RobotBettingAI { get; set; }

        public IEnumerable<RobotBettingImpervaModel> RobotBettingImperva { get; set; }

        public IEnumerable<RobotBettingAIModel> RobotBettingOCRD { get; set; }
    }
}
