namespace Fanex.NAP.CTS.ViewModel.RobotReport
{
    using Fanex.NAP.CTS.Model.RobotReport;
    using System.Collections.Generic;

    public class AIRobotReportViewModel
    {
        public IEnumerable<AIRobotReportModel> Items { get; set; }

        public IEnumerable<AIRobotReportTotalAccountModel> TFoot { get; set; }
    }
}
