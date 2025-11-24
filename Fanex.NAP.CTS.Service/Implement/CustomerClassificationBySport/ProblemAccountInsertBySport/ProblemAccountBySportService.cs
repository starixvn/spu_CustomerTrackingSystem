namespace Fanex.NAP.CTS.Service.Implement.CustomerClassificationBySport.ProblemAccountInsertBySport
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Service;
    using Fanex.NAP.CTS.Criteria.CustomerClassificationBySport.BaseCustomerClassificationBySport;
    using Fanex.NAP.CTS.Criteria.CustomerClassificationBySport.ProblemAccountInsertBySport;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Model.CustomerClassificationBySport;
    using Fanex.NAP.CTS.Model.CustomerClassificationBySport.ProblemAccountInsertBySport;
    using Fanex.NAP.CTS.Service.Interface.CustomerClassificationBySport.ProblemAccountInsertBySport;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class ProblemAccountBySportService : IProblemAccountBySportService
    {
        private readonly CriteriaService _criteriaService;

        public ProblemAccountBySportService()
        {
            _criteriaService = new CriteriaService();
        }

        #region Public Methods
        public IEnumerable<ProblemAccountLosingBySportModel> GetPALosingBySport(IEnumerable<BaseCustomerInfoBySportModel> custInfos, BySport_FunctionId functionId)
        {
            if (custInfos.IsNotNullOrEmpty())
            {
                var custInfosJson = JsonConvert.SerializeObject(custInfos);

                var criteria = new ProblemAccountClassificationBySportGetCriteria
                {
                    CustJson = custInfosJson,
                    FunctionId = (int)functionId,
                };

                return _criteriaService.Fetch<ProblemAccountLosingBySportModel>(criteria);
            }

            return Enumerable.Empty<ProblemAccountLosingBySportModel>();
        }

        public IEnumerable<InsertPABySportResultModel> InsertProblemAccountBySport(IEnumerable<InsertProblemAccountBySportModel> paBySports, CCInputFlow cCInputFlow, BySport_FunctionId functionId)
        {
            if (paBySports.IsNotNullOrEmpty())
            {
                var paBySportClassification = CombinePAWithLosingPerformace(paBySports, functionId);

                var criteria = new InsertCustomerClassificationBySportCriteria
                {
                    InputFlowID = (int)cCInputFlow,
                    CustClassificationJson = JsonConvert.SerializeObject(paBySportClassification)
                };

                var custInfos = _criteriaService.Fetch<CustInfoClassificationBySportModel>(criteria);


                if (custInfos.IsNotNullOrEmpty())
                {
                    return custInfos.Select(x => new InsertPABySportResultModel
                    {
                        CustID = x.CustID,
                        SportGroup = x.SportGroup
                    }).Distinct();
                }
            }

            return Enumerable.Empty<InsertPABySportResultModel>();
        }
        #endregion

        #region Private Methods
        private IEnumerable<InsertProblemAccountBySportModel> CombinePAWithLosingPerformace(IEnumerable<InsertProblemAccountBySportModel> paBySports, BySport_FunctionId functionId)
        {
            var custInfos = paBySports.Select(x => new BaseCustomerInfoBySportModel
            {
                CustID = x.CustID,
                SportGroup = x.SportGroup
            }).Distinct(x => new { x.CustID, x.SportGroup });

            var paBySportLosings = GetPALosingBySport(custInfos, functionId);

            return MergePALosingClassification(paBySports, paBySportLosings);
        }

        private IEnumerable<InsertProblemAccountBySportModel> MergePALosingClassification(IEnumerable<InsertProblemAccountBySportModel> paBySports
            , IEnumerable<ProblemAccountLosingBySportModel> paBySportLosings)
        {
            if (paBySports.IsNotNullOrEmpty())
            {
                return paBySports.GroupJoin(paBySportLosings ?? Enumerable.Empty<ProblemAccountLosingBySportModel>(),
                                    pa => $"{pa.CustID}_{pa.SportGroup}",
                                    pl => $"{pl.CustId}_{pl.SportGroup}",
                                    (pa, pl) => new { pa, pl })
                    .SelectMany(
                         group => group.pl.DefaultIfEmpty(),
                          (group, temp) => new InsertProblemAccountBySportModel
                          {
                              CTSCustID = group.pa.CTSCustID,
                              CustID = group.pa.CustID,
                              SubscriberID = group.pa.SubscriberID,
                              CategoryID = group.pa.CategoryID,
                              IsLicensee = group.pa.IsLicensee,
                              CreatedBy = group.pa.CreatedBy,
                              CreatedDate = group.pa.CreatedDate,
                              Remark = group.pa.Remark,
                              MatchID = group.pa.MatchID,
                              SportGroup = group.pa.SportGroup,

                              BetCount = temp?.BetCount,
                              ActiveDays = temp?.ActiveDays,
                              TurnoverRM = temp?.TurnoverRM,
                              WinlossRM = temp?.WinlossRM,
                              WinlossStatus = temp?.WinlossStatus,
                              PerformanceTime = temp?.PerformanceTime
                          }
                    );
            }

            return Enumerable.Empty<InsertProblemAccountBySportModel>();
        }

        #endregion
    }
}
