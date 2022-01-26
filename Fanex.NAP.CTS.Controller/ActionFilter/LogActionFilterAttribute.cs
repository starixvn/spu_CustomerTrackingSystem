namespace Fanex.NAP.CTS.Model
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using System;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class LogActionFilterAttribute : ActionFilterAttribute
    {
        public MonitorCtsFunctionId functionId { get; set; }

        public int userId { get; set; }

        public SystemId systemId { get; set; }

        private readonly IMonitorCtsFunctionService _monitorService;

        public LogActionFilterAttribute()
        {
            _monitorService = new MonitorCtsFunctionService();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            InsertStatsAccessment();
        }

        private void InsertStatsAccessment()
        {
            try
            {
                _monitorService.StatsAccessmentInsert(systemId, functionId, userId);
            }
            catch (Exception ex)
            {
                LogService.Error("Monitor CTS Function - Insert Stats Accessment fail", ex);
            }

            //var controllerName = routeData.Values["controller"];
            //var actionName = routeData.Values["action"];
            //var message = String.Format("{0} controller:{1} action:{2}", methodName, controllerName, actionName);
            //Debug.WriteLine(message, "Action Filter Log");
        }
    }
}