namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using Fanex.NAP.CTS.Criteria.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Service.Implement;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public class PushDataToExternalService : IPushDataToExternalService
    {
        private readonly ICustomerClassifyService customerClassifyService;
        private readonly IDynamicRepository repository;

        public PushDataToExternalService()
        {
            customerClassifyService = new CustomerClassifyService();
            repository = new DynamicRepository();
        }

        public bool PushDataToExternal(IEnumerable<long> custIds, PushDataToExternalModel paras)
        {
            if (custIds == null || !custIds.Any())
            {
                return true;
            }

            var custIdsChunk = custIds.Distinct().Chunk(paras.ExternalBatchSize);

            int retryTime = Convert.ToInt32(ConfigurationManager.AppSettings["RetryTime"]);

            foreach (var custids in custIdsChunk)
            {
                var lastestClassifications = customerClassifyService.GetLatestClassification(string.Join(",", custids));

                if (lastestClassifications != null && lastestClassifications.Any())
                {
                    PushCCToMainDB(lastestClassifications, retryTime);

                    if (paras.IsPushCCToSoon88)
                    {
                        PushCustomerClassificationToSoon88(lastestClassifications);
                    }
                }
            }

            return true;
        }

        public bool PushCCToMainDB(IEnumerable<LatestClassificationModel> lastestClassifications, int retryTime)
        {
            bool isSuccess = true;

            if (lastestClassifications != null && lastestClassifications.Any())
            {
                isSuccess = RetryPushCCToMainDB(lastestClassifications, retryTime);

                if (isSuccess)
                {
                    customerClassifyService.UpdateAppliedCustomerClassification(JsonConvert.SerializeObject(lastestClassifications));
                }
            }

            return isSuccess;
        }

        public bool RetryPushCCToMainDB(IEnumerable<LatestClassificationModel> customerClassifications, int retryTime)
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
            var criteria = new SyncCustomerClassToMainDbCriteria
            {
                custList = customerClassList.ToXmlString()
            };

            repository.Execute(criteria);

            return new DBResult { ErrorCode = criteria.ErrorCode, ErrorMsg = criteria.ErrorMessage };
        }
    }
}