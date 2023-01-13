

namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    using System.Collections.Generic;
    using System.Linq;

    public class StakeIpPatternByAIViewModel
    {
        public StakeIpPatternByAIViewModel()
        {
            StakeIpPatternInformations = Enumerable.Empty<StakeIpPatternInfoViewModel>();
        }

        public IEnumerable<StakeIpPatternInfoViewModel> StakeIpPatternInformations { get; set; }

        public string ErrorMessage { get; set; }
    }
}
