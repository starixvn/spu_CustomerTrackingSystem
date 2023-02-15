namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using System.Collections.Generic;
    using System.Linq;

    public class MatchesAssociationByAIViewModel
    {
        public MatchesAssociationByAIViewModel(string originGroupIDs)
        {
            MatchesInfo = Enumerable.Empty<MatchesInfoViewModel>();
            IsExistGGB = !string.IsNullOrEmpty(originGroupIDs);
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

        public bool IsExistGGB { get; set; }

        public string ErrorMessage { get; set; }
    }
}
