namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.ProbationManagement;
    using Fanex.NAP.CTS.Service.Interface;
    using System.Collections.Generic;

    public class ProbationManagementService : IProbationManagementService
    {
        public DBResult ChangeCategory(long ctsCustId, string newCategoryName)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ProbationCategoryModel> GetProbationCategory(string username, int dayLeft, decimal minMargin, decimal maxMargin)
        {
            return new List<ProbationCategoryModel>();
           
        }
    }
}
