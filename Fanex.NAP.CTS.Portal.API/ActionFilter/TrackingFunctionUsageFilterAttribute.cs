namespace Fanex.NAP.CTS.Portal.API.ActionFilter
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using System;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    /// <summary>
    /// ActionFilter for tracking CTSFunction Usage
    /// </summary>
    public class TrackingFunctionUsageFilterAttribute : ActionFilterAttribute
    {

        public TrackingCtsFunctionId FunctionId { get; set; }


        public SystemId SystemId { get; set; }

        private readonly IMonitorCtsFunctionService _monitorService;

        public TrackingFunctionUsageFilterAttribute(SystemId systemId, TrackingCtsFunctionId functionId)
        {
            _monitorService = new MonitorCtsFunctionService();
            this.FunctionId = functionId;
            this.SystemId = systemId;
        }

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            try
            {
                _monitorService.StatsAccessmentInsert(SystemId, FunctionId, 0);
            }
            catch (Exception ex)
            {
                LogService.Info($"Tracking function APIs usage - Insert Stats Accessment fail - {ex.Message}");
            }
        }
    }
}