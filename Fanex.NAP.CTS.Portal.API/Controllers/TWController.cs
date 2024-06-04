namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Model.API.TWInitialSmart;
    using Fanex.NAP.CTS.Model.LogInputData;
    using Fanex.NAP.CTS.Portal.API.ActionFilter;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement.Api;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class TWController : ApiController
    {
        private readonly ITWInitialSmartService tWInitialSmartService;

        public TWController()
        {
            tWInitialSmartService = new TWInitialSmartService();
        }

        [HttpPost]
        [Route("api/TW/AddCustomerEarlyBlock")]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.AddCustomerInitialSmarts)]
        public IHttpActionResult TWAddCustomerInitialSmarts([FromBody] List<InitialSmartCustInfoModel> custInfos)
        {
            try
            {
                var isSuccess = true;

                if (!isSuccess)
                {
                    return BadRequest($"Add Customers Initial Smart failed. Data: {JsonConvert.SerializeObject(custInfos)}");
                }               
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage($"CTS API - TW AddCustomerInitialSmarts")
                                       .AddMessage(ex.Message)
                                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                                       .AddMessage($"Data {JsonConvert.SerializeObject(custInfos)} - DateTime {DateTime.UtcNow} UTC").ToString();

                LogService.Error(logMessage);

                return InternalServerError(ex);
            }

            return Ok();
        }
    }
}