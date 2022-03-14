namespace Fanex.NAP.CTS.Model
{
    using Fanex.Logging;
    using Fanex.NAP.Core.Web.Session;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class TrackingFunctionUsageFilterAttribute : ActionFilterAttribute
    {
        public TrackingCtsFunctionId FunctionId { get; set; }

        public ICurrentSession CurrentSession { get; set; }

        public SystemId SystemId { get; set; }

        public string IsSkipTrackingFuncAttr { get; set; }

        private readonly IMonitorCtsFunctionService _monitorService;

        public TrackingFunctionUsageFilterAttribute(SystemId systemId, TrackingCtsFunctionId functionId, string IsSkipTrackingFuncAttr = "false")
        {
            _monitorService = new MonitorCtsFunctionService();
            CurrentSession = new CurrentSession();
            this.FunctionId = functionId;
            this.SystemId = systemId;
            this.IsSkipTrackingFuncAttr = IsSkipTrackingFuncAttr;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (IsSkipTrackingFuncAttr != null && filterContext.ActionParameters.ContainsKey(IsSkipTrackingFuncAttr))
                {
                    var isSkipTracking = filterContext.ActionParameters[IsSkipTrackingFuncAttr] as bool? ?? false;

                    if (isSkipTracking)
                    {
                        return;
                    }
                }

                InsertStatsAccessment();
            }
            catch (Exception ex)
            {
                LogService.Info($"Tracking function usage - Insert Stats Accessment fail - {ex.Message}");
            }
        }

        private void InsertStatsAccessment()
        {
            _monitorService.StatsAccessmentInsert(SystemId, FunctionId, CurrentSession.UserId);
        }
    }
}