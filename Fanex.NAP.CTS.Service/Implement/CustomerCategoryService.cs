namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Criteria.Category;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.Account.CustomerCategory;
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using Fanex.NAP.CTS.Resource;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class CustomerCategoryService : BaseCustomerClassificationService, ICustomerCategoryService
    {
        private IDynamicRepository _repository;

        public CustomerCategoryService()
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

        public ResultInsertCustomerClassificationModel MarkVVIPCustomerCategory(CustomerCategoryModel custCategoryModel)
        {
            var custList = new List<AddCustomerCategoryInfoModel>()
            {
                new AddCustomerCategoryInfoModel
                {
                    CustID = custCategoryModel.CustId,
                    CTSCustID = custCategoryModel.CTSCustId,
                    RoleID = custCategoryModel.RoleId,
                    SubscriberID = custCategoryModel.SubscriberId,
                    CreatedBy = custCategoryModel.CreatedBy,
                    Remark = custCategoryModel.Remark
                }
            };

            var modelInsert = new InsertCustomerClassificationModel()
            {
                CustClassificationJson = JsonConvert.SerializeObject(custList),
                FromAction = (int)FromFlow.CTSWEB,
                AffectDownline = custCategoryModel.IsAffectToAllDownlines
            };

            return base.InsertCustomerClassification(CCInputFlow.VVIP, modelInsert);
        }

        public ResultRemoveCustomerClassificationModel UnmarkVVIPCustomerCategory(CustomerCategoryModel custCategoryModel)
        {
            var custList = new List<RemoveCustomerCategoryInfoModel>()
            {
                new RemoveCustomerCategoryInfoModel
                {
                    CustID = custCategoryModel.CustId,
                    CTSCustID = custCategoryModel.CTSCustId,
                    RoleID = custCategoryModel.RoleId,
                    CreatedBy = custCategoryModel.CreatedBy
                }
            };

            var modelRemove = new RemoveCustomerClassificationModel()
            {
                CustClassificationJson = JsonConvert.SerializeObject(custList),
                FromAction = (int)FromFlow.CTSWEB,
                AffectDownline = custCategoryModel.IsAffectToAllDownlines
            };

            return base.RemoveCustomerClassification(CCInputFlow.RemoveVVIP, modelRemove);
        }

    }
}
