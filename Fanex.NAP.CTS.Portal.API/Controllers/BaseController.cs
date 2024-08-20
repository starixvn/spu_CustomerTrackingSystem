namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.ViewModel.API.Http;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Results;

    public abstract class BaseController : ApiController
    {
        public BaseController()
        {

        }

        [NonAction]
        protected JsonResult<HttpResponseViewModel<T>> Result<T>(T detail, string message = "")
        {
            var result = new HttpResponseViewModel<T>
            {
                statusCode = message.IsNullOrEmpty() ? HttpStatusCode.OK : HttpStatusCode.BadRequest,
                errorMessage = message,
                detail = detail
            };

            return Json(result);
        }
    }
}
