namespace Fanex.NAP.CTS.ViewModel.ProblemAccountMonitor
{
    public class VoidRequestToTVSParam
    {
        public string EnCryptedUserId { get; set; }

        public string EnCryptedOnUserId { get; set; }

        public string EnCryptedTime { get; set; }

        public bool IsTest { get; set; }

        public int SubIssueTypeId { get; set; }

        public int ActionId { get; set; }

        public string RedirectUrl { get; set; }

    }
}