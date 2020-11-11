using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.NAP.CTS.Core;
using Fanex.NAP.CTS.Criteria.CustomerClassification;
using Fanex.NAP.CTS.Model.Account;
using Fanex.NAP.CTS.Resource;
using Fanex.NAP.CTS.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Service.Implement
{
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
