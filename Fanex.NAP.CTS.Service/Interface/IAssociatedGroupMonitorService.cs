namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.AssociatedGroupMonitor;
    using Fanex.NAP.CTS.ViewModel.AssociatedGroupMonitor;  
    using System.Collections.Generic;  

    public interface IAssociatedGroupMonitorService
    {
        IEnumerable<AssociatedGroupMonitorModel> GetAssociatedGroupMonitor();

        IEnumerable<AssociatedAccountMonitorModel> GetAssociatedAccountMonitor(long groupID);

        DBResult AddAssociatedGroupMonitor(AddAssociatedGroupViewModel model);

        DBResult RemoveAssociatedGroupMonitor(RemoveAssociatedGroupViewModel model);

        DBResult AddAssociatedAccount(AddAssociatedAccountViewModel model);

        byte[] ExportExcelAssociatedAccount(IEnumerable<AssociatedAccountMonitorModel> associatedAccounts
            , string fileName
            , string templatePath
            , ExportToExcelAssociatedAccountViewModel model);

    }
}
