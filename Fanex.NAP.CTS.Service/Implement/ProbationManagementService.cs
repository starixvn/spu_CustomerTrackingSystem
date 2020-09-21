namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Criteria.ProbationManagement;
    using Fanex.NAP.CTS.Model.ProbationManagement;
    using Fanex.NAP.CTS.Service.Interface;
    using System;
    using System.Collections.Generic;

    public class ProbationManagementService : IProbationManagementService
    {
        public IEnumerable<CategoryModel> GetCategories(int categoryMode)
        {
            var criteria = new GetCategoryCriteria
            {
                CategoryMode = categoryMode
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CategoryModel>(criteria);
            }
        }

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

        public DBResult ChangeCategory(string jsonData, string remark, int createdBy)
        {
            var criteria = new UpdateProbationCategory
            {
                CustIdList = jsonData,
                Remark = remark,
                CreatedBy = createdBy
            };

            try
            {
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    db.ExecuteNonQuery(criteria);

                    if (string.IsNullOrWhiteSpace(criteria.ErrorMessage))
                    {
                        return new DBResult
                        {
                            ErrorCode = 0,
                            ErrorMsg = criteria.ErrorMessage
                        };
                    }
                }

                return new DBResult
                {
                    ErrorCode = -1,
                    ErrorMsg = criteria.ErrorMessage
                };
            }
            catch (Exception ex)
            {
                var errorMessage = "CTS - Probation Management - ChangeCategory exception: " + ex.Message;

                LogService.Error(errorMessage, ex);

                return new DBResult
                {
                    ErrorCode = -1,
                    ErrorMsg = ex.Message
                };
            }            
        }
    }
}
