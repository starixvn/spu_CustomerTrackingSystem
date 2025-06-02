namespace Fanex.NAP.CTS.Service.Implement.CustomerClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using Fanex.NAP.CTS.Criteria.CustomerClassification.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassification.PushDataToExternal;
    using Fanex.NAP.CTS.Service.Interface.CustomerClassification;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NAP.CTS.Core.Service;

    public class CustomerClassificationBySportService : ICustomerClassificationBySportService
    {
        private readonly ICriteriaService _repository;

        public CustomerClassificationBySportService()
        {
            _repository = new CriteriaService();
        }

        public bool PushCCBySportToMainDB(IEnumerable<InsertNormalAccountCustInfoModel> custInfos, int batchExternalSize)
        {
            var custInfoChunks = custInfos.Chunk(batchExternalSize).ToList();

            foreach (var custs in custInfoChunks)
            {
                var ccBySports = GetCustomerClassificationBySports(custs);

                // Constants.InternalRetry 3
                var isSuccess = RetryPushCustClassificationBySportToMainDB(ccBySports, 3);

                if (!isSuccess)
                {
                    return false;
                }
            }

            return true;
        }

        public IEnumerable<GetCustomerClassificationBySportModel> GetCustomerClassificationBySports(IEnumerable<InsertNormalAccountCustInfoModel> custInfos)
        {
            if (custInfos != null && custInfos.Any())
            {
                return GetCustomerClassificationBySportsDataAccess(custInfos.Distinct());
            }

            return Enumerable.Empty<GetCustomerClassificationBySportModel>();
        }

        #region private
        private IEnumerable<GetCustomerClassificationBySportModel> GetCustomerClassificationBySportsDataAccess(IEnumerable<InsertNormalAccountCustInfoModel> custInfos)
        {
            var criteria = new GetCustomerClassificationBySportCriteria
            {
                CustInfos = JsonConvert.SerializeObject(custInfos)
            };

            return _repository.Fetch<GetCustomerClassificationBySportModel>(criteria);
        }

        private DBResult UpdateCustomerClassificationBySportToMainDB(IEnumerable<CustomerClassificationUpdateBySportToMainDBModel> custClassification)
        {
            var criteria = new CustomerClassificationUpdateBySportCriteria
            {
                custList = custClassification.ToXmlString()
            };

            _repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = criteria.ErrorCode,
                ErrorMsg = criteria.ErrorMessage
            };
        }

        private bool RetryPushCustClassificationBySportToMainDB(IEnumerable<GetCustomerClassificationBySportModel> classificationBySports, int retryTime)
        {
            if (classificationBySports != null && classificationBySports.Any())
            {
                var custClassification = classificationBySports.Select(x => new CustomerClassificationUpdateBySportToMainDBModel
                {
                    CustID = x.CustID,
                    SportID = x.SportID,
                    CustomerClass = x.CustomerClass
                });

                while (retryTime >= 0)
                {
                    try
                    {
                        var result = UpdateCustomerClassificationBySportToMainDB(custClassification);

                        if (result.ErrorCode != 0 && retryTime == 0)
                        {
                            LogExtension.LogError($"CTS - UpdateCustomerClassificationBySportToMainDB failed {result.ErrorMsg} with listCust: {JsonConvert.SerializeObject(custClassification)}");
                            return false;
                        }
                        else if (result.ErrorCode != 0)
                        {
                            retryTime--;
                        }
                        else
                        {
                            retryTime = -1;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (retryTime == 0)
                        {
                            var jsonDataForLog = JsonConvert.SerializeObject(custClassification);

                            LogExtension.LogError($"UpdateCustomerClassificationBySportToMainDB exception {ex.Message} with listCust: {jsonDataForLog}", ex);
                            return false;
                        }

                        retryTime--;
                    }
                }
            }

            return true;
        }
        #endregion
    }
}
