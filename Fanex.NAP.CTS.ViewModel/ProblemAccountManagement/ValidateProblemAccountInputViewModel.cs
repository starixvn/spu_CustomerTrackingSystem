namespace Fanex.NAP.CTS.ViewModel.ProblemAccountManagement
{
    using Fanex.NAP.CTS.Model;

    public class ValidateProblemAccountInputViewModel
    {
        public string UserNames { get; set; }

        public int CategoryID { get; set; }

        public int CategoryGroup { get; set; }

        public int CategoryIDAgency { get; set; }

        public int CategoryGroupAgency { get; set; }

        public string Comment { get; set; }

        public ProblemAccountAction PAAction { get; set; }

        public bool IsUnmarkDownlines { get; set; }

        public bool IsRobot { get; set; }
    }
}