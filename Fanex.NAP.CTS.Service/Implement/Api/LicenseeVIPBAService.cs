namespace Fanex.NAP.CTS.Service.Implement.Api
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.Api;
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class LicenseeVIPBAService : ILicenseeVIPBAService
    {
        private readonly IDynamicRepository repository;
        private readonly ICustomerClassificationService custClassificationService;
        private readonly IRescanCustomerClassificationService rescanCustomerClassificationService;

        public LicenseeVIPBAService()
        {
            repository = new DynamicRepository();
            custClassificationService = new CustomerClassificationService();
            rescanCustomerClassificationService = new RescanCustomerClassificationService();
        }

        public bool CheckValid(LicenseeVIPBAInputModel parameter)
        {
            if (parameter == null || parameter.CreatedBy == 0 || string.IsNullOrWhiteSpace(parameter.CustIDs))
            {
                return false;
            }

            return true;
        }

        public bool CheckValid(RemoveLicenseeBABySiteInputModel parameter)
        {
            if (parameter == null || parameter.CreatedBy == 0 || string.IsNullOrWhiteSpace(parameter.SiteName))
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
                if (batchCustIds != null && batchCustIds.Any())
                {
                    var result = InsertLicVIP(batchCustIds);

                    if (result != null && result.Any())
                    {
                        custClassificationService.PushDataToExternal(result, new PushDataToExternalModel
                        {
                            IsUpdateDangerToMainDB = false,
                            IsSyncAFC = false,
                            IsPushCCToSoon88 = false,
                            ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize
                        });
                    }
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
                if (batchCustIds != null && batchCustIds.Any())
                {
                    var result = RemoveLicVIP(batchCustIds);

                    if (result != null && result.Any())
                    {
                        rescanCustomerClassificationService.RescanCustomerClassification(new RescanCustClassificationModel
                        {
                            CustIds = result,
                            IsRescanPA = true,
                            IsPushCCToSoon88 = true,
                            IsUpdateDangerToMainDB = true,
                            FunctionId = FunctionId.RemoveLicVIP_API,
                            ServiceName = "RemoveLicVIP"
                        });
                    }
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
                    var listCust =  custIds.Select(c => new LicenseeVIPBAModel
                    {
                        CustID = c,
                        CreatedBy = parameter.CreatedBy
                    });

                    return listCust.Distinct().Chunk(ConfigSettings.InternalChunkSize);
                }
            }

            return Enumerable.Empty<List<LicenseeVIPBAModel>>();
        }

        private IEnumerable<long> InsertLicVIP(IEnumerable<LicenseeVIPBAModel> parameter)
        {
            var criteria = new LicenseeVIPInsertCriteria
            {
                CustJson = JsonConvert.SerializeObject(parameter)
            };

            return repository.Fetch<long>(criteria);
        }


        private IEnumerable<long> RemoveLicVIP(IEnumerable<LicenseeVIPBAModel> parameter)
        {
            var criteria = new LicenseeVIPInsertCriteria
            {
                CustJson = JsonConvert.SerializeObject(parameter)
            };

            return repository.Fetch<long>(criteria);
        }

        #endregion Private Methods
    }
}