namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Model.AssociatedGroupMonitor;
    using System;
    using System.Collections.Generic;  

    public interface IAssociatedGroupMonitorService
    {
        IEnumerable<AssociatedGroupMonitorModel> GetAssociatedGroupMonitor();
    }
}
