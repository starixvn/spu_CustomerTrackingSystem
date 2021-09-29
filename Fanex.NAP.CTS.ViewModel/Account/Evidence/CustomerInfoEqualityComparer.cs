namespace Fanex.NAP.CTS.ViewModel.Account.Evidence
{
    using System.Collections.Generic;

    public class CustomerInfoEqualityComparer : IEqualityComparer<CTSCustomerEvidenceViewModel>
    {
        public bool Equals(CTSCustomerEvidenceViewModel x, CTSCustomerEvidenceViewModel y)
        {
            if (x.CustID == y.CustID
                && x.CTSCustID == y.CTSCustID)
                return true;

            return false;
        }

        public int GetHashCode(CTSCustomerEvidenceViewModel obj)
        {
            return obj.GetHashCode();
        }
    }
}