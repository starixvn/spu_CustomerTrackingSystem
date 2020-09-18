namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.ProbationManagement;
    using System.Collections.Generic;

    public interface IProbationManagementService
    {
        IEnumerable<ProbationCategoryModel> GetProbationCategory(string username, int dayLeft, int categoryMode);

        DBResult ChangeCategory(long ctsCustId, string newCategoryName);

    }
}
