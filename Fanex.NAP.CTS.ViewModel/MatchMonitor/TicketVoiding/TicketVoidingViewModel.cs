namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using Fanex.NAP.CTS.Core.Utilities;
    using System.Collections.Generic;

    public class TicketVoidingViewModel
    {
        public TicketVoidingViewModel(){
            IsTest = false;
            SubIssueTypeId = string.Empty;
            ActionId = string.Empty;
            TVSRedirectURL = ConfigSettings.TVSTicketVoidingURL;
        }

        public string UserId { get; set; }

        public string OnUserId { get; set; }

        public string Time { get; set; }

        public bool IsTest { get; set; }

        public string TransIds { get; set; }

        public string SubIssueTypeId { get; set; }

        public string ActionId { get; set; }

        public string TVSRedirectURL { get; set; }
    }
}