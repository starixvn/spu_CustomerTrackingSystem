namespace Fanex.NAP.CTS.ViewModels.Account.Associated
{
    using Fanex.NAP.CTS.Model;

    public class AssociatedAccountByAccountFilter
    {
        public int AccountId { get; set; }

        public int CustId { get; set; }        

        public bool DeviceAwayOne { get; set; }

        public bool DeviceAwayMore { get; set; }        

        public string Status { get; set; }

        public EvidenceStatus EvidenceStatus { get; set; }

        public int userId { get; set; }

        public bool isMasterSubscriberAssignment { get; set; }
    }
}