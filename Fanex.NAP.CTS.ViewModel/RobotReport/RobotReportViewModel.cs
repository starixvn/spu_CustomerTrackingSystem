namespace Fanex.NAP.CTS.ViewModel.RobotReport
{
    using Fanex.NAP.CTS.Model.RobotReport;
    using System.Collections.Generic;

    public class RobotReportViewModel
    {
        public IEnumerable<RobotReportModel> Items { get; set; }

        public RobotReportModel TFoot { get; set; }
    }
}
