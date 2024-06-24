namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.CustomerClassification;
    using System;
    using System.Collections.Generic;

    public interface IBaseCustomerClassificationService
    {
        ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInsertInputFlow inputFlow, InsertCustomerClassificationModel model);
    }
}
