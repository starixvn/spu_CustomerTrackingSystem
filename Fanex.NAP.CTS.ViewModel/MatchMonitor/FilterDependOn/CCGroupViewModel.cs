namespace Fanex.NAP.CTS.ViewModel.MatchMonitor.FilterDependOn
{
    using System.Collections.Generic;

    public class CCGroupViewModel
    {
        public CCGroupViewModel()
        {
            this.CCList = new List<int>();
        }

        public string CCGroupName { get; set; }

        public IEnumerable<int> CCList { get; set; }
    }
}
