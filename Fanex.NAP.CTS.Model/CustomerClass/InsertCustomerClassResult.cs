namespace Fanex.NAP.CTS.Model.CustomerClass
{
    using System.Collections.Generic;

    public class InsertCustomerClassResult
    {
        public IEnumerable<CustomerClassForMainDb> CustomerClassList { get; set; }

        public string OpErrorMessage { get; set; }

        public int ErrorCode { get; set; }

    }
}