namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
    using Fanex.NAP.CTS.Criteria.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Implement;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class CustomerClassificationService : ICustomerClassificationService
    {
        private readonly IDynamicRepository repository;
        private readonly IAntiFraudCenterService _antiFraudCenterService;

        public CustomerClassificationService()
        {
            repository = new DynamicRepository();
            _antiFraudCenterService = new AntiFraudCenterService();
        }

        public bool PushDataToExternalPackge(IEnumerable<long> custIds, PushDataToExternalModel paras, FromFlow fromFlow)
        {
            bool isSuccess = true;

            if (custIds != null && custIds.Any())
            {
                custIds = custIds.Distinct();

                isSuccess= PushDataToExternal(custIds, paras);

                _antiFraudCenterService.InsertAFCAccounts(custIds, fromFlow, paras.IsSyncAFC);
            }

            return isSuccess;
        }

        public bool PushDataToExternal(IEnumerable<long> custIdsToSyncCC, PushDataToExternalModel paras)
        {
            if (custIdsToSyncCC == null || !custIdsToSyncCC.Any())
            {
                return true;
            }

            bool isSuccess = true;

            var custIdsChunk = custIdsToSyncCC.Distinct().Chunk(paras.ExternalBatchSize);

            foreach (var batchCustIds in custIdsChunk)
            {
                var latestcustClassification = GetLatestCCAndDangerLevel(batchCustIds);

                if (latestcustClassification != null && latestcustClassification.Any())
                {
                    isSuccess = PushCCToMainDB(latestcustClassification, Consts.Retry);

                    if (paras.IsPushCCToSoon88)
                    {
                        PushCustomerClassificationToSoon88(latestcustClassification);
                    }

                    if (paras.IsUpdateDangerToMainDB && isSuccess)
                    {
                        isSuccess = UpdateDangerLevelToMainDB(latestcustClassification);
                    }
                }
            }

            return isSuccess;
        }

        public bool UpdateDangerLevelToMainDB(IEnumerable<LatestCCDangerLevelModel> lastestClassifications)
        {
            if (lastestClassifications != null && lastestClassifications.Any())
            {
                var groupDangerType = lastestClassifications.Where(c=>c.DangerLevel != null).GroupBy(c => c.DangerLevelType);

                if (groupDangerType != null && groupDangerType.Any())
                {
                    foreach (var dangerBatch in groupDangerType)
                    {
                        var updateDangerLevel = dangerBatch.Select(c => new DangerLevelModel()
                        {
                            CustId = c.CustID,
                            DangerLevel = c.DangerLevel.Value
                        });

                        var isUpdateSuccessful = RetryUpdateDangerLevelToMainDB(updateDangerLevel, dangerBatch.Key.Value, Consts.Retry);

                        if (!isUpdateSuccessful)
                        {
                            return isUpdateSuccessful;
                        }
                    }
                }
            }

            return true;
        }

        public bool PushCCToMainDB(IEnumerable<LatestCCDangerLevelModel> lastestClassifications, int retryTime)
        {
            bool isSuccess = true;

            if (lastestClassifications != null && lastestClassifications.Any())
            {
                var custClassification = lastestClassifications.Select(c => new LatestClassificationModel()
                {
                    CustId = c.CustID,
                    Category = c.CategoryID,
                    CustomerClass = c.CustomerClass
                });

                isSuccess = RetryPushCCToMainDB(custClassification, retryTime);
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
                    var result = UpdateCCToMainDB(customerClassifications);

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

        public void PushCustomerClassificationToSoon88(IEnumerable<LatestCCDangerLevelModel> customerClassifications)
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
                    CustID = c.CustID,
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

        public bool RetryUpdateDangerLevelToMainDB(IEnumerable<DangerLevelModel> dangerLevels, int dangerType, int retryTime)
        {
            if (dangerLevels == null || !dangerLevels.Any())
            {
                return true;
            }

            while (retryTime >= 0)
            {
                try
                {
                    var result = UpdateDangerLevelToMainDB(dangerLevels, dangerType);

                    if (result.ErrorCode != 0 && retryTime == 0)
                    {
                        LogService.Error($"CTS - UpdateDangerLevelToMainDB throws exception {result.ErrorMsg} with listCust: {JsonConvert.SerializeObject(dangerLevels)}");

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
                        LogService.Error($"CTS - UpdateDangerLevelToMainDB throws exception {ex.Message} with listCust: {JsonConvert.SerializeObject(dangerLevels)}");

                        return false;
                    }

                    retryTime--;
                }
            }

            return true;
        }

        #region DataAccess Layer

        public DBResult UpdateCCToMainDB(IEnumerable<LatestClassificationModel> customerClassList)
        {
            var criteria = new SyncCustomerClassToMainDbCriteria
            {
                custList = customerClassList.ToXmlString()
            };

            repository.Execute(criteria);

            return new DBResult { ErrorCode = criteria.ErrorCode, ErrorMsg = criteria.ErrorMessage };
        }

        public IEnumerable<LatestCCDangerLevelModel> GetLatestCCAndDangerLevel(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                var criteria = new GetLastestCCDangerLevelCriteria
                {
                    CustIds = string.Join(",", custIds)
                };

                return repository.Fetch<LatestCCDangerLevelModel>(criteria) ?? Enumerable.Empty<LatestCCDangerLevelModel>();
            }

            return new List<LatestCCDangerLevelModel>();
        }

        public IEnumerable<LatestCCDangerLevelModel> GetLatestCCAndDangerLevel(string custIds)
        {
            if (!string.IsNullOrEmpty(custIds))
            {
                var criteria = new GetLastestCCDangerLevelCriteria
                {
                    CustIds = custIds
                };

                return repository.Fetch<LatestCCDangerLevelModel>(criteria) ?? Enumerable.Empty<LatestCCDangerLevelModel>();
            }

            return new List<LatestCCDangerLevelModel>();
        }

        public DBResult UpdateDangerLevelToMainDB(IEnumerable<DangerLevelModel> dangerLevel, int dangerType)
        {
            if (dangerLevel != null && dangerLevel.Any())
            {
                var criteria = new DangerLevelUpdateCriteria
                {
                    CustList = dangerLevel.ToXmlString(),
                    DangerType = dangerType
                };

                repository.Execute(criteria);

                return new DBResult { ErrorCode = criteria.ErrorCode, ErrorMsg = criteria.ErrorMessage };
            }

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }

        #endregion DataAccess Layer
    }
}