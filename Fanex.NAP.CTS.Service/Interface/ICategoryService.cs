namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.API;
    using System.Collections.Generic;

    public interface ICategoryService
    {
        IEnumerable<CategoryCustomerClassModel> GetCustomerClassCategory();
    }
}
