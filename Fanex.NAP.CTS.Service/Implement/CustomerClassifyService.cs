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
            var problemAccounts = CheckToRescanPA(string.Join(",", custIds)).ToList();

            if (problemAccounts != null && problemAccounts.Any())
            {
                RescanProblemAccount(problemAccounts);

                custIds.RemoveAll(item => problemAccounts.Select(c => c.CustID).Contains(item));
            }

            // Check and rescan normal account
            if (custIds != null && custIds.Any())
            {
                RescanNormalAccount(custIds);
            }

            return isSuccess;
        }

        private IEnumerable<MultipleProblemAccountsModel> CheckToRescanPA(string custIds)
        {
            var problemAccountList = new List<MultipleProblemAccountsModel>();

            if (string.IsNullOrEmpty(custIds))
            {
                return problemAccountList;
            }

            var problemAccounts = GetProblemAccounts(custIds, null).ToList();

            problemAccountList.AddRange(problemAccounts);

            problemAccounts = GetProblemAccounts_CN88(custIds, null).ToList();

            problemAccountList.AddRange(problemAccounts);

            return problemAccountList;
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

        private void RescanProblemAccount(List<MultipleProblemAccountsModel> problemAccounts)
        {
            if (problemAccounts == null && !problemAccounts.Any())
            {
                return;
            }

            var output = InsertProblemAccountToCTS(problemAccounts);

            if (string.IsNullOrEmpty(output.ErrorMessage) && output.CustIds != null && output.CustIds.Any())
            {
                var latestCustIdsChunk = output.CustIds.Chunk(ConfigSettings.PushCCToExternalChunkSize);

                foreach (var custids in latestCustIdsChunk)
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
            else if (!string.IsNullOrEmpty(output.ErrorMessage))
            {

                LogService.Error($"CTS - Rescan Problem Account Exception: {output.ErrorMessage} - Data: {JsonConvert.SerializeObject(problemAccounts)}");

                return;
            }
        }

        private InsertCategoryResultModel InsertProblemAccountToCTS(List<MultipleProblemAccountsModel> problemAccounts)
        {
            var custIds = new List<int>();

            var result = problemAccounts.Select(x => new
            {
                CustId = x.CustID,
                CategoryId = x.ReasonID
            });

            var jsonData = JsonConvert.SerializeObject(result);

            var repository = new DynamicRepository();

            var criteria = new InsertProblemAccountCriteria
            {
                ProblemAccountJson = jsonData
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                custIds = repository.Fetch<int>(criteria).ToList();
            }

            return new InsertCategoryResultModel() { ErrorMessage = criteria.ErrorMessage, CustIds = custIds };
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

    }
}
