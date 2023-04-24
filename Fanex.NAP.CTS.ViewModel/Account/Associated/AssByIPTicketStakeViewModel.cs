namespace Fanex.NAP.CTS.ViewModel.Account
{
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using System;
    using System.Collections.Generic;

    public class AssByIPTicketStakeViewModel
    {
        public AssByIPTicketStakeViewModel() { }
      
        public DateTime MaxEventDateInGroup { get; set; }

        public string IP { get; set; }

        public IEnumerable<AssByIPTicketStakeItemViewModel> Items { get; set; }

    }
}