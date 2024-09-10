namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountManagement;    
    using System.Collections.Generic;

    public interface IProblemAccountManagementService
    {    
        IEnumerable<CategoryInfoModel> GetAllCategory();

        IEnumerable<GetProblemAccountModel> GetProblemAccounts(ProblemAccountFilterViewModel filter);
        
        IEnumerable<ProblemAccountCategoryModel> GetProblemAccountCategories(ViewCategoryPageSource categoryPageSource);

        IEnumerable<ProblemAccountValidatedModel> ValidateProblemAccounts(ValidateProblemAccountInputViewModel model);

		byte[] ExportProblemAccount(IEnumerable<GetProblemAccountViewModel> problemAccounts, string fileName, string templatePath, ProblemAccountFilterViewModel filter);        
    }
}

