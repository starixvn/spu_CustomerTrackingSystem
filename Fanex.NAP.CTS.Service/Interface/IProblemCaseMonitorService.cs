namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Model.ProblemCaseMonitor;
    using System.Collections.Generic;

    public interface IProblemCaseMonitorService
    {
        IEnumerable<ProblemCasesFilterModel> GetProblemCasesFilter(int listId, bool status);

        IEnumerable<ProblemCasesMonitorModel> GetProblemCaseMonitor(string apiUrl, ProblemCasesMonitorFilterModel para);

        byte[] ExportProblemCaseMonitor(List<ProblemCasesMonitorModel> data, string fileName, string templatePath, string sheetName);
    }
}
