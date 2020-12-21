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
    using Fanex.NAP.CTS.Model.CustomerClassify;
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

            var latestCustIds = InsertProblemAccountToCTS(problemAccounts);

            if (latestCustIds == null && !latestCustIds.Any())
            {
                return;
            }

            var latestCustIdsChunk = latestCustIds.Chunk(ConfigSettings.PushCCToExternalChunkSize);

            foreach (var custids in latestCustIdsChunk)
            {
                var lastestClassifications = GetLatestClassification(string.Join(",", custids));

                if (lastestClassifications != null && lastestClassifications.Any())
                {
                    _pushDataToExternalService.PushCustomerClassificationToMain(lastestClassifications, 2);

                    UpdateAppliedCustomerClassification(JsonConvert.SerializeObject(lastestClassifications));

                    _pushDataToExternalService.PushCustomerClassificationToSoon88(lastestClassifications);
                }
            }
        }

        private IEnumerable<int> InsertProblemAccountToCTS(List<MultipleProblemAccountsModel> problemAccounts)
        {
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
                return repository.Fetch<int>(criteria);
            }
        }

        private IEnumerable<LatestClassificationModel> GetLatestClassification(string custIds)
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

        private void UpdateAppliedCustomerClassification(string jsonData)
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
                    LogService.Error($"RescanCustomerClassifyService UpdateAppliedCustomerClassification failed {criteria.ErrorMessage} with data: {jsonData}");
                }
            }
        }

        private bool RescanProblemAccount()
        {
            throw new NotImplementedException();
        }

        private bool RescanNormalAccount(List<int> customers)
        {
            if (customers != null || customers.Any())
            {
                var custIdsChunk = customers.Chunk(ConfigSettings.PushCCToExternalChunkSize).ToList();

                foreach (var custIds in custIdsChunk)
                {
                    var lastestClassifications = GetLatestClassification(string.Join(",", custIds));

                    var custIdsToBeRescaned = string.Join(",", lastestClassifications.Select(cust => cust.CustId));

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
