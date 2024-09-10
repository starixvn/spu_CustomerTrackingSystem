namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using System.Collections.Generic;

    public class ResultInsertCustomerClassificationModel
    {
        public IEnumerable<CustInfoClassificationModel> CustInfos { get; set; }

        public string ErrorMessage { get; set; }
    }
}
