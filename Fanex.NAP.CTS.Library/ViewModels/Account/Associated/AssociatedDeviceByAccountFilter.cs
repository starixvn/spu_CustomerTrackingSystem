namespace Fanex.NAP.CTS.Library.ViewModels.Account.Associated
{
    using Fanex.NAP.CTS.Library.Models;

    public class AssociatedDeviceByAccountFilter
    {
        public int AccountId { get; set; }

        public bool UsedByAccountCurrent { get; set; }

        public bool UsedByAccountAssociated { get; set; }

        public EvidenceStatus EvidenceStatus { get; set; }        
    }
}