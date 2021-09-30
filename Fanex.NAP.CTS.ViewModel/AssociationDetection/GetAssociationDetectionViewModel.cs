namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{
    using System.Collections.Generic;

    public class GetAssociationDetectionViewModel
    {
        public GetAssociationDetectionViewModel()
        {
            ItemList = new List<AssociationDetectionItemViewModel>();
            ErrorCode = 0;
            ErrorMessage = string.Empty;
        }

        public IEnumerable<AssociationDetectionItemViewModel> ItemList { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
