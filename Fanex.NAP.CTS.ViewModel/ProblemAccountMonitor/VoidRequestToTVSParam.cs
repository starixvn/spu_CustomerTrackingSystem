namespace Fanex.NAP.CTS.ViewModel.ProblemAccountMonitor
{
    using System.Collections.Generic;

    public class VoidRequestToTVSParam
    {
        public string UserId { get; set; }

        public string OnUserId { get; set; }

        public string Time { get; set; }

        public bool IsTest { get; set; }

        public int SubIssueTypeId { get; set; }

        public int ActionId { get; set; }

        public string TransIds { get; set; }

        public string TargetUrl { get; set; }

    }
}
