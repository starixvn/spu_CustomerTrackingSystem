namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Core.Service;
    using Fanex.NAP.CTS.Criteria.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;   
    using Fanex.NAP.CTS.Service.Interface;

    public class BaseCustomerClassificationService : IBaseCustomerClassificationService
    {
        private readonly ICriteriaService repository;

        public BaseCustomerClassificationService()
        {
            repository = new CriteriaService();
        }

        public GetRescanCustomerClassificationModel GetRescanCustomerClassification(string custIDs)
        {
            var criteria = new GetRescanCustomerClassificationCriteria
            {
                CustIDs = custIDs
            };

            var custInfos = repository.Fetch<RescanCustomerClassificationModel>(criteria);

            return new GetRescanCustomerClassificationModel { CustInfos = custInfos };
        }

        public ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInputFlow inputFlow, InsertCustomerClassificationModel model)
        {
            var criteria = new InsertCustomerClassificationCriteria
            {
                InputFlowID = (int)inputFlow,
                CustClassificationJson = model.CustClassificationJson,
                IsToQueue = model.IsToQueue,
                QueueID = model.QueueID,
                UplineRoleID = model.UplineRoleID,
                IsAffectDownline = model.IsAffectDownline,
                FromAction = model.FromAction
            };

            var custInfos = repository.Fetch<CustInfoClassificationModel>(criteria);

            return new ResultInsertCustomerClassificationModel { CustInfos = custInfos, ErrorMessage = criteria.ErrorMessage };
        }

        public ResultRemoveCustomerClassificationModel RemoveCustomerClassification(CCInputFlow inputFlow, RemoveCustomerClassificationModel model)
        {
            var criteria = new RemoveCustomerClassificationCriteria
            {
                InputFlowID = (int)inputFlow,
                FromAction = model.FromAction,
                UplineRoleID = model.UplineRoleID,
                QueueID = model.QueueID,
                IsAffectDownline = model.IsAffectDownline,
                IsFromQueue = model.IsFromQueue,
                CustClassificationJson = model.CustClassificationJson
            };

            var custInfos = repository.Fetch<ResultRemoveCCInfoModel>(criteria);

            return new ResultRemoveCustomerClassificationModel { CustInfos = custInfos, ErrorMessage = criteria.ErrorMessage };
        }
    }
}
