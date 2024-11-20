namespace Fanex.NAP.CTS.Service
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria.CustomerClassification;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Service.Implement;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class RescanCustomerClassificationService : ProblemAccountClassificationService, IRescanCustomerClassificationService
    {
        private readonly ICustomerClassificationService custClassificationService;       
        private readonly IDynamicRepository repository;

        public RescanCustomerClassificationService()
        {
            custClassificationService = new CustomerClassificationService();         
            repository = new DynamicRepository();
        }

        /// <summary>
        /// RescanCustomerClassification for Member
        /// </summary>
        public bool RescanCustomerClassification(RescanCustClassificationModel parameters)
        {
            var result = new RescanResultModel();
            var custRescans = GetRescanCustomerClassification(string.Join(",", parameters.CustIds));

            if (custRescans.CustInfos != null && custRescans.CustInfos.Any())
            {
                var groupingCustRescans = custRescans.CustInfos.GroupBy(x => x.InputFlowID).Select(g => new { InputFlowID = g.Key, CustRescans = g.Select(x => x) });

                foreach (var group in groupingCustRescans)
                {
                    if ((CCInputFlow)group.InputFlowID == CCInputFlow.RescanNormal)
                    {
                        PreNormalClassification(group.CustRescans.Select(x => x.CustID), parameters);
                    }
                    else
                    {
                        result = RescanProblemAccount(group.CustRescans, parameters, (CCInputFlow)group.InputFlowID);
                    }
                }                
            }
            return result.IsSuccess;
        }

        #region Private Methods

        private RescanResultModel RescanProblemAccount(IEnumerable<RescanCustomerClassificationModel> custNormals, RescanCustClassificationModel paras, CCInputFlow ccInputFlow)
        {
            var resultCustRescans = base.RescanCustomerClassification(custNormals, ccInputFlow, paras.IsToQueue);

            var isSuccess = PushDataToExternalPackge(resultCustRescans, paras);

            return new RescanResultModel()
            {
                CustIds = resultCustRescans.ToList(),
                IsSuccess = isSuccess
            };
        }

        private void PreNormalClassification(IEnumerable<long> custNormals, RescanCustClassificationModel paras)
        {
            var custIDToString = string.Join(",", custNormals);

            var criteria = new NormalClassificationRescanCriteria
            {
                ListCustId = custIDToString,
                FunctionId = (int)paras.FunctionId,
                RescanTypeId = (int)RescanNormalType.Manual
            };

            repository.Execute(criteria);           
        }

        private bool PushDataToExternalPackge(IEnumerable<long> custIdsToSyncCC, RescanCustClassificationModel parameters)
        {            
            if (custIdsToSyncCC.Any())
            {
                return custClassificationService.PushDataToExternalPackge(custIdsToSyncCC.Distinct(), new PushDataToExternalModel
                {
                    ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
                    IsUpdateDangerToMainDB = parameters.IsUpdateDangerToMainDB,
                    IsSyncAFC = ConfigSettings.IsSyncAFCData,
                    RoleGroup = RoleGroupType.Member
                }, FromFlow.CTSWEB);
            }

            return true;
        }
        #endregion Private Methods
    }
}