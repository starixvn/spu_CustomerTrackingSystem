namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.API.UpdateCategoryWhenPAUpdated;
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountManagement;    
    using System.Collections.Generic;

    public interface IProblemAccountManagementService
    {
        IEnumerable<SubscriberGroupModel> GetAllSites();

        IEnumerable<ProblemAccountModel> GetProblemAccounts(ProblemAccountFilterViewModel filter);

        IEnumerable<ProblemAccountCommentSelModel> GetProblemAccountCommentSel(string custIdList);

        IEnumerable<ProblemAccountCategoryModel> GetProblemAccountCategories();

        IEnumerable<ProblemAccountValidatedModel> ValidateProblemAccounts(ValidateProblemAccountInputViewModel model);

        IEnumerable<GetCustEvidenceByReasonIdModel> GetCustEvidenceByReasonIds();        

        IEnumerable<ResultProblemAccountAddCustomerModel> MarkProblemAccountSiteCredit(IEnumerable<ProblemAccountValidatedModel> models, int userID);

        IEnumerable<ResultProblemAccountAddCustomerModel> MarkProblemAccountSiteLicensee(IEnumerable<ProblemAccountValidatedModel> models, int userID);

        IEnumerable<ResultProblemAccountAddCustomerModel> UpdateProblemAccountSiteCredit(IEnumerable<ProblemAccountValidatedModel> models, int userID);

        IEnumerable<ResultProblemAccountAddCustomerModel> UpdateProblemAccountSiteLicensee(IEnumerable<ProblemAccountValidatedModel> models, int userID);

        IEnumerable<ResultProblemAccountAddCustomerModel> UnmarkProblemAccountSiteCredit(IEnumerable<ProblemAccountValidatedModel> models, bool isExceptDownlinesByTVS, bool isExceptDownlinesByOPS, int userID);

        IEnumerable<ResultProblemAccountAddCustomerModel> UnmarkProblemAccountSiteLicensee(IEnumerable<ProblemAccountValidatedModel> models, int userID);

        IEnumerable<long> UpdateProblemAccountEvidence(UpdateCategoryParamsModel paras);

        IEnumerable<long> RemoveProblemAccountEvidence(string custIds);

    }
}
