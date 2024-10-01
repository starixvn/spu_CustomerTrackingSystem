namespace Fanex.NAP.CTS.Model.CustomerClassificationAgency.BaseCustomerClassificationAgency
{
    using System.Collections.Generic;

    public class ResultRemoveCustomerClassificationAgencyModel
    {
        public IEnumerable<ResultRemoveCCInfoAgencyModel> CustInfos { get; set; }

        public string ErrorMessage { get; set; }
    }
}