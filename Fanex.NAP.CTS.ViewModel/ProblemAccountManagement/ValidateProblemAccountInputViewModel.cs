namespace Fanex.NAP.CTS.ViewModel.ProblemAccountManagement
{
    using Fanex.NAP.CTS.Model;

    public class ValidateProblemAccountInputViewModel
    {
        public string UserNames { get; set; }

        public int CategoryID { get; set; }

        public string Comment { get; set; }

        public ProblemAccountAction PAAction { get; set; }

        public bool IsUnmarkDownlines { get; set; }
       
    }
}