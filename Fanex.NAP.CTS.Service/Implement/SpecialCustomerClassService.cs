namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.SpecialCustomerClass;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using Fanex.NAP.CTS.Model.SpecialCustomerClass;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Implement.CustomerClassification;
    using Fanex.NAP.CTS.Service.Interface.CustomerClassification;
    using Fanex.NAP.CTS.ViewModel.SpecialCustomerClass;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class SpecialCustomerClassService : BaseCustomerClassificationService, ISpecialCustomerClassService
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
            var specialCustomers = GetSpecialCustomerClassDataAccess(model);

            var result = specialCustomers?.Select(x => new CustomerClassViewModel
            {
                CTSCustID = x.CTSCustID,
                CustID = x.CustID,
                SportID = x.SportID,
                Username = x.Username,
                SubscriberName = x.SubscriberName,
                Comment = x.Remark,
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
            var generalInsertResult = new ResultInsertCustomerClassificationModel();
            var sportInsertResult = new ResultInsertCustomerClassificationBySportModel();

            var generalCusts = validateUsers.Users.Where(x => x.SportID == 0).Select(x => new AddGenreralUserSpecialCCModel
            {
                CTSCustID = x.CTSCustID,
                CustID = x.CustID,
                IsLicenseeVIP = x.IsLicenseeVIP,                
                SubscriberID = x.SubscriberID,
                CustomerClass = model.CustomerClass,
                CreatedBy = createdBy,
                Remark = model.Comment
            });

            var sportCusts = validateUsers.Users.Where(x => x.SportID != 0).Select(x => new AddSportUserSpecialCCModel
            {
                CTSCustID = x.CTSCustID,
                CustID = x.CustID,
                SportID = x.SportID,
                IsLicenseeVIP = x.IsLicenseeVIP,
                SubscriberID = x.SubscriberID,
                CustomerClass = model.CustomerClass,
                CreatedBy = createdBy,
                Remark = model.Comment
            });                        

            if (generalCusts.Any())
            {
                var generalModelInsert = new InsertCustomerClassificationModel()
                {
                    FromAction = (int)FromFlow.CTSWEB,
                    CustClassificationJson = JsonConvert.SerializeObject(generalCusts)
                };

                generalInsertResult = base.InsertCustomerClassification(CCInputFlow.SpecialCC, generalModelInsert);
            }

            if (sportCusts.Any())
            {
                var sportmodelInsert = new InsertCustomerClassificationBySportModel()
                {
                    CustClassificationJson = JsonConvert.SerializeObject(sportCusts)
                };

                sportInsertResult = base.InsertCustomerClassificationBySport(CCInputFlow.BySportInsertSpecialCC, sportmodelInsert);
            }          

            // PushCC
            Parallel.Invoke(
                () =>
                {
                    try
                    {
                        if (generalInsertResult.CustInfos != null && generalInsertResult.CustInfos.Any())
                        {
                            PushDataToExternal(generalInsertResult.CustInfos.Select(x => x.CustID));
                        }                        
                    }
                    catch (Exception ex)
                    {
                        ex.LogError();

                        throw;
                    }
                }
                ,
                () =>
                {
                    try
                    {
                        if (sportInsertResult.CustInfos != null && sportInsertResult.CustInfos.Any())
                        {
                            PushCCBySportToMainDB(sportInsertResult.CustInfos);
                        }                        
                    }
                    catch (Exception ex)
                    {
                        ex.LogError();

                        throw;
                    }
                });
        }

        public void RemoveCustomerClass(RemoveSpecialCCParamsModel model, ValidateSpecialCCResultViewModel validateUsers, int removedBy)
        {            
            var custInfos = validateUsers.Users.Select(x => new RemoveSpecialCCModel
            {
                CTSCustID = x.CTSCustID,
                CustID = x.CustID,
                SubscriberID = x.SubscriberID,
                SportID = x.SportID,
                Remark = model.Comment,
                CreatedBy = removedBy
            });

            RemoveSpecialCCGeneral(custInfos.Where(x => x.SportID == 0).Select(x => x));

            RemoveSpecialCCBySport(custInfos.Where(x => x.SportID != 0).Select(x => x));
        }
        #endregion

        #region private

        private void RemoveSpecialCCGeneral(IEnumerable<RemoveSpecialCCModel> custInfos)
        {           
            if (custInfos.Any())
            {
                var removeModel = new RemoveCustomerClassificationModel
                {
                    FromAction = (int)FromFlow.CTSWEB,
                    CustClassificationJson = JsonConvert.SerializeObject(custInfos)
                };

                var resultRemove = RemoveCustomerClassification(CCInputFlow.RemoveSpecialCC, removeModel);

                if (resultRemove.CustInfos != null && resultRemove.CustInfos.Any())
                {
                    _rescanGeneralService.RescanCustomerClassification(new RescanCustClassificationModel
                    {
                        CustIds = resultRemove.CustInfos.Select(x=>x.CustID),
                        IsUpdateDangerToMainDB = true,
                        IsToQueue = false,
                        FunctionId = FunctionId.RemoveSpecialCCWeb,
                        ServiceName = "RemoveSpecialCustomer"
                    });
                }
            }
        }

        private void RemoveSpecialCCBySport(IEnumerable<RemoveSpecialCCModel> custInfos)
        {         
            if (custInfos.Any())
            {
                var removeModel = new RemoveCustomerClassificationBySportModel
                {                    
                    CustClassificationJson = JsonConvert.SerializeObject(custInfos)
                };

                var resultRemove = RemoveCustomerClassificationBySport(CCInputFlow.BySportRemoveSpecialCC, removeModel);

                if (resultRemove.CustInfos != null && resultRemove.CustInfos.Any())
                {
                    var rescanUserList = resultRemove.CustInfos.Select(x => new RescanNormalBySportListCustModel { CustID = x.CustID, SportID = x.SportID });

                    _rescanBySportService.ResanNormalBySportForNewCustomers(rescanUserList, BySport_FunctionId.Manual);
                }
            }
        }

        private void PushDataToExternal(IEnumerable<long> custIDList)
        {
            if (custIDList != null && custIDList.Any())
            {
                var custIdsChunk = custIDList.Chunk(ConfigSettings.PushCCToExternalChunkSize);

                foreach (var custs in custIdsChunk)
                {
                    _customerClassificationService.PushDataToExternal(custs, new PushDataToExternalModel
                    {
                        IsUpdateDangerToMainDB = false,
                        IsSyncAFC = false,                      
                        ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize
                    });
                }
            }
        }

        private void PushCCBySportToMainDB(IEnumerable<CustInfoClassificationBySportModel> custInfoParams)
        {
            if (custInfoParams != null && custInfoParams.Any())
            {
                var custInfos = custInfoParams.Select(x => new InsertNormalAccountCustInfoModel
                {
                    CustID = x.CustID,
                    SportGroup = x.SportGroup
                });

                _customerClassificationBySportService.PushCCBySportToMainDB(custInfos, ConfigSettings.PushCCToExternalChunkSize);
            }
        }

        private IEnumerable<CustomerClassModel> GetSpecialCustomerClassDataAccess(SearchSpecialCCParamsModel model)
        {
            var criteria = new GetAllSpecialCustomerClassCriteria()
            {
                IsGetAllInfo = true,
                Username = model.Username ?? string.Empty,
                SportList = model.SportIds
            };

            return _repository.Fetch<CustomerClassModel>(criteria);
        }
        #endregion
    }
}