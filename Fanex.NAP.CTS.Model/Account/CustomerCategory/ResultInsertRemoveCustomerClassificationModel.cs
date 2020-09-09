namespace Fanex.NAP.CTS.Model.Account
{
    using Fanex.NAP.CTS.Model.SpecialCustomerClass;
    using System.Collections.Generic;

    public class ResultInsertRemoveCustomerClassificationModel
    {
        public IEnumerable<CustomerClassForMainDb> CustomerClassList { get; set; }

        public string OpErrorMessage { get; set; }
    }
}