namespace Fanex.NAP.CTS.Service.Interface.Api
{
    using Fanex.NAP.CTS.Model.API.ProblemCaseMonitor;
    using System;
    using System.Collections.Generic;   

    public interface IProblemCaseMonitorService
    {
        IEnumerable<ProblemCaseModel> GetProlemCase(int listId, bool status);

        IEnumerable<ProblemCaseMonitorModel> GetProblemCaseMonitor(ProblemCaseMonitorParamsModel paras);
    }
}
