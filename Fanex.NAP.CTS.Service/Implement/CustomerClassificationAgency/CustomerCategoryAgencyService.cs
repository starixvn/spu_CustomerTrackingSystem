namespace Fanex.NAP.CTS.Service.Implement.CustomerClassificationAgency
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.Account.CustomerCategory;
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.BaseCustomerClassificationAgency;
    using Fanex.NAP.CTS.Service.Interface.CustomerClassificationAgency;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class CustomerCategoryAgencyService : BaseCustomerClassificationAgencyService, ICustomerCategoryAgencyService
    {
        public CustomerCategoryAgencyService() : base() { }

        public ResultInsertCustomerClassificationAgencyModel MarkVVIPCustomerCategory(CustomerCategoryModel custCategoryModel)
        {
            var custList = new List<AddCustomerCategoryInfoModel>()
            {
                new AddCustomerCategoryInfoModel
                {
                    CustID = custCategoryModel.CustId,
                    CTSCustID = custCategoryModel.CTSCustId,
                    RoleID = (int)custCategoryModel.RoleId,
                    SubscriberID = custCategoryModel.SubscriberId,
                    CreatedBy = custCategoryModel.CreatedBy,
                    Remark = custCategoryModel.Remark
                }
            };

            var modelInsert = new InsertCustomerClassificationAgencyModel()
            {
                CustClassificationJson = JsonConvert.SerializeObject(custList),
                FromAction = (int)FromFlow.CTSWEB,
                IsAffectDownline = custCategoryModel.IsAffectToAllDownlines
            };

            return base.InsertCustomerClassificationAgency(AgencyCCInputFlow.InsertVVIP, modelInsert);
        }

        public ResultRemoveCustomerClassificationAgencyModel UnmarkVVIPCustomerCategory(CustomerCategoryModel custCategoryModel)
        {
            var custList = new List<RemoveCustomerCategoryInfoModel>()
            {
                new RemoveCustomerCategoryInfoModel
                {
                    CustID = custCategoryModel.CustId,
                    CTSCustID = custCategoryModel.CTSCustId,
                    //RoleID = custCategoryModel.RoleId,
                    CreatedBy = custCategoryModel.CreatedBy
                }
            };

            var modelRemove = new RemoveCustomerClassificationAgencyModel()
            {
                CustClassificationJson = JsonConvert.SerializeObject(custList),
                FromAction = (int)FromFlow.CTSWEB,
                IsAffectDownline = custCategoryModel.IsAffectToAllDownlines
            };

            return base.RemoveCustomerClassificationAgency(AgencyCCInputFlow.RemoveVVIP, modelRemove);
        }
    }
}
