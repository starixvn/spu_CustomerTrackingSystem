namespace Fanex.NAP.CTS.Service.Implement.CustomerClassificationBySport
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Service;
    using Fanex.NAP.CTS.Criteria.CustomerClassificationBySport.RescanNormalClassificationBySport;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassificationBySport.ProblemAccountInsertBySport;
    using Fanex.NAP.CTS.Model.CustomerClassificationBySport.RescanCustomerClassificationBySport;
    using Fanex.NAP.CTS.Service.Implement.CustomerClassificationBySport.ProblemAccountInsertBySport;
    using Fanex.NAP.CTS.Service.Interface.CustomerClassificationBySport;
    using Fanex.NAP.CTS.Service.Interface.CustomerClassificationBySport.ProblemAccountInsertBySport;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class RescanCustomerClassificationBySportService : IRescanCustomerClassificationBySportService
    {
        private readonly ICriteriaService _repository;
        private readonly ICustomerClassificationBySportService _customerClassificationBySport;
        private readonly IProblemAccountBySportService _problemAccountBySport;

        public RescanCustomerClassificationBySportService()
        {
            _repository = new CriteriaService();
            _customerClassificationBySport = new CustomerClassificationBySportService();
            _problemAccountBySport = new ProblemAccountBySportService();
        }

        public bool RescanCustomerClassificationBySport(RescanCustClassificationBySportModel rescanModel)
        {
            var isSuccess = true;

            var distinctCustInfos = rescanModel.CustInfos.Distinct(x => new { x.CustID, x.SportID }).ToList();

            var custRescans = distinctCustInfos
                .Chunk(rescanModel.InternalChunkSize)
                .SelectMany(chunk => GetRescanCustomerClassificationBySport(chunk));

            if (custRescans.IsNotNullOrEmpty())
            {
                var groupingCustRescans = custRescans.GroupBy(x => x.InputFlowID);

                foreach (var group in groupingCustRescans)
                {
                    if (!isSuccess) break;

                    if ((BySportCCInputFlow)group.Key == BySportCCInputFlow.RescanNormal)
                    {
                        ProcessNormalCustomers(group, rescanModel.FunctionId, rescanModel.InternalChunkSize);
                    }
                    else
                    {
                        isSuccess = ProcessProblemAccounts(group, (BySportCCInputFlow)group.Key, rescanModel);
                    }
                }
            }

            return isSuccess;
        }

        #region Private Methods
        private void ProcessNormalCustomers(IEnumerable<RescanCustomerClassificationBySportResultModel> group, BySport_FunctionId functionId, int internalBatchSize)
        {
            var normalCustomers = group.Select(x => new RescanNormalBySportListCustModel
            {
                CustID = x.CustID,
                SportID = x.SportID
            });

            foreach (var chunk in normalCustomers.Chunk(internalBatchSize))
            {
                ResanNormalBySportForNewCustomers(chunk, functionId);
            }
        }

        bool ProcessProblemAccounts(IEnumerable<RescanCustomerClassificationBySportResultModel> group, BySportCCInputFlow inputFlow, RescanCustClassificationBySportModel rescanModel)
        {
            foreach (var chunk in group.Chunk(rescanModel.InternalChunkSize))
            {
                var result = RescanProblemAccountBySport(chunk, inputFlow, rescanModel);
                if (!result.IsSuccess)
                {
                    return false;
                }
            }
            return true;
        }

        private RescanBySportResultModel RescanProblemAccountBySport(IEnumerable<RescanCustomerClassificationBySportResultModel> custInfos, BySportCCInputFlow ccInputFlow, RescanCustClassificationBySportModel rescanModel)
        {
            var mappingCustInfoInsert = custInfos.Select(x => new InsertProblemAccountBySportModel(x));

            var paInsertResult = _problemAccountBySport.InsertProblemAccountBySport(mappingCustInfoInsert, ccInputFlow, rescanModel.FunctionId);

            var isSuccess = _customerClassificationBySport.PushCCBySportToMainDB(paInsertResult, rescanModel.PushCCChunkSize, rescanModel.PushCCRetryTime);

            return new RescanBySportResultModel
            {
                CustInfos = paInsertResult,
                IsSuccess = isSuccess,
            };
        }

        private IEnumerable<RescanCustomerClassificationBySportResultModel> GetRescanCustomerClassificationBySport(IEnumerable<RescanCustomerClassificationBySportRequestModel> custInfos)
        {
            if (custInfos.IsNotNullOrEmpty())
            {
                var custJson = JsonConvert.SerializeObject(custInfos);

                var criteria = new GetRescanCustomerClassificationBySportCriteria
                {
                    CustInfoJson = custJson,
                };

                return _repository.Fetch<RescanCustomerClassificationBySportResultModel>(criteria);
            }

            return Enumerable.Empty<RescanCustomerClassificationBySportResultModel>();
        }

        private void ResanNormalBySportForNewCustomers(IEnumerable<RescanNormalBySportListCustModel> custIds, BySport_FunctionId functionId)
        {
            if (custIds != null && custIds.Any())
            {
                var custToString = JsonConvert.SerializeObject(custIds.Distinct());

                PreprocesRescanNormalClassificationBySport(custToString, functionId);
            }
        }

        private void PreprocesRescanNormalClassificationBySport(string custIds, BySport_FunctionId functionId)
        {
            if (!string.IsNullOrEmpty(custIds))
            {
                var criteria = new PreprocessRescanNormalClassificationBySportCriteria
                {
                    ListCustId = custIds,
                    FunctionId = (int)functionId,
                    QueryType = (int)BySport_RescanNormalType.Manual
                };

                _repository.Execute(criteria);
            }
        }
        #endregion
    }
}
