namespace Fanex.NAP.CTS.ViewModel.AssociationDetection
{
    using System.Collections.Generic;
    using System.Linq;

    public class DetectAssociationDangerFilterViewModel
    {
        public DetectAssociationDangerFilterViewModel()
        {
            Danger1 = Enumerable.Empty<DetectAssociationFilterDangerViewModel>();
            Danger2 = Enumerable.Empty<DetectAssociationFilterDangerViewModel>();
            Danger3 = Enumerable.Empty<DetectAssociationFilterDangerViewModel>();
            Danger4 = Enumerable.Empty<DetectAssociationFilterDangerViewModel>();
            Danger5 = Enumerable.Empty<DetectAssociationFilterDangerViewModel>();
            NoneDanger = Enumerable.Empty<DetectAssociationFilterDangerViewModel>();
        }

        public IEnumerable<DetectAssociationFilterDangerViewModel> Danger1 { get; set; }

        public IEnumerable<DetectAssociationFilterDangerViewModel> Danger2 { get; set; }
        
        public IEnumerable<DetectAssociationFilterDangerViewModel> Danger3 { get; set; }
        
        public IEnumerable<DetectAssociationFilterDangerViewModel> Danger4 { get; set; }
        
        public IEnumerable<DetectAssociationFilterDangerViewModel> Danger5 { get; set; }

        public IEnumerable<DetectAssociationFilterDangerViewModel> NoneDanger { get; set; }
    }
}
