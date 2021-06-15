namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Criteria.ProblemCaseMonitor;
    using Fanex.NAP.CTS.Model.ProblemCaseMonitor;
    using Fanex.NAP.CTS.Service.Interface;
    using System.Collections.Generic;
    using System.Linq;

    public class ProblemCaseMonitorService : IProblemCaseMonitorService
    {
        public IEnumerable<ProblemCasesFilterModel> GetProblemCasesFilter(int listId, bool status)
        {
            var criteria = new ProblemCaseFilterGetCriteria()
            {
                ListId = listId,
                Status = status
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ProblemCasesFilterModel>(criteria).ToList();
            }
        }
    }
}
