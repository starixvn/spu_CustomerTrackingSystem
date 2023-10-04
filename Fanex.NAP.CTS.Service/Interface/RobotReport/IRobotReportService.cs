namespace Fanex.NAP.CTS.Service.Interface.RobotReport
{
    using Fanex.NAP.CTS.Model.RobotReport;
    using Fanex.NAP.CTS.ViewModel.RobotReport;

    public interface IRobotReportService
    {
        RobotReportViewModel GetRobotReport(RobotReportParamModel param);
        
        AIRobotReportViewModel GetAIRobotReport(RobotReportParamModel param);

        byte[] ExportRobotReport(string templatePath, string fileName, RobotReportParamModel param);

        byte[] ExportAIRobotReport(string templatePath, string fileName, RobotReportParamModel param);
    }
}
