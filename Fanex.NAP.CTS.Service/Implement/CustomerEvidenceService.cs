namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Core.Service;
    using Fanex.NAP.CTS.Criteria.ImportEvidences;
    using Fanex.NAP.CTS.ViewModel.Account.Evidence;
    using System.Collections.Generic;

    public class CustomerEvidenceService : ICustomerEvidenceService
    {
        private readonly ICriteriaService repository;

        public CustomerEvidenceService()
        {
            repository = new CriteriaService();
        }

        public IEnumerable<CTSCustomerEvidenceViewModel> CheckCleanCustFlaggedEvidence(string ctsCustIds)
        {
            var criteria = new CheckCleanCustFlaggedEvidenceCriteria
            {
                CTSCustIds = ctsCustIds
            };

            return repository.Fetch<CTSCustomerEvidenceViewModel>(criteria);
        }
    }
}