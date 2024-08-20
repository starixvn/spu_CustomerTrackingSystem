namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Model.API.CustomerClassCategory;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using System.Collections.Generic;

    public interface ICustomerCategoryService
    {
        IEnumerable<CategoryCustomerClassModel> GetCustomerClassCategory();

        ResultInsertCustomerClassificationModel MarkVVIPCustomerCategory(CustomerCategoryModel categoryModel);

        ResultRemoveCustomerClassificationModel UnmarkVVIPCustomerCategory(CustomerCategoryModel categoryModel);

        IEnumerable<CustomerClassCategoryByCustIDModel> GetCustomerClassCategoryByCustID(long custid);
    }
}
