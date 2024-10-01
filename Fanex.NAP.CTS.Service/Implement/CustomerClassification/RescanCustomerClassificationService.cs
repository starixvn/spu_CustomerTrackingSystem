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
        public bool RescanCustomerClassification(RescanCustClassificationModel paras)
        {
            var result = new RescanResultModel();
            //var custIdsToRescanPA = new List<long>();
            //var smaCustIds = GetAgentCusts(paras.CustIds);
            var custRescans = GetRescanCustomerClassification(string.Join(",", paras.CustIds));

            if (custRescans.CustInfos != null && custRescans.CustInfos.Any())
            {
                var groupingCustRescans = custRescans.CustInfos.GroupBy(x => x.InputFlowID).Select(g => new { InputFlowID = g.Key, CustRescans = g.Select(x => x) });

                foreach (var group in groupingCustRescans)
                {
                    if ((CCInputFlow)group.InputFlowID == CCInputFlow.RescanNormal)
                    {
                        PreNormalClassification(group.CustRescans.Select(x => x.CustID), paras);
                    }
                    else
                    {
                        result = RescanProblemAccount(group.CustRescans, paras, (CCInputFlow)group.InputFlowID);

                        //custIdsToRescanPA.AddRange(result.CustIds);
                    }
                }                
            }

            //if (smaCustIds.Any())
            //{
            //    result.IsSuccess = PushDataToExternal(paras, custIdsToRescanPA, smaCustIds);
            //}

            return result.IsSuccess;
        }

        #region Private Methods

        private RescanResultModel RescanProblemAccount(IEnumerable<RescanCustomerClassificationModel> custNormals, RescanCustClassificationModel paras, CCInputFlow ccInputFlow)
        {
            var resultCustRescans = base.RescanCustomerClassification(custNormals, ccInputFlow, paras.IsToQueue);

            PushDataToExternalPackge(resultCustRescans, paras);

            return new RescanResultModel()
            {
                CustIds = resultCustRescans.ToList(),
                IsSuccess = true
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
            bool isSuccess = true;

            if (custIdsToSyncCC.Any())
            {
                isSuccess = custClassificationService.PushDataToExternalPackge(custIdsToSyncCC.Distinct(), new PushDataToExternalModel
                {
                    ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
                    IsUpdateDangerToMainDB = parameters.IsUpdateDangerToMainDB,
                    IsSyncAFC = ConfigSettings.IsSyncAFCData
                }, FromFlow.CTSWEB);
            }

            return isSuccess;
        }
        #endregion Private Methods
    }
}