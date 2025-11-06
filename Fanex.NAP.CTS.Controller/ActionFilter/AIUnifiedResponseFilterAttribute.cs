namespace Fanex.NAP.CTS.Controller.ActionFilter
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Resource.Consts;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Web.Mvc;

    public class AIUnifiedResponseFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;

            if (filterContext.Exception != null)
            {
                var ex = filterContext.Exception;

                var data = DataAfterHandleException(ex, actionName);

                filterContext.Result = data;
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;

                filterContext.ExceptionHandled = true;
            }
            else
            if (filterContext.Result is JsonResult jsonResult)
            {
                filterContext.Result = CreateSuccessResponse(jsonResult.Data);
            }

            base.OnActionExecuted(filterContext);
        }

        #region Response Helpers

        private JsonResult CreateSuccessResponse(object data)
        {
            return new JsonResult
            {
                Data = new
                {
                    statuscode = (int)HttpStatusCode.OK,
                    data = data
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }

        private JsonResult CreateErrorResponse(string message, int statuscode = 400)
        {
            return new JsonResult
            {
                Data = new
                {
                    statuscode = statuscode,
                    message = message
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region Exception Handler
        private JsonResult DataAfterHandleException(Exception ex, string nameOfFunction)
        {
            switch (ex)
            {
                case InvalidDataException invalidDataException:
                    return CreateErrorResponse(ex.Message, (int)Consts.HttpStatus.InvalidData);
                case HttpRequestException httpRequestException
                    when httpRequestException.InnerException?.Message?.Contains("connection was closed unexpectedly") == true ||
                         httpRequestException.InnerException?.Message?.Contains("expected to be kept alive was closed by the server") == true:
                    return CreateErrorResponse("The connection was interrupted, please retry again", (int)Consts.HttpStatus.HttpNetworkClose);
                case ObjectDisposedException objectDisposedException:
                    return CreateErrorResponse("The connection was interrupted, please retry again", (int)Consts.HttpStatus.HttpNetworkClose);
                case HttpListenerException httpListenerException:
                    LogExtension.LogError($"CTS - AI Insights {nameOfFunction} - AI Service Error", httpListenerException);
                    return CreateErrorResponse($"An error occurred while {nameOfFunction}", (int)Consts.HttpStatus.AIServiceError);
                case AggregateException agr:
                    LogExtension.LogError($"CTS - AI Insights {nameOfFunction} Inner Failed", agr);
                    return CreateErrorResponse($"An error occurred while {nameOfFunction}");
                default:
                    LogExtension.LogError($"CTS - AI Insights {nameOfFunction}", ex);
                    return CreateErrorResponse($"An error occurred while {nameOfFunction}");
            }
        }
        #endregion
    }
}
