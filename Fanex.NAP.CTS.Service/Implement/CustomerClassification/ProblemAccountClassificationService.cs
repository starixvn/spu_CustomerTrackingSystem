namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.CustomerClassify;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using Fanex.NAP.CTS.Service.Interface;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ProblemAccountClassificationService : IProblemAccountClassificationService
    {
        private readonly IDynamicRepository _repository;

        public ProblemAccountClassificationService()
        {
            _repository = new DynamicRepository();
        }

        public ProblemAccountInsertResultModel InsertProblemAccountByCategory(IEnumerable<InsertProblemAccountModel> problemAccounts, IEnumerable<ProblemAccountLosingClassificationModel> probAccountLosing, int userID)
        {
            var proAccountClassification = MergeProblemAccountClassification(problemAccounts, probAccountLosing);

            if (proAccountClassification.Any())
            {
                var problemAccountJson = JsonConvert.SerializeObject(proAccountClassification);

                var criteria = new ProblemAccountInsertByCategoryCriteria
                {
                    ProblemAccountJson = problemAccountJson
                };

                var assCustForRollback = _repository.Fetch<CheckPACustomerAssociationForRollbackModel>(criteria);

                return new ProblemAccountInsertResultModel
                {
                    ErrorMessage = criteria.ErrorMessage,
                    ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                    CustIds = assCustForRollback?.Select(x => x.CustID) ?? Enumerable.Empty<long>(),
                    NewPAAssociationCustomerForRollback = assCustForRollback
                };
            }

            return new ProblemAccountInsertResultModel { CustIds = new List<long>(), ErrorCode = 0, ErrorMessage = string.Empty };
        }

        private static IEnumerable<InsertProblemAccountModel> MergeProblemAccountClassification(IEnumerable<InsertProblemAccountModel> problemAccounts, IEnumerable<ProblemAccountLosingClassificationModel> probAccountLosing)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                return problemAccounts.GroupJoin(probAccountLosing,
                                            pa => pa.CustID,
                                            paLosing => paLosing.CustId,
                                            (pa, paLosing) => new { pa, paLosing })
                                            .SelectMany(
                                                 group => group.paLosing.DefaultIfEmpty(),
                                                 (group, temp) => new InsertProblemAccountModel
                                                 {
                                                     CustID = group.pa.CustID,
                                                     ReasonID = group.pa.ReasonID,
                                                     RobotCounter = group.pa.RobotCounter,
                                                     CreatedBy = group.pa.CreatedBy,
                                                     BetCount = temp?.BetCount,
                                                     ActiveDays = temp?.ActiveDays,
                                                     TurnoverRM = temp?.TurnoverRM,
                                                     WinlossRM = temp?.WinlossRM,
                                                     WinlossStatus = temp?.WinlossStatus
                                                 });
            }

            return Enumerable.Empty<InsertProblemAccountModel>();
        }
    }
}
