namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Criteria.CustomerClassify;
    using Fanex.NAP.CTS.Resource;
    using Fanex.NAP.CTS.Service.Interface;
    using System;

    public class ClassifyCustomerService : IClassifyCustomerService
    {
        private IDynamicRepository _repository;

        public ClassifyCustomerService()
        {
            _repository = new DynamicRepository();
        }

        public void ClassifyCustomerGeneralCategoryDaily(string custIds)
        {         
            var criteria = new NormalAccountsClassificationGeneralCategorizeDailyCriteria
            {
                ListCustIds = custIds
            };

            try
            {
                _repository.Execute(criteria);
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(ResourcesManager.GetValue(ConstsResources.INSERT_CTS_USER_ERROR), custIds);
                LogService.Error(errorMessage, ex);
            }
        }
    }
}
