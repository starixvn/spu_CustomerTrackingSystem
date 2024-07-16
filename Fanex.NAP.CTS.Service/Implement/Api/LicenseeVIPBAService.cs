namespace Fanex.NAP.CTS.Service.Implement.Api
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class LicenseeVIPBAService : BaseCustomerClassificationService, ILicenseeVIPBAService
    {        
        private readonly ICustomerClassificationService custClassificationService;
        private readonly IRescanCustomerClassificationService rescanCustomerClassificationService;

        public LicenseeVIPBAService()
        {            
            custClassificationService = new CustomerClassificationService();
            rescanCustomerClassificationService = new RescanCustomerClassificationService();
        }

        public bool CheckValid(LicenseeVIPBAInputModel parameter)
        {
            if (parameter == null || string.IsNullOrWhiteSpace(parameter.CustIDs))
            {
                return false;
            }

            return true;
        }

        public void InsertLicVIP(LicenseeVIPBAInputModel parameter)
        {
            if (parameter == null)
            {
                return;
            }

            var custInfo = MappingLicVIPBAModel(parameter);

            foreach (var batchCustIds in custInfo)
            {
                var custList = InsertLicVIP(batchCustIds);

                if (custList != null && custList.CustInfos.IsNotNullOrEmpty())
                {
                    custClassificationService.PushDataToExternal(custList.CustInfos.Select(x => x.CustID), new PushDataToExternalModel
                    {
                        IsUpdateDangerToMainDB = false,
                        IsSyncAFC = false,                    
                        ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize
                    });
                }
            }
        }

        public void RemoveLicVIP(LicenseeVIPBAInputModel parameter)
        {
            if (parameter == null)
            {
                return;
            }

            var custInfo = MappingLicVIPBAModel(parameter);

            foreach (var batchCustIds in custInfo)
            {
                var resultRemove = RemoveLicVIP(batchCustIds);

                if (resultRemove != null && resultRemove.CustInfos.IsNotNullOrEmpty())
                {
                    rescanCustomerClassificationService.RescanCustomerClassification(new RescanCustClassificationModel
                    {
                        CustIds = resultRemove.CustInfos.Select(x => x.CustID),
                        IsUpdateDangerToMainDB = true,
                        FunctionId = FunctionId.RemoveLicVIP_API,
                        ServiceName = "RemoveLicVIP"
                    });
                }
            }
        }

        public void InsertLicBA(LicenseeVIPBAInputModel parameter)
        {
            if (parameter == null)
            {
                return;
            }

            var custInfo = MappingLicVIPBAModel(parameter);

            foreach (var batchCustIds in custInfo)
            {
                var custList = InsertLicBA(batchCustIds);

                if (custList != null && custList.CustInfos.IsNotNullOrEmpty())
                {
                    custClassificationService.PushDataToExternal(custList.CustInfos.Select(x => x.CustID), new PushDataToExternalModel
                    {
                        IsUpdateDangerToMainDB = false,
                        IsSyncAFC = false,                        
                        ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize
                    });
                }
            }
        }

        public void RemoveLicBA(LicenseeVIPBAInputModel parameter)
        {
            if (parameter == null)
            {
                return;
            }

            var custInfo = MappingLicVIPBAModel(parameter);

            foreach (var batchCustIds in custInfo)
            {
                var resultRemove = RemoveLicBA(batchCustIds);

                if (resultRemove.CustInfos != null && resultRemove.CustInfos.Any())
                {
                    rescanCustomerClassificationService.RescanCustomerClassification(new RescanCustClassificationModel
                    {
                        CustIds = resultRemove.CustInfos.Select(x=>x.CustID),
                        IsUpdateDangerToMainDB = false,
                        FunctionId = FunctionId.RemoveLicBA_API,
                        ServiceName = "RemoveLicBA"
                    });
                }
            }
        }

        #region Private Methods

        private IEnumerable<IEnumerable<LicenseeVIPBAModel>> MappingLicVIPBAModel(LicenseeVIPBAInputModel parameter)
        {
            if (parameter != null)
            {
                var custIds = parameter.CustIDs.Split(',')?.Select(c => Convert.ToInt64(c))?.Distinct();

                if (custIds != null && custIds.Any())
                {
                    var listCust = custIds.Select(c => new LicenseeVIPBAModel
                    {
                        CustID = c,
                        CreatedBy = parameter.CreatedBy ?? Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"]) //Lic UserID is not in CTS system, so default StarixIT
                    });

                    return listCust.Distinct().Chunk(ConfigSettings.InternalChunkSize);
                }
            }

            return Enumerable.Empty<List<LicenseeVIPBAModel>>();
        }

        private ResultInsertCustomerClassificationModel InsertLicVIP(IEnumerable<LicenseeVIPBAModel> parameter)
        {
            var model = new InsertCustomerClassificationModel
            {
                CustClassificationJson = JsonConvert.SerializeObject(parameter)
            };

            return InsertCustomerClassification(CCInputFlow.LicVIP, model);
        }

        private ResultRemoveCustomerClassificationModel RemoveLicVIP(IEnumerable<LicenseeVIPBAModel> model)
        {           
            var modelRemove = new RemoveCustomerClassificationModel
            {
                CustClassificationJson = JsonConvert.SerializeObject(model)
            };

            return RemoveCustomerClassification(CCInputFlow.RemoveLicVIP, modelRemove);
        }

        private ResultInsertCustomerClassificationModel InsertLicBA(IEnumerable<LicenseeVIPBAModel> model)
        {
            var modelInsert = new InsertCustomerClassificationModel
            {
                CustClassificationJson = JsonConvert.SerializeObject(model)
            };

            return InsertCustomerClassification(CCInputFlow.LicBA, modelInsert);
        }

        private ResultRemoveCustomerClassificationModel RemoveLicBA(IEnumerable<LicenseeVIPBAModel> model)
        {
            var modelRemove = new RemoveCustomerClassificationModel 
            {
                CustClassificationJson = JsonConvert.SerializeObject(model)
            };

            return RemoveCustomerClassification(CCInputFlow.RemoveLicBA, modelRemove);
        }

        #endregion Private Methods
    }
}