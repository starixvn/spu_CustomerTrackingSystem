namespace Fanex.NAP.CTS.ViewModel.Account
{
    using System;
    using System.Collections.Generic;

    public class AssociationByIPInfoViewModel
    {
        public AssociationByIPInfoViewModel()
        {
            ItemList = new List<AssociationByIPItemViewModel>();            
        }

        public UInt32 TotalMatchCount { get; set; }

        public UInt32 TotalIPSharingCount { get; set; }

        public string ErrorMessage { get; set; }

        public int ErrorCode
        {
            get
            {
                return string.IsNullOrEmpty(ErrorMessage) ? 0 : -1;                
            }
        }

        public List<AssociationByIPItemViewModel> ItemList { get; set; }
    }
}
