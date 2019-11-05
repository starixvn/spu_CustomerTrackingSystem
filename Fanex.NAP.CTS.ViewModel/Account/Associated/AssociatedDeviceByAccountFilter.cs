namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using Fanex.NAP.CTS.Model;

    public class AssociatedDeviceByAccountFilter
    {
        public int AccountId { get; set; }

        public bool UsedByAccountCurrent { get; set; }

        public bool UsedByAccountAssociated { get; set; }

        public EvidenceStatus EvidenceStatus { get; set; }        
    }
}