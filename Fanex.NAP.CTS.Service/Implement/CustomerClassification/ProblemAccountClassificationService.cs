namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.CustomerClassify;
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using Fanex.NAP.CTS.Model.ImportEvidences;
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using Fanex.NAP.CTS.Service.Interface;
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

        public ProblemAccountInsertResultModel InsertProblemAccountByCategory(IEnumerable<ProblemAccountValidatedModel> models, int userID)
        {
            var problemAccountJson = "";

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


    }
}
