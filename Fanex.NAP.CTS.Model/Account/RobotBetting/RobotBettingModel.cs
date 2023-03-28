namespace Fanex.NAP.CTS.Model.Account.RobotBetting
{
    using System.Collections.Generic;

    public class RobotBettingModel
    {
        public RobotDateAndTypeModel TypeModel { get; set; }

        public IEnumerable<RobotBettingAIModel> RobotBettingAI { get; set; }

        public IEnumerable<RobotBettingImpervaModel> RobotBettingImperva { get; set; }
    }
}
