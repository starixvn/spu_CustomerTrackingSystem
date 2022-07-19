namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountManagement;    
    using System.Collections.Generic;

    public interface IProblemAccountManagementService
    {
        IEnumerable<SubscriberGroupModel> GetAllSites();

        IEnumerable<ProblemAccountModel> GetProblemAccounts(ProblemAccountFilterViewModel filter);
        
        IEnumerable<ProblemAccountCategoryModel> GetProblemAccountCategories();

        IEnumerable<ProblemAccountValidatedModel> ValidateProblemAccounts(ValidateProblemAccountInputViewModel model);

        IEnumerable<GetCustEvidenceByReasonIdModel> GetCustEvidenceByReasonIds();             

		byte[] ExportProblemAccount(IEnumerable<ProblemAccountViewModel> problemAccounts, string fileName, string templatePath, ProblemAccountFilterViewModel filter);        

        IEnumerable<long> RemoveProblemAccountEvidence(string custIds);

    }
}

