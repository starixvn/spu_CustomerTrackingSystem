namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Criteria.ImportEvidences;
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CustomerEvidenceService : ICustomerEvidenceService
    {
        private readonly IDynamicRepository repository;

        public CustomerEvidenceService()
        {
            repository = new DynamicRepository();
        }

        public void UpdateBetLimitOfFlaggedCustomer(string custIds)
        {
            var criteria = new UpdateBetLimitByEvidenceAddingCriteria
            {
                CustIDList = custIds
            };

            repository.Execute(criteria);
        }

        public void UpdateBetLimitOfUnflaggedCustomer(string custIds)
        {
            var criteria = new UpdateBetLimitByEvidenceRemovingCriteria
            {
                CustIDList = custIds
            };

            repository.Execute(criteria);
        }

        public AddEvidenceResultModel AddCustEvidenceMultipleByCustId(string custIds, int evidenceId, int userId)
        {
            var result = new List<AddMultiEvidenceResultModel>();

            var custEvidenceList = custIds.Split(',').Select(ids => new AddAccountEvidenceModel()
            {
                CustID = Convert.ToInt32(ids),
                EvidenceID = evidenceId
            });

            var jsonData = JsonConvert.SerializeObject(custEvidenceList);

            var criteria = new InsertMultipleCustEviByCustIdCriteria
            {
                NewAccountEvidences = jsonData,
                UserId = userId,
                ErrorMessage = string.Empty
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                result = db.Query<AddMultiEvidenceResultModel>(criteria).ToList();
            }

            return new AddEvidenceResultModel { ErrorMessage = criteria.ErrorMessage, Eivdences = result };
        }

        public IEnumerable<long> CheckCleanCustFlaggedEvidence(string ctsCustIds)
        {
            var criteria = new CheckCleanCustFlaggedEvidenceCriteria
            {
                CTSCustIds = ctsCustIds
            };

            return repository.Fetch<long>(criteria);
        }
    }
}