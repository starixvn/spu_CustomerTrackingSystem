namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using System.Collections.Generic;

    public class MatchesAssociationByAIViewModel
    {
        public MatchesAssociationByAIViewModel()
        {
            MatchesInfo = new List<MatchesInfoViewModel>();
        }

        public List<MatchesInfoViewModel> MatchesInfo { get; set; }

        public int TotalMatchCount { get; set; }

        public string ErrorMessage { get; set; }

    }
}
