namespace Fanex.NAP.CTS.Portal.API.ActionFilter
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using System.Linq;
    using Fanex.NAP.CTS.Core.Extentions;
    using System.Text;

    public class CheckValidInputParameterFilterAttribute : ActionFilterAttribute
	{
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            if (!filterContext.ModelState.IsValid)
            {
                var invalidModelState = filterContext.ModelState.Values
                    .Where(x => x.Errors.Count > 0)?.SelectMany(x => x.Errors?.Select(y => y.Exception.Message));

                var invalidJson = invalidModelState.IsNotNullOrEmpty() ? JsonConvert.SerializeObject(invalidModelState) : string.Empty;

                var logMessage = new LogMessage("Invalid Input Parameter")
                    .AddMessage(invalidJson)
                    .ToString();

                LogService.Error(logMessage);

                filterContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                filterContext.Response.Content = new StringContent(invalidJson, Encoding.Default, "application/json");
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }
}