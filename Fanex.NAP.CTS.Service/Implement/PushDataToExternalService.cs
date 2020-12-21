namespace Fanex.NAP.CTS.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using Fanex.NAP.CTS.Criteria.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Service.Implement;
    using Newtonsoft.Json;

    public class PushDataToExternalService : IPushDataToExternalService
    {
        public bool PushCustomerClassificationToMain(IEnumerable<LatestClassificationModel> customerClassifications, int retryTime)
        {
            if (customerClassifications == null || !customerClassifications.Any())
            {
                return true;
            }

            while (retryTime >= 0)
            {
                try
                {
                    var result = SyncCustomerClassToMain(customerClassifications);

                    if (result.ErrorCode != 0 && retryTime == 0)
                    {
                       LogService.Error($"CTS - PushDataToExternalService - exception from MainDB {result.ErrorMsg} with listCust: {JsonConvert.SerializeObject(customerClassifications)}");
                       return false;
                    }
                    else if (result.ErrorCode != 0)
                    {
                        retryTime--;
                    }
                    else
                    {
                        retryTime = -1;
                    }
                }
                catch (Exception ex)
                {
                    if (retryTime == 0)
                    {
                        var jsonDataForLog = JsonConvert.SerializeObject(customerClassifications);

                        LogService.Error($"PushDataToExternalService - PushCustomerClassificationToMain exception {ex.Message} with listCust: {jsonDataForLog}", ex);

                        return false;
                    }

                    retryTime--;
                }
            }

            return true;
        }

        public void PushCustomerClassificationToSoon88(IEnumerable<LatestClassificationModel> customerClassifications)
        {
            var feedDataToSoon88Service = new FeedDataToSoon88Service();

            if (customerClassifications == null || !customerClassifications.Any())
            {
                return;
            }

            try
            {
                var custClassList = customerClassifications.Select(c => new CustomerClassForMainDb()
                {
                    CustID = c.CustId,
                    CustomerClass = c.CustomerClass
                });

                feedDataToSoon88Service.FeedDataToSoon88(custClassList);
            }
            catch (Exception ex)
            {
                var jsonDataForLog = JsonConvert.SerializeObject(customerClassifications);

                var logMessage = new LogMessage(ex.Message)
                .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Feed Data to Soon88 - Start Exception {logMessage} with listCust: {jsonDataForLog}", ex);
            }
        }

        private DBResult SyncCustomerClassToMain(IEnumerable<LatestClassificationModel> customerClassList)
        {
            var repository = new DynamicRepository();

            var criteria = new SyncCustomerClassToMainDbCriteria
            {
                custList = customerClassList.ToXmlString()
            };

            repository.Execute(criteria);

            return new DBResult { ErrorCode = criteria.ErrorCode, ErrorMsg = criteria.ErrorMessage };
        }
    }
}
