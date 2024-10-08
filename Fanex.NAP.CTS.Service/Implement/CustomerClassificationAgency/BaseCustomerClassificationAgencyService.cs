namespace Fanex.NAP.CTS.Service.Implement.CustomerClassificationAgency
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.CustomerClassificationAgency.BaseCustomerClassificationAgency;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.BaseCustomerClassificationAgency;

    public class BaseCustomerClassificationAgencyService
    {
        protected readonly IDynamicRepository _repository;

        public BaseCustomerClassificationAgencyService()
        {
            _repository = new DynamicRepository();
        }

        public GetRescanCustomerClassificationAgencyModel GetRescanCustomerClassificationAgency(string custIDs)
        {
            var criteria = new GetRescanCustomerClassificationAgencyCriteria
            {
                CustIDs = custIDs
            };

            var custInfos = _repository.Fetch<RescanCustomerClassificationAgencyModel>(criteria);

            return new GetRescanCustomerClassificationAgencyModel { CustInfos = custInfos };
        }

        public ResultInsertCustomerClassificationAgencyModel InsertCustomerClassificationAgency(AgencyCCInputFlow inputFlow, InsertCustomerClassificationAgencyModel model)
        {
            var criteria = new InsertCustomerClassificationAgencyCriteria
            {
                InputFlowID = (int)inputFlow,
                CustClassificationJson = model.CustClassificationJson,
                IsToQueue = model.IsToQueue,
                QueueID = model.QueueID,
                UplineRoleID = model.UplineRoleID,
                IsAffectDownline = model.IsAffectDownline,
                FromAction = model.FromAction
            };

            var custInfos = _repository.Fetch<CustInfoClassificationAgencyModel>(criteria);

            return new ResultInsertCustomerClassificationAgencyModel { CustInfos = custInfos, ErrorMessage = criteria.ErrorMessage };
        }

        public ResultRemoveCustomerClassificationAgencyModel RemoveCustomerClassificationAgency(AgencyCCInputFlow inputFlow, RemoveCustomerClassificationAgencyModel model)
        {
            var criteria = new RemoveCustomerClassificationAgencyCriteria
            {
                InputFlowID = (int)inputFlow,
                FromAction = model.FromAction,
                UplineRoleID = model.UplineRoleID,
                QueueID = model.QueueID,
                IsAffectDownline = model.IsAffectDownline,
                IsFromQueue = model.IsFromQueue,
                CustClassificationJson = model.CustClassificationJson
            };

            var custInfos = _repository.Fetch<ResultRemoveCCInfoAgencyModel>(criteria);

            return new ResultRemoveCustomerClassificationAgencyModel { CustInfos = custInfos, ErrorMessage = criteria.ErrorMessage };
        }
    }
}
