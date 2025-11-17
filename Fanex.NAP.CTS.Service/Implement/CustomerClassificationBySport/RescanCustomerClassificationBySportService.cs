namespace Fanex.NAP.CTS.Service.Implement.CustomerClassificationBySport
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Service;
    using Fanex.NAP.CTS.Criteria.CustomerClassificationBySport.RescanNormalClassificationBySport;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassificationBySport;
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

        public bool RescanCustomerClassificationBySport(IEnumerable<BaseCustomerInfoBySportModel> custInfos, BySport_FunctionId functionId, int batchSize)
        {
            var result = new RescanBySportResultModel();

            var custRescans = GetRescanCustomerClassificationBySport(custInfos);

            if (custRescans.IsNotNullOrEmpty())
            {
                var groupingCustRescans = custRescans.GroupBy(x => x.InputFlowID);

                foreach (var group in groupingCustRescans)
                {
                    if ((BySportCCInputFlow)group.Key == BySportCCInputFlow.RescanNormal)
                    {
                        var normalCustomerBySports =
                            group.Select(x => new RescanNormalBySportListCustModel
                            {
                                CustID = x.CustID,
                                SportID = x.SportGroup,
                            });

                        ResanNormalBySportForNewCustomers(normalCustomerBySports, functionId);
                    }
                    else
                    {
                        result = RescanProblemAccountBySport(group, (BySportCCInputFlow)group.Key, functionId, batchSize);
                    }
                }
            }

            return result.IsSuccess;
        }

        #region Private Methods
        private RescanBySportResultModel RescanProblemAccountBySport(IEnumerable<RescanCustomerClassificationBySportModel> custInfos, BySportCCInputFlow ccInputFlow, BySport_FunctionId functionId, int batchSize)
        {
            var mappingCustInfoInsert = custInfos.Select(x => new InsertProblemAccountBySportModel(x));

            var paInsertResult = _problemAccountBySport.InsertProblemAccountBySport(mappingCustInfoInsert, ccInputFlow, functionId);

            var isSuccess = _customerClassificationBySport.PushCCBySportToMainDB(paInsertResult, batchSize);

            return new RescanBySportResultModel
            {
                CustInfos = paInsertResult,
                IsSuccess = isSuccess,
            };
        }

        private IEnumerable<RescanCustomerClassificationBySportModel> GetRescanCustomerClassificationBySport(IEnumerable<BaseCustomerInfoBySportModel> custInfos)
        {
            if (custInfos.IsNotNullOrEmpty())
            {
                var custJson = JsonConvert.SerializeObject(custInfos);

                var criteria = new GetRescanCustomerClassificationBySportCriteria
                {
                    CustInfoJson = custJson,
                };

                return _repository.Fetch<RescanCustomerClassificationBySportModel>(criteria);
            }

            return Enumerable.Empty<RescanCustomerClassificationBySportModel>();
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
