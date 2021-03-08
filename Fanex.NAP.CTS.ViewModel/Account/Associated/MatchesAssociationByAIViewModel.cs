namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using System.Collections.Generic;

    public class MatchesAssociationByAIViewModel
    {
        public List<MatchesInfoViewModel> MatchesInfo { get; set; }

        public int TotalMatchCount { get; set; }

        public string ErrorMessage { get; set; }

    }
}
