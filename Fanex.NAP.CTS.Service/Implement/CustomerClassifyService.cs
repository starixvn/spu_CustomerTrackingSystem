namespace Fanex.NAP.CTS.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.Account.ChainToEvidence;
    using Fanex.NAP.CTS.Criteria.CustomerClassify;
    using Fanex.NAP.CTS.Criteria.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Newtonsoft.Json;

    public class CustomerClassifyService : ICustomerClassifyService
    {
        private readonly IPushDataToExternalService _pushDataToExternalService;

        public CustomerClassifyService()
        {
            _pushDataToExternalService = new PushDataToExternalService();
        }

        public void RescanCustomerClassify(List<int> custIds)
        {
            // Check and rescan problem account
            var existedCustEvidences = GetExistedCustomerEvidences(string.Join(",", custIds)).ToList();
            var existedProblemAccounts = GetExistedProblemAccounts(string.Join(",", custIds)).ToList();

            var isRescanPA = (existedCustEvidences != null && existedCustEvidences.Any()) || (existedProblemAccounts != null && existedProblemAccounts.Any());

            if (isRescanPA)
            {
                RescanProblemAccount(existedProblemAccounts, existedCustEvidences);

                custIds.RemoveAll(item => existedProblemAccounts.Select(c => c.CustID).Contains(item));
                custIds.RemoveAll(item => existedCustEvidences.Select(c => c.CustId).Contains(item));
            }

            // Check and rescan normal account
            if (custIds != null && custIds.Any())
            {
                RescanNormalAccount(custIds);
            }
        }

        private IEnumerable<CustEvidenceModel> GetExistedCustomerEvidences(string custIds)
        {
            var existedEvidences = new List<CustEvidenceModel>();

            if (string.IsNullOrEmpty(custIds))
            {
                return existedEvidences;
            }

            var criteria = new GetExistedEvidencesCriteria
            {
                CustIds = custIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                existedEvidences = db.Query<CustEvidenceModel>(criteria).ToList();
            }

            return existedEvidences ?? new List<CustEvidenceModel>();
        }

        private IEnumerable<MultipleProblemAccountsModel> GetExistedProblemAccounts(string custIds)
        {
            var problemAccounts = new List<MultipleProblemAccountsModel>();

            if (string.IsNullOrEmpty(custIds))
            {
                return problemAccounts;
            }

            var problemAccountsNAPCM = GetProblemAccounts(custIds, null).ToList();
            var problemAccountsCN88 = GetProblemAccounts_CN88(custIds, null).ToList();

            if (problemAccountsNAPCM != null && problemAccountsNAPCM.Any())
            {
                problemAccounts.AddRange(problemAccountsNAPCM);
            }

            if (problemAccountsCN88 != null && problemAccountsCN88.Any())
            {
                problemAccounts.AddRange(problemAccountsCN88);
            }

            return problemAccounts;
        }

        private IEnumerable<MultipleProblemAccountsModel> GetProblemAccounts(string custIDs, DateTime? dateTime)
        {
            var criteria = new GetProblemAccountsCriteria
            {
                CustIds = custIDs,
                Datetime = dateTime
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MultipleProblemAccountsModel>(criteria).ToList();
            }
        }

        private IEnumerable<MultipleProblemAccountsModel> GetProblemAccounts_CN88(string custIDs, DateTime? dateTime)
        {
            var criteria = new GetProblemAccountsCN88Criteria
            {
                CustIds = custIDs,
                Datetime = dateTime
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<MultipleProblemAccountsModel>(criteria).ToList();
            }
        }

        private void RescanProblemAccount(List<MultipleProblemAccountsModel> problemAccounts, List<CustEvidenceModel> existedEvidences)
        {
            var custIdsToSyncCC = new List<int>();

            // Re-Sync Customer Evidence to category in CTS
            var result = SyncCustomerEvidenceToCategory(existedEvidences);

            if (string.IsNullOrEmpty(result.ErrorMessage) && result.CustIds != null && result.CustIds.Any())
            {
                custIdsToSyncCC.AddRange(result.CustIds);
            }

            // Re-Insert Problem Account in CTS
            result = InsertProblemAccountToCTS(problemAccounts);

            if (string.IsNullOrEmpty(result.ErrorMessage) && result.CustIds != null && result.CustIds.Any())
            {
                custIdsToSyncCC.AddRange(result.CustIds);
            }

            var custIdsChunk = custIdsToSyncCC.Distinct().Chunk(ConfigSettings.PushCCToExternalChunkSize);

            foreach (var custids in custIdsChunk)
            {
                var lastestClassifications = GetLatestClassification(string.Join(",", custids));

                if (lastestClassifications != null && lastestClassifications.Any())
                {
                    bool isSyncMainSuccessfully = _pushDataToExternalService.PushCustomerClassificationToMain(lastestClassifications, 3);

                    if (isSyncMainSuccessfully)
                    {
                        UpdateAppliedCustomerClassification(JsonConvert.SerializeObject(lastestClassifications));
                    }

                    _pushDataToExternalService.PushCustomerClassificationToSoon88(lastestClassifications);
                }
            }
        }

        private InsertCategoryResultModel SyncCustomerEvidenceToCategory(List<CustEvidenceModel> custEvidences)
        {
            try
            {
                if (custEvidences == null || !custEvidences.Any())
                {
                    return new InsertCategoryResultModel() { ErrorMessage = string.Empty, CustIds = null };
                }

                var evidencesChunk = custEvidences.Chunk(ConfigSettings.UpdateInternalChunkSize);

                var custIds = new List<int>();

                foreach (var evidences in evidencesChunk)
                {
                    custIds.AddRange(SyncCustomerEvidenceToCategory(JsonConvert.SerializeObject(evidences)));
                }

                return new InsertCategoryResultModel { ErrorMessage = string.Empty, CustIds = custIds };
            }
            catch (Exception ex)
            {
                LogService.Error($"Web - CustomerClassifyService - SyncCustomerEvidenceToCategory exception: {ex.Message} - Data: {JsonConvert.SerializeObject(custEvidences)}", ex);

                return new InsertCategoryResultModel { ErrorMessage = ex.Message, CustIds = null };
            }
        }

        private List<int> SyncCustomerEvidenceToCategory(string custEvidencesJson)
        {
            var custIds = new List<int>();
            var repository = new DynamicRepository();

            if (string.IsNullOrEmpty(custEvidencesJson))
            {
                return custIds;
            }

            var criteria = new CustEvidenceSyncToCategoryCriteria
            {
                CustEvidencesJson = custEvidencesJson
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                custIds = repository.Fetch<int>(criteria).ToList();
            }

            if (!string.IsNullOrEmpty(criteria.ErrorMessage))
            {
                LogService.Error($"Web - CustomerClassifyService - SyncCustomerEvidenceToCategory failed: {criteria.ErrorMessage} - Data: {custEvidencesJson}");
            }

            return (custIds != null) ? custIds : new List<int>();
        }

        private InsertCategoryResultModel InsertProblemAccountToCTS(List<MultipleProblemAccountsModel> problemAccounts)
        {
            try
            {
                if (problemAccounts == null || !problemAccounts.Any())
                {
                    return new InsertCategoryResultModel() { ErrorMessage = string.Empty, CustIds = null };
                }

                var probalemAccountsChunk = problemAccounts.Chunk(ConfigSettings.UpdateInternalChunkSize);

                var custIds = new List<int>();

                foreach (var problemAccs in probalemAccountsChunk)
                {
                    var result = problemAccs.Select(x => new { CustId = x.CustID, CategoryId = x.ReasonID });

                    custIds.AddRange(InsertProblemAccount(JsonConvert.SerializeObject(result)));
                }

                return new InsertCategoryResultModel() { ErrorMessage = string.Empty, CustIds = custIds };
            }
            catch (Exception ex)
            {
                LogService.Error($"Web - CustomerClassifyService - InsertProblemAccountToCTS exception: {ex.Message} - Data: { JsonConvert.SerializeObject(problemAccounts)}", ex);

                return new InsertCategoryResultModel { ErrorMessage = ex.Message, CustIds = null };
            }
        }

        private List<int> InsertProblemAccount(string problemAccountJson)
        {
            var repository = new DynamicRepository();
            var custIds = new List<int>();

            if (string.IsNullOrEmpty(problemAccountJson))
            {
                return custIds;
            }

            var criteria = new InsertProblemAccountCriteria
            {
                ProblemAccountJson = problemAccountJson
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                custIds = repository.Fetch<int>(criteria).ToList();
            }

            if (!string.IsNullOrEmpty(criteria.ErrorMessage))
            {
                LogService.Error($"Web - CustomerClassifyService - InsertProblemAccount failed: {criteria.ErrorMessage} - Data: {problemAccountJson}");
            }

            return (custIds != null) ? custIds : new List<int>();
        }

        public IEnumerable<LatestClassificationModel> GetLatestClassification(string custIds)
        {
            if (string.IsNullOrEmpty(custIds))
            {
                return new List<LatestClassificationModel>();
            }

            var criteria = new GetLastestClassificationCriteria
            {
                CustIds = custIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<LatestClassificationModel>(criteria);
            }
        }

        public void UpdateAppliedCustomerClassification(string jsonData)
        {
            var repository = new DynamicRepository();

            var criteria = new UpdateAppliedCustomerClassificationCriteria
            {
                ProblemCustomerJson = jsonData
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                repository.Fetch<string>(criteria);

                if (!string.IsNullOrEmpty(criteria.ErrorMessage))
                {
                    LogService.Error($"Web - CustomerClassifyService - UpdateAppliedCustomerClassification failed {criteria.ErrorMessage} with data: {jsonData}");
                }
            }
        }

        private void RescanNormalAccount(List<int> customers)
        {
            if (customers == null || !customers.Any())
            {
                return;
            }

            var custIdsChunk = customers.Chunk(ConfigSettings.PushCCToExternalChunkSize).ToList();

            foreach (var custIds in custIdsChunk)
            {
                var custIdsToBeRescaned = string.Join(",", custIds);

                ClassifyCustomerGeneralCategoryDaily(custIdsToBeRescaned, false);
            }
        }

        private void ClassifyCustomerGeneralCategoryDaily(string customers, bool isCheckLast14Days)
        {
            var repository = new DynamicRepository();

            var criteria = new NormalAccountsClassificationGeneralCategorizeDailyCriteria
            {
                ListCustIds = customers,
                IsCheckLast14Days = isCheckLast14Days
            };

            repository.Execute(criteria);
        }

    }
}
