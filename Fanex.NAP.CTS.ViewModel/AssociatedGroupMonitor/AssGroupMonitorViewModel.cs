namespace Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor
{
    using Fanex.NAP.CTS.Model.ProblemAccountManagement;
    using System.Collections.Generic;

    public class AssGroupMonitorViewModel
    {
        public bool IsHostPermission { get; set; }

        public IEnumerable<ProblemAccountCategoryModel> CategoryList { get; set; }
    }
}
