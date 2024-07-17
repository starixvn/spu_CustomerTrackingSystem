namespace Fanex.NAP.CTS.Service.Implement.CustomerClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.CTS.Service.Interface.CustomerClassification;
    using System;
    using System.Collections.Generic;

    public class PAPotentialClassificationService : IPAPotentialClassificationService
    {
        private readonly IDynamicRepository repository;

        public PAPotentialClassificationService()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<RescanPADangerousModel> GetDangerousScorePA(string custIDs)
        {
            var criteria = new RescanDangerousScorePACriteria
            {
                CustIDs = custIDs
            };

            return repository.Fetch<RescanPADangerousModel>(criteria);
        }

        public IEnumerable<TWInitialSmartPAModel> GetInitialSmartPA(string custIDs)
        {
            var criteria = new CheckExistTWInitialSmartPACriteria
            {
                CustIDs = custIDs
            };

            return repository.Fetch<TWInitialSmartPAModel>(criteria);
        }

        public IEnumerable<GetInitialTWGroupBettingModel> GetInitialTWGroupBetting(string custIDs)
        {
            var criteria = new RescanInitialTWGroupBettingCriteria() { CustIDs = custIDs };

            return repository.Fetch<GetInitialTWGroupBettingModel>(criteria);
        }

    }
}
