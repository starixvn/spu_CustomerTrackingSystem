namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.SpecialCustomerClass;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using Fanex.NAP.CTS.Model.SpecialCustomerClass;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Implement.CustomerClassification;
    using Fanex.NAP.CTS.Service.Interface.CustomerClassification;
    using Fanex.NAP.CTS.ViewModel.SpecialCustomerClass;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class SpecialCustomerClassService : ISpecialCustomerClassService
    {
        #region construction
        private readonly ICustomerClassificationService _customerClassificationService;
        private readonly ICustomerClassificationBySportService _customerClassificationBySportService;
        private readonly IRescanCustomerClassificationService _rescanGeneralService;
        private readonly IRescanCustomerClassificationBySportService _rescanBySportService;
        private readonly IDynamicRepository _repository;

        public SpecialCustomerClassService()
        {
            _customerClassificationService = new CustomerClassificationService();
            _customerClassificationBySportService = new CustomerClassificationBySportService();
            _rescanGeneralService = new RescanCustomerClassificationService();
            _rescanBySportService = new RescanCustomerClassificationBySportService();
            _repository = new DynamicRepository();
        }
        #endregion

        #region public
        public ValidateSpecialCCResultViewModel ValidateSpecialCC(FunctionValidateSpecialCC functionID, List<UserListSpecialCCParamsModel> userList)
        {
            var users = Enumerable.Empty<ValidateSpecialCCItemViewModel>();

            if (userList != null && userList.Any())
            {
                var distinctUserList = userList.Distinct(x => $"{x.SportID}_{x.Username}");

                var criteria = new ValidateSpecialCCCriteria
                {
                    ActionType = (int)functionID,
                    ListUser = JsonConvert.SerializeObject(distinctUserList)
                };

                users = _repository.Fetch<ValidateSpecialCCItemViewModel>(criteria);
            }

            return new ValidateSpecialCCResultViewModel
            {
                Users = users ?? new List<ValidateSpecialCCItemViewModel>()
            };
        }

        public IEnumerable<CustomerClassViewModel> GetSpecialCustomerClass(SearchSpecialCCParamsModel model)
        {
            var specialCustomers = GetSpecialCustomerClass(model.Username); // continue...

            var result = specialCustomers?.Select(x => new CustomerClassViewModel
            {
                CTSCustID = x.CTSCustID,
                CustID = x.CustID,
                SportID = x.SportID,
                Username = x.Username,
                SubscriberName = x.SubscriberName,
                Comment = x.Comment,
                CustomerClass = x.CustomerClass,
                CreatedBy = x.CreatedBy,
                CreatedDateString = x.CreatedDate != DateTime.MinValue
                    ? x.CreatedDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss)
                    : string.Empty
            });

            return result;
        }

        public void InsertCustomerClass(AddSpecialCCParamsModel model, ValidateSpecialCCResultViewModel validateUsers, int createdBy)
        {
            var userList = validateUsers.Users.Select(x => new AddUserListSpecialCCModel
            {
                CTSCustID = x.CTSCustID,
                CustID = x.CustID,
                IsLicenseeVIP = x.IsLicenseeVIP,
                SportID = x.SportID,
                SubscriberID = x.SubscriberID
            });

            var criteria = new InsertSpecialCustomerClassCriteria
            {
                ListCust = JsonConvert.SerializeObject(userList),
                CustomerClass = model.CustomerClass,
                Remark = model.Comment,
                CreatedBy = createdBy
            };

            var insertCTSResult = _repository.Fetch<ResultSpecialCCViewModel>(criteria);

            // PushCC
            Parallel.Invoke(() =>
            {
                try
                {
                    PushDataToExternal(insertCTSResult.Where(x => x.SportID == 0));
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                          .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                          .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error(logMessage);

                    throw;
                }
            }
            ,
            () =>
            {
                try
                {
                    PushCCBySportToMainDB(insertCTSResult.Where(x => x.SportID != 0));
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                          .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                          .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error(logMessage);

                    throw;
                }
            });
        }

        public void RemoveCustomerClass(RemoveSpecialCCParamsModel model, ValidateSpecialCCResultViewModel validateUsers, int createdBy)
        {
            var userList = validateUsers.Users.Select(x => new RemoveUserListSpecialCCModel
            {
                CTSCustID = x.CTSCustID,
                CustID = x.CustID,
                SportID = x.SportID,
            });

            var criteria = new RemoveSpecialCustomerClassCriteria()
            {
                ListCust = JsonConvert.SerializeObject(userList),
                Comment = model.Comment,
                CreatedBy = createdBy
            };

            _repository.Execute(criteria);

            // Rescan
            Parallel.Invoke(() =>
            {
                try
                {
                    _rescanGeneralService.RescanCustomerClassification(new RescanCustClassificationModel
                    {
                        CustIds = validateUsers.Users.Where(x => x.SportID == 0).Select(x => x.CustID),
                        IsRescanPA = true,
                        IsPushCCToSoon88 = true,
                        IsUpdateDangerToMainDB = true,
                        IsToQueue = false,
                        FunctionId = FunctionId.RemoveSpecialCCWeb,
                        ServiceName = "RemoveSpecialCustomer"
                    });
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                          .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                          .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error(logMessage);

                    throw;
                }
            }
            ,
            () =>
            {
                try
                {
                    _rescanBySportService.ResanNormalBySportForNewCustomers(validateUsers.Users.Where(x => x.SportID != 0).Select(x => x.CustID), BySport_FunctionId.Manual);
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                          .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                          .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error(logMessage);

                    throw;
                }
            });
        }
        #endregion

        #region private
        private void PushDataToExternal(IEnumerable<ResultSpecialCCViewModel> resultSpecialCC)
        {
            if (resultSpecialCC != null && resultSpecialCC.Any())
            {
                var custIdsChunk = resultSpecialCC.Chunk(ConfigSettings.PushCCToExternalChunkSize);

                foreach (var custs in custIdsChunk)
                {
                    _customerClassificationService.PushDataToExternal(custs.Select(c => c.CustID).Distinct(), new PushDataToExternalModel
                    {
                        IsUpdateDangerToMainDB = false,
                        IsSyncAFC = false,
                        IsPushCCToSoon88 = false,
                        ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize
                    });
                }
            }
        }

        private void PushCCBySportToMainDB(IEnumerable<ResultSpecialCCViewModel> custInfoParams)
        {
            if (custInfoParams != null || custInfoParams.Any())
            {
                var custInfos = custInfoParams.Select(x => new InsertNormalAccountCustInfoModel
                {
                    CustID = x.CustID,
                    SportGroup = x.SportID
                });

                _customerClassificationBySportService.PushCCBySportToMainDB(custInfos, ConfigSettings.PushCCToExternalChunkSize);
            }
        }

        private IEnumerable<CustomerClassModel> GetSpecialCustomerClass(string username)
        {
            var criteria = new GetAllSpecialCustomerClassCriteria()
            {
                IsGetAllInfo = true,
                Username = username ?? string.Empty
            };

            return _repository.Fetch<CustomerClassModel>(criteria);
        }
        #endregion
    }
}