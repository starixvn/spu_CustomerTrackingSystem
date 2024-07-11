namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using System.Collections.Generic;

    public class ResultRemoveCustomerClassificationModel
    {
        public IEnumerable<ResultRemoveCCInfoModel> CustInfos { get; set; }

        public string ErrorMessage { get; set; }
    }
}
