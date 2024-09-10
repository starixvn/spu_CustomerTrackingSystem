namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using System.Collections.Generic;

    public class ResultInsertCustomerClassificationBySportModel
    {
        public IEnumerable<CustInfoClassificationBySportModel> CustInfos { get; set; }

        public string ErrorMessage { get; set; }
    }
}
