namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountManagement;    
    using System.Collections.Generic;

    public interface IProblemAccountManagementService
    {
        IEnumerable<SubscriberGroupModel> GetAllSites();

        IEnumerable<CategoryInfoModel> GetAllCategory();

        IEnumerable<GetProblemAccountModel> GetProblemAccounts(ProblemAccountFilterViewModel filter);
        
        IEnumerable<ProblemAccountCategoryModel> GetProblemAccountCategories(bool isFromPAM);

        IEnumerable<ProblemAccountValidatedModel> ValidateProblemAccounts(ValidateProblemAccountInputViewModel model);

        IEnumerable<GetCustEvidenceByReasonIdModel> GetCustEvidenceByReasonIds();             

		byte[] ExportProblemAccount(IEnumerable<GetProblemAccountViewModel> problemAccounts, string fileName, string templatePath, ProblemAccountFilterViewModel filter);        

        IEnumerable<long> RemoveProblemAccountEvidence(string custIds);

    }
}

