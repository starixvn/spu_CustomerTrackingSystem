namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;

    public interface IBaseCustomerClassificationService
    {
        GetRescanCustomerClassificationModel GetRescanCustomerClassification(string custIDs);

        ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInputFlow inputFlow, InsertCustomerClassificationModel model);

        ResultRemoveCustomerClassificationModel RemoveCustomerClassification(CCInputFlow inputFlow, RemoveCustomerClassificationModel model);        
    }
}
