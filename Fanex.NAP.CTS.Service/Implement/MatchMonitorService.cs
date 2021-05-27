namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Criteria.ManageSubscriber;
    using Fanex.NAP.CTS.Model.ManageSubscriber;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using System.Collections.Generic;
    using System.Linq;

    public class MatchMonitorService : IMatchMonitorService
    {
        public List<MatchMonitorModel> GetMatchMonitor(MatchMonitorFilterViewModel filter)
        {
            var criteria = new MatchMonitorGetCriteria()
            {
                ip_FromDate = filter.FromDate,
                ip_ToDate = filter.ToDate,
                ip_BetType = filter.BetTypes,
                ip_IsVerified = filter.IsVerified
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MatchMonitorModel>(criteria).ToList();
            }
        }
    }
}