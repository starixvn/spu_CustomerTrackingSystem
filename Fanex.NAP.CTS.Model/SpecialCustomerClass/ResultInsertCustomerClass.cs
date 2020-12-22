namespace Fanex.NAP.CTS.Model.SpecialCustomerClass
{
    using Fanex.NAP.CTS.Model.CustomerClassify;
    using System.Collections.Generic;

    public class ResultInsertCustomerClass
    {
        public IEnumerable<LatestClassificationModel> CustomerClassList { get; set; }

        public string OpErrorMessage { get; set; }

    }
}