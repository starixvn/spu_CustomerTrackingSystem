namespace Fanex.NAP.CTS.Service.Interface.CustomerClassificationAgency
{
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.BaseCustomerClassificationAgency;

    public interface ICustomerCategoryAgencyService
    {
        ResultRemoveCustomerClassificationAgencyModel UnmarkVVIPCustomerCategory(CustomerCategoryModel custCategoryModel);
    }
}
