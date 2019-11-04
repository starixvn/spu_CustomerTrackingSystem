namespace Fanex.NAP.CTS.Library.ViewModels.Account.Associated
{
    using Fanex.NAP.CTS.Library.Models;
    using System.Collections.Generic;

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