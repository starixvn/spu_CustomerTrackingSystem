namespace Fanex.NAP.CTS.Model.ProbationManagement
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.SpecialCustomerClass;
    using System.Collections.Generic;

    public class ResultChangeCategoryToCts
    {
        public DBResult DBResult { get; set; }

        public IEnumerable<CustomerClassForMainDb> CustomerClassList { get; set; }
    }
}
