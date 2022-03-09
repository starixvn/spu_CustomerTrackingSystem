namespace Fanex.NAP.CTS.Service
{    
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountManagement;
    using System;
    using System.Collections.Generic;

    public interface IProblemAccountManagementService
    {
        IEnumerable<SubscriberGroupModel> GetAllSites();

        IEnumerable<ProblemAccountModel> GetProblemAccounts(ProblemAccountFilterViewModel filter);

        IEnumerable<ProblemAccountCategoryModel> GetProblemAccountCategories();

        IEnumerable<ProblemAccountValidatedModel> ValidateProblemAccounts();

        IEnumerable<GetCustEvidenceByReasonIdModel> GetCustEvidenceByReasonIds();
    }
}
