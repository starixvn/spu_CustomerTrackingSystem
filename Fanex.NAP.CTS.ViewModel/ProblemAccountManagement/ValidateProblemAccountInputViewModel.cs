namespace Fanex.NAP.CTS.ViewModel.ProblemAccountManagement
{
    using Fanex.NAP.CTS.Model;

    public class ValidateProblemAccountInputViewModel
    {
        public string UserNames { get; set; }

        public int CategoryID { get; set; }

        public ProblemAccountAction PAAction { get; set; }

        public int UserID { get; set; }

        public bool FlaggedAndAffected { get; set; }

        public bool OnlyAffected { get; set; }

    }
}
