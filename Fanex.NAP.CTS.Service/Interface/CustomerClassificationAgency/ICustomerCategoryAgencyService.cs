namespace Fanex.NAP.CTS.Service.Interface.CustomerClassificationAgency
{
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.BaseCustomerClassificationAgency;

    public interface ICustomerCategoryAgencyService
    {
        ResultInsertCustomerClassificationAgencyModel MarkVVIPCustomerCategory(CustomerCategoryModel custCategoryModel);

        ResultRemoveCustomerClassificationAgencyModel UnmarkVVIPCustomerCategory(CustomerCategoryModel custCategoryModel);
    }
}
