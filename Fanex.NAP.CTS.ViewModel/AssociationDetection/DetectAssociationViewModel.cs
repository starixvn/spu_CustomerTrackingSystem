namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{  
    using System.Collections.Generic;

    public class DetectAssociationViewModel
    {
        public DetectAssociationViewModel()
        {
            ItemList = new List<DetectAssociationItemViewModel>();
            ErrorCode = 0;
            ErrorMessage = string.Empty;
        }

        public IEnumerable<DetectAssociationItemViewModel> ItemList { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
