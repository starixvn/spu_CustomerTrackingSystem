namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.RescanCustClassificationModel;

    public interface IRescanCustomerClassificationService
    {
        bool RescanCustomerClassification(RescanCustClassificationModel parameters);
    }
}
