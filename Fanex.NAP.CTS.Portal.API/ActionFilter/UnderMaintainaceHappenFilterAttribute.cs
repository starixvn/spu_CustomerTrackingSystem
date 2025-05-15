namespace Fanex.NAP.CTS.Portal.API.ActionFilter
{
    using ONEbook.UM;
    using ONEbook.UM.Services;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using Fanex.NAP.CTS.User.Helper;
    using System.Net.Http;

    public class UnderMaintainaceHappenFilterAttribute : ActionFilterAttribute
    {
        private IMaintenanceService _maintenanceService;

        public UnderMaintainaceHappenFilterAttribute()
        {
            _maintenanceService = new MaintenanceService(UMClientManager.RestClient, UMClientManager.CacheService, UMClientManager.UMClientConfigs, UMClientManager.HttpContext);
        }

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            if (UMHelper.IsUnderMaintainaceHappen(_maintenanceService))
            {
                filterContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                filterContext.Response.Content = new StringContent("System is under maintenance, please try again later.");
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }
}