namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Service.Interface;

    public class BaseCustomerClassificationService : IBaseCustomerClassificationService
    {
        private readonly IDynamicRepository repository;

        public BaseCustomerClassificationService()
        {
            repository = new DynamicRepository();
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
                AffectDownline = model.AffectDownline,
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
                CustClassificationJson = model.CustClassificationJson,
                AffectDownline = model.AffectDownline,
                FromAction = model.FromAction
            };

            var custInfos = repository.Fetch<CustInfoClassificationModel>(criteria);

            return new ResultRemoveCustomerClassificationModel { CustInfos = custInfos };
        }
    }
}
