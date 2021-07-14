namespace Fanex.NAP.CTS.Model.Account
{
    public class SuspiciousCustomerModel
    {
        public int CustId { get; set; }

        public bool IsDelinquent { get; set; }

        public bool IsHedge { get; set; }

        public bool IsDirty { get; set; }
        
    }
}
