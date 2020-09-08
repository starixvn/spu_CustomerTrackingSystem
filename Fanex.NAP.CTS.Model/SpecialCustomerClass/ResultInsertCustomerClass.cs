namespace Fanex.NAP.CTS.Model.SpecialCustomerClass
{
    using System.Collections.Generic;

    public class ResultInsertCustomerClass
    {
        public IEnumerable<CustomerClassForMainDb> CustomerClassList { get; set; }

        public string OpErrorMessage { get; set; }

    }
}