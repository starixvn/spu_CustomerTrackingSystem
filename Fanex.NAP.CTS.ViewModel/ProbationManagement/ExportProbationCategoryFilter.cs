namespace Fanex.NAP.CTS.ViewModel.ProbationManagement
{
    public class ExportProbationCategoryFilter
    {
        public string Username { get; set; }

        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public string StatusIDs { get; set; }

        public string StatusNames { get; set; }

        public string DaysLeft { get; set; }

        public int FromDaysLeft { get; set; }

        public int ToDaysLeft { get; set; }

        public int IsAssociatedPA { get; set; }

        public string AssociatedWithPA { get; set; }

        public string MinWinLoss { get; set; }

        public string MaxWinLoss { get; set; }

        public string MinMargin { get; set; }

        public string MaxMargin { get; set; }

        public int CategoryMode { get; set; }

    }
}
