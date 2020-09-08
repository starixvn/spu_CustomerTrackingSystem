namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using Fanex.NAP.CTS.Criteria.SpecialCustomerClass;
    using Fanex.NAP.CTS.Model.SpecialCustomerClass;
    using System;
    using System.Collections.Generic;

    public class SpecialCustomerClassService : ISpecialCustomerClassService
    {
        public IEnumerable<CustomerClassModel> GetAllCustomerClass()
        {
            var criteria = new GetAllSpecialCustomerClassCriteria()
            {
                IsGetAllInfo = true
            };

            return DataService.GetData<CustomerClassModel>(criteria);
        }

        public ResultInsertCustomerClass InsertCustomerClass(string username, int custClass, bool isAffectToAssociation, int createdBy)
        {
            var insertCTSResult = new ResultInsertCustomerClass();
            var criteria = new InsertSpecialCustomerClassCriteria()
            {
                Username = username,
                CustomerClass = custClass,
                CreatedBy = createdBy,
                IsAffectToAssociation = isAffectToAssociation
            };

            var repository = new DynamicRepository();
            insertCTSResult.CustomerClassList = repository.Fetch<CustomerClassForMainDb>(criteria);
            insertCTSResult.OpErrorMessage = criteria.ErrorMessage;

            return insertCTSResult;
        }

        public DBResult RemoveCustomerClass(long ctsCustID, int createdBy)
        {
            var criteria = new RemoveSpecialCustomerClassCriteria()
            {
                CTSCustID = ctsCustID,
                CreatedBy = createdBy
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                try
                {
                    db.ExecuteNonQuery(criteria);

                    if (!string.IsNullOrEmpty(criteria.ErrorMessage))
                    {
                        return new DBResult
                        {
                            ErrorCode = -1,
                            ErrorMsg = criteria.ErrorMessage
                        };
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error("CTS - Remove Special Customer Class - RemoveCustomerClass exception: " + ex.Message, ex);

                    return new DBResult
                    {
                        ErrorCode = -1,
                        ErrorMsg = ex.Message
                    };
                }
            }

            return new DBResult
            {
                ErrorCode = 0,
                ErrorMsg = string.Empty
            };
        }

        public DBResult SyncCustomerClassToMainDb(IEnumerable<CustomerClassForMainDb> customerClassList, string syncAction)
        {
            var criteria = new SyncCustomerClassToMainDbCriteria()
            {
                custList = customerClassList.ToXmlString(),
                ErrorCode = 0,
                ErrorMessage = string.Empty
            };

            try
            {
                var repository = new DynamicRepository();

                repository.Execute(criteria);

                if (criteria.ErrorCode != 0)
                {
                    var errorMessage = "CTS - " + syncAction + " Special Customer Class - SyncCustomerClassToMainDb fail:";
                    errorMessage += " errorCode = " + criteria.ErrorCode;
                    errorMessage += "; customerClassList = " + criteria.custList;

                    LogService.Error(errorMessage);

                    return new DBResult
                    {
                        ErrorCode = criteria.ErrorCode,
                        ErrorMsg = criteria.ErrorMessage
                    };
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "CTS - " + syncAction + " Special Customer Class - SyncCustomerClassToMainDb exception:";
                errorMessage += " customerClassList = " + criteria.custList;

                LogService.Error(errorMessage, ex);

                return new DBResult
                {
                    ErrorCode = -1,
                    ErrorMsg = ex.Message
                };
            }

            return new DBResult
            {
                ErrorCode = 0,
                ErrorMsg = string.Empty
            };
        }
    }
}