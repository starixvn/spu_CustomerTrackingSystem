namespace Fanex.NAP.CTS.Model.CustomerClassification.ProblemAccount
{
    using System.Collections.Generic;

    public class UnmarkPAResultModel
    {
        public string ErrorMessage { get; set; }

        public int ErrorCode { get; set; }

        public IEnumerable<CustInfoModel> CustomerInfo { get; set; }

    }
}
