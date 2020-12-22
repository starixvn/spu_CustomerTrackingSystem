namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using System.Collections.Generic;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using Newtonsoft.Json;
    using Fanex.NAP.CTS.Criteria.ImportEvidences;
    using Fanex.Data.Repository;
    using System.Linq;
    using System;
    using Fanex.Data;

    public class CustomerEvidenceService : ICustomerEvidenceService
    {
        private IDynamicRepository _repository;
        public CustomerEvidenceService()
        {
            _repository = new DynamicRepository();
        }
        public IEnumerable<int> GetAssociatedAccounts(GetAssociatedAccountByCTSCustIdsCriteria criteria)
        {
            return DataService.GetData<int>(criteria);
        }

        public IEnumerable<int> GetRemoveEvidenceAssociatedAccounts(GetRemoveEvidenceAssociatedAccountCriteria criteria)
        {
            return DataService.GetData<int>(criteria);
        }

        public IEnumerable<int> GetCleanAccounts(GetCleanAccountByCTSCustIdsCriteria criteria)
        {
            return DataService.GetData<int>(criteria);
        }

        public void UpdateBetLimitOfFlaggedCustomer(UpdateBetLimitByEvidenceAddingCriteria criteria)
        {
            DataService.Execute(criteria);
        }

        public void UpdateBetLimitOfUnflaggedCustomer(UpdateBetLimitByEvidenceRemovingCriteria criteria)
        {
            DataService.Execute(criteria);
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
    }
}