namespace Fanex.NAP.CTS.Model
{
    using Fanex.Logging;
    using Fanex.NAP.Core.Web.Session;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using System;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class TrackingUserAccessFilterAttribute : ActionFilterAttribute
    {
        public MonitorCtsFunctionId functionId { get; set; }

        public ICurrentSession currentSession { get; set; }

        public SystemId systemId { get; set; }

        private readonly IMonitorCtsFunctionService _monitorService;

        public TrackingUserAccessFilterAttribute()
        {
            _monitorService = new MonitorCtsFunctionService();
            currentSession = new CurrentSession();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            InsertStatsAccessment();
        }

        private void InsertStatsAccessment()
        {
            try
            {
                _monitorService.StatsAccessmentInsert(systemId, functionId, currentSession.UserId);
            }
            catch (Exception ex)
            {
                LogService.Info($"Tracking CTS's user access - Insert Stats Accessment fail - {ex.Message}");
            }
        }
    }
}