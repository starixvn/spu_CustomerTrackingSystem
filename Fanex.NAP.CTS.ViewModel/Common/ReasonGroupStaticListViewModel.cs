namespace Fanex.NAP.CTS.ViewModel.Common
{
    using System.Collections.Generic;

    public class ReasonGroupStaticListViewModel
    {
        public ReasonGroupStaticListViewModel()
        {
            this.ReasonItems = new List<ReasonStaticListViewModel>();
        }

        public string ReasonGroupName { get; set; }

        public IEnumerable<ReasonStaticListViewModel> ReasonItems { get; set; }

    }
}
