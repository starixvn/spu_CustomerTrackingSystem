namespace Fanex.NAP.CTS.Service.Interface.CustomerClassificationBySport
{
    using Fanex.NAP.CTS.Model.CustomerClassificationBySport.RescanCustomerClassificationBySport;

    public interface IRescanCustomerClassificationBySportService
    {
        bool RescanCustomerClassificationBySport(RescanCustClassificationBySportModel rescanModel);
    }
}
