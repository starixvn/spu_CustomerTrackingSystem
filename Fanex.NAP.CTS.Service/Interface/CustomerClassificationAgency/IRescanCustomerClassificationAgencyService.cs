namespace Fanex.NAP.CTS.Service.Interface.CustomerClassificationAgency
{
    using Fanex.NAP.CTS.Model.CustomerClassificationAgency.RescanCustomerClassificationAgency;

    public interface IRescanCustomerClassificationAgencyService
    {
        bool RescanCustomerClassificationAgency(RescanCustClassificationAgencyModel paras);
    }
}
