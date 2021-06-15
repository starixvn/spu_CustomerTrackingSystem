namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Criteria.ProblemCaseMonitor;
    using Fanex.NAP.CTS.Model.ProblemCaseMonitor;
    using Fanex.NAP.CTS.Service.Interface;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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

        public IEnumerable<ProblemCasesMonitorModel> GetProblemCaseMonitor(string apiUrl, ProblemCasesMonitorFilterModel para)
        {
            var param = new
            {
                FromDate = para.FromDate,
                ToDate = para.ToDate,
                SubscriberIDList = para.SubscriberIds,
                ProblemCaseIDList = para.ProblemCaseIds,
            };

            var data = ApiHelper.Post<IEnumerable<ProblemCasesMonitorModel>>(apiUrl, param);

            Task.WhenAll(data);

            return data.Result ?? new List<ProblemCasesMonitorModel>();
        }
    }
}
