namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.AssociatedGroupMonitor;
    using Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor;
    using System;
    using System.Collections.Generic;  

    public interface IAssociatedGroupMonitorService
    {
        IEnumerable<AssociatedGroupMonitorModel> GetAssociatedGroupMonitor();

        DBResult AddAssociatedGroupMonitor(AddAssociatedGroupViewModel model);

        DBResult RemoveAssociatedGroupMonitor(RemoveAssociatedGroupViewModel model);

        DBResult AddAssociatedAccount(AddAssociatedAccountViewModel model);

    }
}
