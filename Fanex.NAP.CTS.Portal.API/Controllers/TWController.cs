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
    using System.Linq;
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
                var dbExistedCusts = tWInitialSmartService.AddCustTWInitialSmarts(custInfos);

                if (dbExistedCusts != null && dbExistedCusts.Any())
                {
                    var logMessage = new LogMessage($"CTS API - TW AddCustomerInitialSmarts cust have existed")
                                      .AddMessage($"Custs existed in DB: {JsonConvert.SerializeObject(dbExistedCusts)}")
                                      .AddMessage($"Data {JsonConvert.SerializeObject(custInfos)} - DateTime {DateTime.UtcNow} UTC").ToString();

                    LogService.Error(logMessage);
                }               
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage($"CTS API - TW AddCustomerInitialSmarts exception")
                                       .AddMessage(ex.Message)
                                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                                       .AddMessage($"Data {JsonConvert.SerializeObject(custInfos)} - DateTime {DateTime.UtcNow} UTC").ToString();

                LogService.Error(logMessage);

                return BadRequest($"Add Customers Initial Smart failed.");
            }

            return Ok("Success");
        }
    }
}