namespace Fanex.NAP.CTS.ViewModel.MatchMonitor
{
    using System.Collections.Generic;
    using System.Linq;

    public class DetectGroupAssociationListViewModel
    {
        public DetectGroupAssociationListViewModel()
        {
            this.ItemList = Enumerable.Empty<DetectGroupAssociationItemViewModel>();
            this.GroupList = Enumerable.Empty<GroupInfoViewModel>();
        }

        public IEnumerable<DetectGroupAssociationItemViewModel> ItemList { get; set; }

        public IEnumerable<GroupInfoViewModel> GroupList { get; set; }
        
    }
}
