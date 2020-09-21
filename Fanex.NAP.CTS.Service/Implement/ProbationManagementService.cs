namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using Fanex.NAP.CTS.Criteria.ProbationManagement;
    using Fanex.NAP.CTS.Criteria.SpecialCustomerClass;
    using Fanex.NAP.CTS.Model.ProbationManagement;
    using Fanex.NAP.CTS.Model.SpecialCustomerClass;
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

        public IEnumerable<AccumulatedInfoModel> GetAccummulatedInfo(IEnumerable<ProbationModel> probationModelList)
        {
            var criteria = new GetAccummulatedInfoCriteria
            {
               CustomersXML = probationModelList.ToXmlString()
            };

            try
            {
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    return db.Query<AccumulatedInfoModel>(criteria);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "CTS - Probation Management - GetAccummulatedInfo exception: " + ex.Message;

                LogService.Error(errorMessage, ex);

                return new List<AccumulatedInfoModel>();
            }
        }

        public ResultChangeCategoryToCts ChangeCategory(string jsonData, string remark, int createdBy)
        {
            var result = new ResultChangeCategoryToCts
            {
                CustomerClassList = new List<CustomerClassForMainDb>()
            };

            var criteria = new UpdateProbationCategory
            {
                CustIdList = jsonData,
                Remark = remark,
                CreatedBy = createdBy
            };

            try
            {
                var repository = new DynamicRepository();
                var customerClass = repository.Fetch<CustomerClassForMainDb>(criteria);

                if (string.IsNullOrWhiteSpace(criteria.ErrorMessage))
                {
                    result.DBResult = new DBResult { ErrorCode = 0, ErrorMsg = criteria.ErrorMessage };
                    result.CustomerClassList = customerClass;

                    return result;
                }
                result.DBResult = new DBResult { ErrorCode = -1, ErrorMsg = criteria.ErrorMessage };
                return result;
            }
            catch (Exception ex)
            {
                var errorMessage = "CTS - Probation Management - ChangeCategory exception: " + ex.Message;

                LogService.Error(errorMessage, ex);

                result.DBResult = new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
                return result;
            }
        }

        public DBResult SyncCustomerClassToMainDb(IEnumerable<CustomerClassForMainDb> customerClassList)
        {
            var criteria = new SyncCustomerClassToMainDbCriteria()
            {
                custList = customerClassList.ToXmlString()
            };

            try
            {
                var repository = new DynamicRepository();

                repository.Execute(criteria);

                if (criteria.ErrorCode != 0)
                {
                    var errorMessage = "CTS - Probation Management - SyncCustomerClassToMainDb fail:";
                    errorMessage += " errorCode = " + criteria.ErrorCode;
                    errorMessage += "; customerClassList = " + criteria.custList;

                    LogService.Error(errorMessage);

                    return new DBResult { ErrorCode = criteria.ErrorCode, ErrorMsg = criteria.ErrorMessage };
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "CTS - Probation Management - SyncCustomerClassToMainDb exception: ";
                errorMessage += " customerClassList = " + criteria.custList;

                LogService.Error(errorMessage, ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }
    }
}
