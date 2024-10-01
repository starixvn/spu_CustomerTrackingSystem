namespace Fanex.NAP.CTS.Model.CustomerClassificationAgency.BaseCustomerClassificationAgency
{
    using System.Collections.Generic;

    public class ResultInsertCustomerClassificationAgencyModel
    {
        public IEnumerable<CustInfoClassificationAgencyModel> CustInfos { get; set; }

        public string ErrorMessage { get; set; }
    }
}
