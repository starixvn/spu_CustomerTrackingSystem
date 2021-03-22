namespace Fanex.NAP.CTS.ViewModel.Account
{
    using System.Collections.Generic;

    public class AssociationByIPInfoViewModel
    {
        public AssociationByIPInfoViewModel()
        {
            ItemList = new List<AssociationByIPItemViewModel>();
        }

        public int TotalMatchCount { get; set; }

        public int TotalIPSharingCount { get; set; }

        public List<AssociationByIPItemViewModel> ItemList { get; set; }
    }
}
