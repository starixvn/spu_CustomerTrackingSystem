namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Criteria.Category;
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Resource;
    using System;
    using System.Collections.Generic;

    public class CategoryService : ICategoryService
    {
        private IDynamicRepository _repository;

        public CategoryService()
        {
            _repository = new DynamicRepository();
        }

        public IEnumerable<CategoryCustomerClassModel> GetCustomerClassCategory()
        {
            try
            {
                return _repository.Fetch<CategoryCustomerClassModel>(new GetCategoryCustomerClassCriteria());
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format(ResourcesManager.GetValue(ConstsResources.CATEGORY_CUSTOMER_CLASS));
                LogService.Error(errorMessage, ex);

                return new List<CategoryCustomerClassModel>();
            }
        }
    }
}
