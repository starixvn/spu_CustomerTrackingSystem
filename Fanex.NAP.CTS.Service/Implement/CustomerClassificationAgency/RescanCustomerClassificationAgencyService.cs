namespace Fanex.NAP.CTS.Service.Implement.CustomerClassificationAgency
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Criteria;
    using Fanex.NAP.CTS.Criteria.CustomerClassificationAgency;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.BaseCustomerClassificationAgency;
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.RescanCustomerClassificationAgency;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Service.Interface.CustomerClassificationAgency;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class RescanCustomerClassificationAgencyService : ProblemAccountClassificationAgencyService, IRescanCustomerClassificationAgencyService
    {
        private readonly ICustomerClassificationService _custClassificationService;

        public RescanCustomerClassificationAgencyService() : base()
        {
            _custClassificationService = new CustomerClassificationService();
        }

        public bool RescanCustomerClassificationAgency(RescanCustClassificationAgencyModel paras)
        {
            var result = new RescanResultAgencyModel();
            var custIdsToRescan = new List<long>();
            var custRescans = GetRescanCustomerClassificationAgency(string.Join(",", paras.CustIds));

            if (custRescans.CustInfos != null && custRescans.CustInfos.Any())
            {
                var groupingCustRescans = custRescans.CustInfos.GroupBy(x => x.InputFlowID).Select(g => new { InputFlowID = g.Key, CustRescans = g.Select(x => x) });

                foreach (var group in groupingCustRescans)
                {
                    if ((AgencyCCInputFlow)group.InputFlowID == AgencyCCInputFlow.RescanNormal)
                    {
                        var custIds = group.CustRescans.Select(x => x.CustID);
                        PreNormalClassificationAgency(custIds, paras);

                        custIdsToRescan.AddRange(custIds);
                    }
                    else
                    {
                        result = RescanProblemAccountAgency(group.CustRescans, paras, (AgencyCCInputFlow)group.InputFlowID);

                        custIdsToRescan.AddRange(result.CustIds);
                    }
                }
            }

            var smaCustIds = GetAgentCusts(paras.CustIds);
            var custToRemovePA = smaCustIds.Except(custIdsToRescan);

            if (custToRemovePA.IsNotNullOrEmpty())
            {
                result.IsSuccess = PushDataToExternalPackge(custToRemovePA?.Distinct(), paras);
            }

            return true;
        }

        #region private method
        private IEnumerable<long> GetAgentCusts(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                var criteria = new GetRoleCustomerCriteria
                {
                    CustIds = string.Join(",", custIds.Distinct())
                };

                var data = _repository.Fetch<RoleCustomerModel>(criteria);

                if (data != null && data.Any())
                {
                    return data.Where(c => c.RoleID == (int)ROLE_CUSTOMER.AGENT
                                        || c.RoleID == (int)ROLE_CUSTOMER.MASTER
                                        || c.RoleID == (int)ROLE_CUSTOMER.SUPER).Select(c => c.CustID);
                }
            }

            return Enumerable.Empty<long>();
        }

        private void PreNormalClassificationAgency(IEnumerable<long> custNormals, RescanCustClassificationAgencyModel paras)
        {
            var custIDToString = string.Join(",", custNormals);

            var criteria = new NormalClassificationRescanAgencyCriteria
            {
                ListCustId = custIDToString,
                FunctionId = (int)paras.FunctionId,
                RescanTypeId = (int)RescanNormalTypeAgency.Manual
            };

            _repository.Execute(criteria);
        }

        private RescanResultAgencyModel RescanProblemAccountAgency(IEnumerable<RescanCustomerClassificationAgencyModel> custNormals, RescanCustClassificationAgencyModel paras, AgencyCCInputFlow ccInputFlow)
        {
            var resultCustRescans = base.RescanCustomerClassificationAgency(custNormals, ccInputFlow, paras.IsToQueue);

            PushDataToExternalPackge(resultCustRescans, paras);

            return new RescanResultAgencyModel()
            {
                CustIds = resultCustRescans.ToList(),
                IsSuccess = true
            };
        }

        private bool PushDataToExternalPackge(IEnumerable<long> custIdsToSyncCC, RescanCustClassificationAgencyModel parameters)
        {
            bool isSuccess = true;

            if (custIdsToSyncCC.Any())
            {
                isSuccess = _custClassificationService.PushDataToExternalPackge(custIdsToSyncCC.Distinct(), new PushDataToExternalModel
                {
                    ExternalBatchSize = ConfigSettings.PushCCToExternalChunkSize,
                    //IsUpdateDangerToMainDB = parameters.IsUpdateDangerToMainDB, -- Agency not handle
                    IsSyncAFC = ConfigSettings.IsSyncAFCData,
                    RoleGroup = RoleGroupType.Agency
                }, FromFlow.CTSWEB);
            }

            return isSuccess;
        }
        #endregion
    }
}
