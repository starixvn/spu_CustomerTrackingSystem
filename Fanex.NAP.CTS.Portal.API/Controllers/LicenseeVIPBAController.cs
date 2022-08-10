namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Model.LogInputData;
    using Fanex.NAP.CTS.Portal.API.ActionFilter;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement.Api;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using Newtonsoft.Json;
    using System;
    using System.Web.Http;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class LicenseeVIPBAController : ApiController
    {
        private readonly ILicenseeVIPBAService licenseeLAPCustomerClassificationService;
        private readonly ILogInputDataService logInputDataService;

        public LicenseeVIPBAController()
        {
            licenseeLAPCustomerClassificationService = new LicenseeVIPBAService();
            logInputDataService = new LogInputDataService();
        }

        /// <summary>
        /// Add Licencee VIP
        /// </summary>
        [HttpPost]
        [Route("api/cts/AddLicVIP")]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.AddLicVIP_API)]
        public IHttpActionResult AddLicVIP([FromBody] LicenseeVIPBAInputModel parameters)
        {
            try
            {
                logInputDataService.LogInputData(new LogInputViewModel { LicenseeVIPBAModel = new LicenseeVIPBALogDataModel { Data = parameters, FromSystem = parameters.IsFromExternal ? FromSystem.External : FromSystem.Internal } }, FunctionId.AddLicVIP_API);

                if (!licenseeLAPCustomerClassificationService.CheckValid(parameters))
                {
                    LogService.Error($"CTS API - AddLicVIP - Invalid input parameter from LAP: {JsonConvert.SerializeObject(parameters)} - DateTime {DateTime.UtcNow} UTC");

                    return BadRequest($"Invalid input parameter. Data: {JsonConvert.SerializeObject(new { parameters.CustIDs, parameters.CreatedBy })}");
                }

                licenseeLAPCustomerClassificationService.InsertLicVIP(parameters);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage($"CTS API - AddLicVIP")
                                       .AddMessage(ex.Message)
                                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                                       .AddMessage($"Data {JsonConvert.SerializeObject(parameters)} - DateTime {DateTime.UtcNow} UTC").ToString();

                LogService.Error(logMessage);

                return InternalServerError(ex);
            }

            return Ok();
        }

        /// <summary>
        /// Remove Licencee VIP
        /// </summary>
        [HttpPost]
        [Route("api/cts/RemoveLicVIP")]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.RemoveLicVIP_API)]
        public IHttpActionResult RemoveLicVIP([FromBody] LicenseeVIPBAInputModel parameters)
        {
            try
            {
                logInputDataService.LogInputData(new LogInputViewModel { LicenseeVIPBAModel = new LicenseeVIPBALogDataModel { Data = parameters, FromSystem = FromSystem.External } }, FunctionId.RemoveLicVIP_API);

                if (!licenseeLAPCustomerClassificationService.CheckValid(parameters))
                {
                    LogService.Error($"CTS API - RemoveLicVIP - Invalid input parameter from LAP: {JsonConvert.SerializeObject(parameters)} - DateTime {DateTime.UtcNow} UTC");

                    return BadRequest($"Invalid input parameter. Data: {JsonConvert.SerializeObject(new { parameters.CustIDs, parameters.CreatedBy })}");
                }

                licenseeLAPCustomerClassificationService.RemoveLicVIP(parameters);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage($"CTS API - RemoveLicVIP")
                                       .AddMessage(ex.Message)
                                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                                       .AddMessage($"Data {JsonConvert.SerializeObject(parameters)} - DateTime {DateTime.UtcNow} UTC").ToString();

                LogService.Error(logMessage);

                return InternalServerError(ex);
            }

            return Ok();
        }

        /// <summary>
        /// Add Licencee BA
        /// </summary>
        [HttpPost]
        [Route("api/cts/AddLicBA")]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.AddLicVIP_API)]
        public IHttpActionResult AddLicBA([FromBody] LicenseeVIPBAInputModel parameters)
        {
            try
            {
                logInputDataService.LogInputData(new LogInputViewModel { LicenseeVIPBAModel = new LicenseeVIPBALogDataModel { Data = parameters, FromSystem = parameters.IsFromExternal ? FromSystem.External : FromSystem.Internal } }, FunctionId.AddLicBA_API);

                if (!licenseeLAPCustomerClassificationService.CheckValid(parameters))
                {
                    LogService.Error($"CTS API - AddLicBA - Invalid input parameter from LAP: {JsonConvert.SerializeObject(parameters)} - DateTime {DateTime.UtcNow} UTC");

                    return BadRequest($"Invalid input parameter. Data: {JsonConvert.SerializeObject(new { parameters.CustIDs, parameters.CreatedBy })}");
                }

                licenseeLAPCustomerClassificationService.InsertLicBA(parameters);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage($"CTS API - AddLicBA")
                                       .AddMessage(ex.Message)
                                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                                       .AddMessage($"Data {JsonConvert.SerializeObject(parameters)} - DateTime {DateTime.UtcNow} UTC").ToString();

                LogService.Error(logMessage);

                return InternalServerError(ex);
            }

            return Ok();
        }

        /// <summary>
        /// Remove Licencee BA
        /// </summary>
        [HttpPost]
        [Route("api/cts/RemoveLicBA")]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.RemoveLicBAP_API)]
        public IHttpActionResult RemoveLicBA([FromBody] LicenseeVIPBAInputModel parameters)
        {
            try
            {
                logInputDataService.LogInputData(new LogInputViewModel { LicenseeVIPBAModel = new LicenseeVIPBALogDataModel { Data = parameters, FromSystem = FromSystem.External } }, FunctionId.RemoveLicBA_API);

                if (!licenseeLAPCustomerClassificationService.CheckValid(parameters))
                {
                    LogService.Error($"CTS API - RemoveLicBA - Invalid input parameter from LAP: {JsonConvert.SerializeObject(parameters)} - DateTime {DateTime.UtcNow} UTC");

                    return BadRequest($"Invalid input parameter. Data: {JsonConvert.SerializeObject(new { parameters.CustIDs, parameters.CreatedBy })}");
                }

                licenseeLAPCustomerClassificationService.RemoveLicBA(parameters);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage($"CTS API - RemoveLicBA")
                                       .AddMessage(ex.Message)
                                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                                       .AddMessage($"Data {JsonConvert.SerializeObject(parameters)} - DateTime {DateTime.UtcNow} UTC").ToString();

                LogService.Error(logMessage);

                return InternalServerError(ex);
            }

            return Ok();
        }

        /// <summary>
        /// Remove Licencee BA by Site
        /// </summary>
        [HttpPost]
        [Route("api/cts/RemoveLicBABySite")]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.RemoveLicBABySite_API)]
        public IHttpActionResult RemoveLicBABySite([FromBody] RemoveLicenseeBABySiteInputModel parameters)
        {
            try
            {
                logInputDataService.LogInputData(new LogInputViewModel { RemoveLicenseeBABySiteModel = new RemoveLicenseeBABySiteLogDataModel { Data = parameters, FromSystem = FromSystem.External } }, FunctionId.RemoveLicBABySite_API);

                if (!licenseeLAPCustomerClassificationService.CheckValid(parameters))
                {
                    LogService.Error($"CTS API - RemoveLicBABySite - Invalid input parameter from LAP: {JsonConvert.SerializeObject(parameters)} - DateTime {DateTime.UtcNow} UTC");

                    return BadRequest($"Invalid input parameter. Data: {JsonConvert.SerializeObject(parameters)}");
                }

                licenseeLAPCustomerClassificationService.RemoveLicBABySite(parameters);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage($"CTS API - RemoveLicBABySite")
                                       .AddMessage(ex.Message)
                                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                                       .AddMessage($"Data {JsonConvert.SerializeObject(parameters)} - DateTime {DateTime.UtcNow} UTC").ToString();

                LogService.Error(logMessage);

                return InternalServerError(ex);
            }

            return Ok();
        }
    }
}