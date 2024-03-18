namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.SpecialCustomerClass;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.SpecialCustomerClass;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.ViewModel.SpecialCustomerClass;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SpecialCustomerClassService : ISpecialCustomerClassService
    {
        private readonly ICustomerClassificationService _custClassificationService;
        private readonly IRescanCustomerClassificationService _customerClassifyService;
        private readonly IDynamicRepository _repository;

        public SpecialCustomerClassService()
        {
            _custClassificationService = new CustomerClassificationService();
            _customerClassifyService = new RescanCustomerClassificationService();
            _repository = new DynamicRepository();
        }

        public IEnumerable<CustomerClassModel> GetAllCustomerClass(string username)
        {
            var criteria = new GetAllSpecialCustomerClassCriteria()
            {
                IsGetAllInfo = true,
                Username = username
            };

            return DataService.GetData<CustomerClassModel>(criteria);
        }

        public ResultInsertCustomerClass InsertCustomerClass(string username, int custClass, bool isAffectToAssociation, int createdBy)
        {
            var insertCTSResult = new ResultInsertCustomerClass();
            var criteria = new InsertSpecialCustomerClassCriteria
            {
                Username = username,
                CustomerClass = custClass,
                CreatedBy = createdBy,
                IsAffectToAssociation = isAffectToAssociation
            };

            var repository = new DynamicRepository();
            insertCTSResult.CustomerClassList = repository.Fetch<LatestClassificationModel>(criteria);
            insertCTSResult.OpErrorMessage = criteria.ErrorMessage;

            return insertCTSResult;
        }

        public ValidateSpecialCCResultViewModel ValidateSpecialCC(FunctionValidateSpecialCC functionID, List<UserListSpecialCCParamsModel> userList)
        {
            var criteria = new ValidateSpecialCCCriteria
            {
                FunctionValidateSpecialCC = (int)functionID,
                ListUser = JsonConvert.SerializeObject(userList)
            };

            return new ValidateSpecialCCResultViewModel
            {
                Users = _repository.Fetch<ValidateSpecialCCItemViewModel>(criteria)
            };
        }

        private ResultInsertRemoveViewModel PushDataToExternal(ResultInsertCustomerClass resultInsertToCTS)
        {
            var resultInsert = new ResultInsertRemoveViewModel
            {
                ErrorCode = 0,
                ErrorMessage = string.Empty
            };

            if (string.IsNullOrEmpty(resultInsertToCTS.OpErrorMessage))
            {
                if (resultInsertToCTS.CustomerClassList != null && resultInsertToCTS.CustomerClassList.Any())
                {
                    var custIdsChunk = resultInsertToCTS.CustomerClassList.Chunk(ConfigSettings.PushCCToExternalChunkSize);

                    foreach (var custs in custIdsChunk)
                    {
                        _custClassificationService.PushDataToExternal(custs.Select(c => c.CustId).Distinct(), new PushDataToExternalModel
                        {
                            IsUpdateDangerToMainDB = false,
                            IsSyncAFC = false,
                            IsPushCCToSoon88 = false,
                            ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize
                        });
                    }
                }
            }
            else
            {
                resultInsert.ErrorCode = -1;
                resultInsert.ErrorMessage = resultInsertToCTS.OpErrorMessage;
            }

            return resultInsert;
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

    }
}