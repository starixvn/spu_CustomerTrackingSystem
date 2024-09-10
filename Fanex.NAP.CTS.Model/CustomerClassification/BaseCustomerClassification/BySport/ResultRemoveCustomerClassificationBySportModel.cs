namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using System.Collections.Generic;

    public class ResultRemoveCustomerClassificationBySportModel
    {
        public IEnumerable<ResultRemoveCCInfoBySportModel> CustInfos { get; set; }

        public string ErrorMessage { get; set; }
    }
}
