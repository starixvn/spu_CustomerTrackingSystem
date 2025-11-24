namespace Fanex.NAP.CTS.Service.Interface.CustomerClassificationBySport.BaseCustomerClassificationBySport
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;

    public interface IBaseCustomerClassificationBySportService
    {
        ResultInsertCustomerClassificationBySportModel InsertCustomerClassificationBySport(CCInputFlow inputFlow, InsertCustomerClassificationBySportModel model);

        ResultRemoveCustomerClassificationBySportModel RemoveCustomerClassificationBySport(CCInputFlow inputFlow, RemoveCustomerClassificationBySportModel model);
    }
}
