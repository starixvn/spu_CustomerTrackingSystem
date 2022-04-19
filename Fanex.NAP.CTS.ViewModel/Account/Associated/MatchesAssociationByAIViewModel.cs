namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using System.Collections.Generic;
    using System.Linq;

    public class MatchesAssociationByAIViewModel
    {
        public MatchesAssociationByAIViewModel()
        {
            MatchesInfo = Enumerable.Empty<MatchesInfoViewModel>();
        }

        public IEnumerable<MatchesInfoViewModel> MatchesInfo { get; set; }

        public int TotalMatchCount 
        { 
            get
            {
                if(MatchesInfo!=null && MatchesInfo.Any())
                {
                    return MatchesInfo.Count();
                }

                return 0;
            }
        }

        public string ErrorMessage { get; set; }
    }
}
