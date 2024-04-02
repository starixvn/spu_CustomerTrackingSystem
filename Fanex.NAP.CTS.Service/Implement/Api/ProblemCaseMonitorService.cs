namespace Fanex.NAP.CTS.Service.Implement.Api
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Criteria.Api.ProblemCaseMonitor;
    using Fanex.NAP.CTS.Model.API.ProblemCaseMonitor;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ProblemCaseMonitorService : IProblemCaseMonitorService
    {
        public IEnumerable<ProblemCaseModel> GetProlemCase(int listId, bool status)
        {
            var criteria = new ProblemCaseCriteria
            {
                ListId = listId,
                Status = status
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = db.Query<ProblemCaseModel>(criteria);

                return result.ToList();
            }
        }        
    }
}
