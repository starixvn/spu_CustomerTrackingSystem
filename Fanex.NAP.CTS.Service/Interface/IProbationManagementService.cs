namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.ProbationManagement;
    using System.Collections.Generic;

    public interface IProbationManagementService
    {
        IEnumerable<ProbationCategoryModel> GetProbationCategory(string username, int dayLeft, decimal minMargin, decimal maxMargin);

        DBResult ChangeCategory(long ctsCustId, string newCategoryName);

    }
}
