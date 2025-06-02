namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Newtonsoft.Json;
    using System;

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
                    Message = exception.CreateLogMessage(serviceName),
                    FromFlow = serviceName,
                    SPName = exception.GetDbExceptionInfo(c => c.Key == "SpName"),
                    Parameters = exception.GetDbExceptionInfo(c => c.Key == "Parameters"),
                };

                LogErrorData(logInfo);
            }
            catch (Exception ex)
            {
                ex.LogError(serviceName);
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