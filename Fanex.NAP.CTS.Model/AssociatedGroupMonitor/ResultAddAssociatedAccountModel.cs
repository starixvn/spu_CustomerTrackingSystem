namespace Fanex.NAP.CTS.Model.AssociatedGroupMonitor
{
    using System.Collections.Generic;

    public class ResultAddAssociatedAccountModel
    {
        public int ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        public IEnumerable<CustInfoAssociatedAccountModel> CustInfos { get; set; }

    }
}