namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Criteria.ProbationManagement;
    using Fanex.NAP.CTS.Model.ProbationManagement;
    using Fanex.NAP.CTS.Service.Interface;
    using System.Collections.Generic;

    public class ProbationManagementService : IProbationManagementService
    {
        public IEnumerable<ProbationCategoryModel> GetProbationCategory(string username, int daysLeft, int categoryMode)
        {
            var criteria = new GetProbationCategoryCriteria
            {
                UserName = username,
                DaysLeft = daysLeft,
                CategoryMode = categoryMode
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<ProbationCategoryModel>(criteria);
            }
        }

        public DBResult ChangeCategory(long ctsCustId, string newCategoryName)
        {
            throw new System.NotImplementedException();
        }
    }
}
