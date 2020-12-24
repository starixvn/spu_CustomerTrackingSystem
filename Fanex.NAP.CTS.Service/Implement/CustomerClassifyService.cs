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

        public bool RescanCustomerClassify(List<int> custIds)
        {
            var isSuccess = true;

            // Check and rescan problem account
            var existedEvidences = CheckExistedEvidenceToRescanPA(string.Join(",", custIds)).ToList();
            var problemAccounts = CheckToRescanPAFromNapCM(string.Join(",", custIds)).ToList();

            if ((problemAccounts != null && problemAccounts.Any()) || (existedEvidences != null && existedEvidences.Any()))
            {
                RescanProblemAccount(problemAccounts, existedEvidences);

                custIds.RemoveAll(item => problemAccounts.Select(c => c.CustID).Contains(item));
                custIds.RemoveAll(item => existedEvidences.Select(c => c.CustId).Contains(item));
            }

            // Check and rescan normal account
            if (custIds != null && custIds.Any())
            {
                RescanNormalAccount(custIds);
            }

            return isSuccess;
        }

        private IEnumerable<CustEvidenceModel> CheckExistedEvidenceToRescanPA(string custIds)
        {
            var existedEvidences = new List<CustEvidenceModel>();

            if (string.IsNullOrEmpty(custIds))
            {
                return existedEvidences;
            }

            existedEvidences = GetExistedEvidences(custIds).ToList();

            return existedEvidences ?? new List<CustEvidenceModel>();
        }

        private IEnumerable<MultipleProblemAccountsModel> CheckToRescanPAFromNapCM(string custIds)
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

        public IEnumerable<CustEvidenceModel> GetExistedEvidences(string custIDs)
        {
            var criteria = new GetExistedEvidencesCriteria
            {
                CustIds = custIDs
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustEvidenceModel>(criteria).ToList();
            }
        }

        private void RescanProblemAccount(List<MultipleProblemAccountsModel> problemAccounts, List<CustEvidenceModel> existedEvidences)
        {
            var result1 = InsertCustEvidenceToCategory(existedEvidences);

            var result2 = InsertProblemAccountToCTS(problemAccounts);

            if (string.IsNullOrEmpty(result1.ErrorMessage) || string.IsNullOrEmpty(result2.ErrorMessage))
            {
                var custIdsToSyncCC = new List<int>();

                if (result1.CustIds != null && result1.CustIds.Any())
                {
                    custIdsToSyncCC.AddRange(result1.CustIds);
                }
                else if (result2.CustIds != null && result2.CustIds.Any())
                {
                    custIdsToSyncCC.AddRange(result2.CustIds);
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
        }

        private InsertCategoryResultModel InsertCustEvidenceToCategory(List<CustEvidenceModel> existedEvidences)
        {
            try
            {
                if(existedEvidences == null || !existedEvidences.Any())
                {
                    return new InsertCategoryResultModel() { ErrorMessage = string.Empty, CustIds = null };
                }

                var evidencesChunk = existedEvidences.Chunk(ConfigSettings.UpdateInternalChunkSize);

                var custIds = new List<int>();

                foreach (var evidences in evidencesChunk)
                {
                    custIds.AddRange(DataAccess4InsertCategory(JsonConvert.SerializeObject(evidences)));
                }

                return new InsertCategoryResultModel { ErrorMessage = string.Empty, CustIds = custIds };
            }
            catch (Exception ex)
            {
                LogService.Error($"Web - Rescan PA - SyncCustomerEvidenceToCategory exception: {ex.Message} - Data: {JsonConvert.SerializeObject(existedEvidences)}", ex);

                return new InsertCategoryResultModel { ErrorMessage = ex.Message, CustIds = null };
            }
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

                    custIds.AddRange(DataAccess4InsertPA(JsonConvert.SerializeObject(result)));
                }

                return new InsertCategoryResultModel() { ErrorMessage = string.Empty, CustIds = custIds };
            }
            catch (Exception ex)
            {
                LogService.Error($"Web - Rescan PA - Insert Problem Acocunts exception: {ex.Message} - Data: { JsonConvert.SerializeObject(problemAccounts)}", ex);

                return new InsertCategoryResultModel { ErrorMessage = ex.Message, CustIds = null };
            }
        }

        public IEnumerable<LatestClassificationModel> GetLatestClassification(string custIds)
        {
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
                    LogService.Error($"CTS Web - Update Applied CustomerClassification failed {criteria.ErrorMessage} with data: {jsonData}");
                }
            }
        }

        private bool RescanNormalAccount(List<int> customers)
        {
            if (customers != null || customers.Any())
            {
                var custIdsChunk = customers.Chunk(ConfigSettings.PushCCToExternalChunkSize).ToList();

                foreach (var custIds in custIdsChunk)
                {
                    var custIdsToBeRescaned = string.Join(",", custIds);

                    ClassifyCustomerGeneralCategoryDaily(custIdsToBeRescaned, false);
                }
            }

            return true;
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

        private List<int> DataAccess4InsertCategory(string jsonData)
        {
            var custIds = new List<int>();
            var repository = new DynamicRepository();

            if (string.IsNullOrEmpty(jsonData))
            {
                return custIds;
            }

            var criteria = new CustEvidenceSyncToCategoryCriteria
            {
                CustEvidencesJson = jsonData
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                custIds = repository.Fetch<int>(criteria).ToList();
            }

            if (!string.IsNullOrEmpty(criteria.ErrorMessage))
            {
                LogService.Error($"Web - Rescan PA - SyncCustomerEvidenceToCategory exception: {criteria.ErrorMessage} - Data: {jsonData}");
            }

            return (custIds != null) ? custIds : new List<int>();
        }

        private List<int> DataAccess4InsertPA(string jsonData)
        {
            var repository = new DynamicRepository();
            var custIds = new List<int>();

            if (string.IsNullOrEmpty(jsonData))
            {
                return custIds;
            }

            var criteria = new InsertProblemAccountCriteria
            {
                ProblemAccountJson = jsonData
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                custIds = repository.Fetch<int>(criteria).ToList();
            }

            if (!string.IsNullOrEmpty(criteria.ErrorMessage))
            {
                LogService.Error($"Web - Rescan PA - Insert Problem Acocunts exception: {criteria.ErrorMessage} - Data: {jsonData}");
            }
            
            return (custIds != null) ? custIds : new List<int>();
        }
    }
}
