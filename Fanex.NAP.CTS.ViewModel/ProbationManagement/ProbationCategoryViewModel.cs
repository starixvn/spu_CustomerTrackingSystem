namespace Fanex.NAP.CTS.ViewModel.ProbationManagement
{
    public class ProbationCategoryViewModel
    {
        public int CustId { get; set; }

        public bool IsNotAccumulatedInfo { get; set; }

        public string UserName { get; set; }

        public string SiteName { get; set; }

        public string Status { get; set; }

        public int SportGroupId { get; set; }        

        public string SportName { get; set; }

        public string TargetCategoryName { get; set; }

        public string TaggedDate { get; set; }

        public bool AssociatedWithPA { get; set; }

        public decimal CurrentWinLoss { get; set; }

        public string CurrentWinLossString { get; set; }

        public decimal FirstMargin { get; set; }

        public decimal CurrentMargin { get; set; }

        public string CurrentMarginString { get; set; }
        
        public int DaysLeft { get; set; }
    }
}
