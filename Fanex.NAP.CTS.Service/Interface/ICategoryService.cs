namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Model.API.CustomerClassCategory;
    using System.Collections.Generic;

    public interface ICategoryService
    {
        IEnumerable<CategoryCustomerClassModel> GetCustomerClassCategory();

        IEnumerable<CustomerClassCategoryByCustIDModel> GetCustomerClassCategoryByCustID(long custid);
    }
}
