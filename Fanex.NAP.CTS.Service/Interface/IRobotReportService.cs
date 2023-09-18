namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Model.RobotReport;
    using Fanex.NAP.CTS.ViewModel.RobotReport;

    public interface IRobotReportService
    {
        RobotReportViewModel GetRobotReport(RobotReportParamModel param);
        AIRobotReportViewModel GetAIRobotReport(RobotReportParamModel param);
    }
}
