namespace Fanex.NAP.CTS.ViewModel.ProbationManagement
{
    public class ProbationCategoryViewModel
    {
        public long CTSCustId { get; set; }

        public int CustId { get; set; }

        public string UserName { get; set; }

        public string SiteName { get; set; }

        public string SportName { get; set; }

        public string TargetCategoryName { get; set; }

        public string TargetDate { get; set; }

        public decimal FirstMargin { get; set; }

        public decimal CurrentMargin { get; set; }

        public int DayLeft { get; set; }
    }
}
