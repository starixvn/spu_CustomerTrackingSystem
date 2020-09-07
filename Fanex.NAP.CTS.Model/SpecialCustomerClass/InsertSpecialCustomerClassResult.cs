namespace Fanex.NAP.CTS.Model.SpecialCustomerClass
{
    using System.Collections.Generic;

    public class InsertSpecialCustomerClassResult
    {
        public IEnumerable<SpecialCustomerClassForMainDb> CustomerClassList { get; set; }

        public string OpErrorMessage { get; set; }

        public int ErrorCode { get; set; }

    }
}