namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using System.Collections.Generic;

    public class GetRescanCustomerClassificationModel
    {
        public IEnumerable<RescanCustomerClassificationModel> CustInfos { get; set; }
    }
}
