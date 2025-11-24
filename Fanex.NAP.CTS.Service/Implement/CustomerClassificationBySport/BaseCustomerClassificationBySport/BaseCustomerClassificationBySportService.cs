namespace Fanex.NAP.CTS.Service.Implement.CustomerClassificationBySport.BaseCustomerClassificationBySport
{
    using Fanex.NAP.CTS.Core.Service;
    using Fanex.NAP.CTS.Criteria.CustomerClassificationBySport.BaseCustomerClassificationBySport;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Service.Interface.CustomerClassificationBySport.BaseCustomerClassificationBySport;

    public class BaseCustomerClassificationBySportService : IBaseCustomerClassificationBySportService
    {
        private readonly CriteriaService _repository;

        public BaseCustomerClassificationBySportService()
        {
            _repository = new CriteriaService();
        }

        public ResultInsertCustomerClassificationBySportModel InsertCustomerClassificationBySport(CCInputFlow inputFlow, InsertCustomerClassificationBySportModel model)
        {
            var criteria = new InsertCustomerClassificationBySportCriteria
            {
                InputFlowID = (int)inputFlow,
                CustClassificationJson = model.CustClassificationJson
            };

            var custInfos = _repository.Fetch<CustInfoClassificationBySportModel>(criteria);

            return new ResultInsertCustomerClassificationBySportModel { CustInfos = custInfos, ErrorMessage = criteria.ErrorMessage };
        }

        public ResultRemoveCustomerClassificationBySportModel RemoveCustomerClassificationBySport(CCInputFlow inputFlow, RemoveCustomerClassificationBySportModel model)
        {
            var criteria = new RemoveCustomerClassificationBySportCriteria
            {
                InputFlowID = (int)inputFlow,
                CustClassificationJson = model.CustClassificationJson
            };

            var custInfos = _repository.Fetch<ResultRemoveCCInfoBySportModel>(criteria);

            return new ResultRemoveCustomerClassificationBySportModel { CustInfos = custInfos, ErrorMessage = criteria.ErrorMessage };
        }
    }
}
