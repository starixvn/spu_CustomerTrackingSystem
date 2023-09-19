namespace Fanex.NAP.CTS.Service.Interface.RobotReport
{
    using Fanex.NAP.CTS.Model.RobotReport;
    using Fanex.NAP.CTS.ViewModel.RobotReport;

    public interface IExportRobotReportService
    {
        byte[] ExportRobotReport(string templatePath, string fileName, RobotReportViewModel data, RobotReportParamModel param);

        byte[] ExportAIRobotReport(string templatePath, string fileName, AIRobotReportViewModel data, RobotReportParamModel param);
    }
}
