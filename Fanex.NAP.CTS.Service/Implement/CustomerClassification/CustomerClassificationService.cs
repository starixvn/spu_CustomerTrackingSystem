namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
    using Fanex.NAP.CTS.Criteria.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Resource.Consts;
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

                isSuccess = PushDataToExternal(custIds, paras);

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
                    isSuccess = PushCCToMainDB(latestcustClassification, paras.RoleGroup, Consts.Retry);

                    if (paras.IsUpdateDangerToMainDB && isSuccess)
                    {
                        isSuccess = UpdateDangerLevelToMainDB(latestcustClassification);
                    }
                }
            }

            return isSuccess;
        }

        private bool UpdateDangerLevelToMainDB(IEnumerable<LatestCCDangerLevelModel> lastestClassifications)
        {
            if (lastestClassifications != null && lastestClassifications.Any())
            {
                var groupDangerType = lastestClassifications.Where(c => c.DangerLevel != null).GroupBy(c => c.DangerLevelType);

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

        private bool PushCCToMainDB(IEnumerable<LatestCCDangerLevelModel> lastestClassifications, RoleGroupType roleGroupType, int retryTime)
        {
            bool isSuccess = true;

            if (lastestClassifications != null && lastestClassifications.Any())
            {
                var custClassification = lastestClassifications.Select(c => new LatestClassificationModel()
                {
                    CustId = c.CustID,
                    Category = c.CategoryID,
                    CustomerClass = c.CustomerClass
                })
                .GroupBy(c => new { c.CustId, c.CustomerClass, c.Category })
                .Select(g => g.First());

                isSuccess = RetryPushCCToMainDB(custClassification, roleGroupType, retryTime);
            }

            return isSuccess;
        }

        public bool RetryPushCCToMainDB(IEnumerable<LatestClassificationModel> customerClassifications, RoleGroupType roleGroupType, int retryTime)
        {
            if (customerClassifications == null || !customerClassifications.Any())
            {
                return true;
            }

            while (retryTime >= 0)
            {
                try
                {
                    var result = UpdateCCToMainDB(customerClassifications, roleGroupType);

                    if (result.ErrorCode == 200)
                    {
                        LogService.Error($"CTS - PushDataToExternalService - exception not existing CustIDs in mainDB, error msg: {result.ErrorMsg}");
                    }

                    if (result.ErrorCode == 200 || result.ErrorCode == 0)
                    {
                        return true;
                    }

                    if (retryTime == 0)
                    {
                        LogService.Error($"CTS - PushDataToExternalService - exception from MainDB with error code: {result.ErrorCode}, error msg: {result.ErrorMsg} with listCust: {JsonConvert.SerializeObject(customerClassifications)}");

                        return false;
                    }

                    retryTime--;
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

        private DBResult UpdateCCToMainDB(IEnumerable<LatestClassificationModel> customerClassList, RoleGroupType roleGroupType)
        {
            var criteria = new SyncCustomerClassToMainDbCriteria
            {
                Mode = (int)roleGroupType,
                CustList = customerClassList.ToXmlString()
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