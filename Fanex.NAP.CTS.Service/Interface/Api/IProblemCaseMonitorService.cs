namespace Fanex.NAP.CTS.Service.Interface.Api
{
    using Fanex.NAP.CTS.Model.API.ProblemCaseMonitor;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IProblemCaseMonitorService
    {
        IEnumerable<ProlemCaseModel> GetProlemCase(int listId, bool status);
    }
}
