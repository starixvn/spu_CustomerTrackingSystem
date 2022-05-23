namespace Fanex.NAP.CTS.Model.CustomerClassification
{
    using System.Collections.Generic;

    public class InsertPAResultModel
    {
        public string ErrorMessage { get; set; }

        public int ErrorCode { get; set; }

        public IEnumerable<CustInfoModel> CustomerInfo { get; set; }
    }
}
