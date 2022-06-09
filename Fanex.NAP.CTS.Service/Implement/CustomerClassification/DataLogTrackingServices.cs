namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Newtonsoft.Json;
    using System;
    using System.Linq;

    public class DataLogTrackingServices : IDataLogTrackingServices
    {
        private readonly IDynamicRepository repository;

        public DataLogTrackingServices()
        {
            repository = new DynamicRepository();
        }

        public void LogErrorData(Exception exception, string serviceName)
        {
            try
            {
                var logInfo = new LogErrorDataModel
                {
                    Message = new LogMessage(exception.Message)
                                   .AddMessage($"{serviceName}")
                                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                                   .AddMessage($"{ DateTime.Now}").ToString(),
                    FromFlow = serviceName,
                    SPName = DatabaseInfoGetters.GetDbExceptionInfo(exception)?.Where(c => c.Key == "SpName")?.FirstOrDefault().Value?.ToString(),
                    Parameters = DatabaseInfoGetters.GetDbExceptionInfo(exception)?.Where(c => c.Key == "Parameters")?.FirstOrDefault().Value?.ToString(),
                };

                LogErrorData(logInfo);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                                 .AddMessage($"{serviceName}")
                                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                                 .AddMessage($"{ DateTime.Now}").ToString();

                LogService.Error(logMessage);
            }
        }

        private void LogErrorData(LogErrorDataModel logInfo)
        {
            var criteria = new InputErrorLogInsertCriteria
            {
                LogInfo = JsonConvert.SerializeObject(logInfo)
            };

            repository.Execute(criteria);
        }
    }
}