using Fanex.NAP.CTS.Core;
using Fanex.NAP.CTS.Model.AssociatedAccountMonitor;
using Fanex.NAP.CTS.ViewModel;
using System.Collections.Generic;

namespace Fanex.NAP.CTS.Service.Interface
{
    public interface IAssociatedAccountService
    {
        IEnumerable<ValidateAssociatedAccountModel> ValidateAssociatedAccount(string toUserNameList, out bool isException);

        DBResult InsertAssociatedAccountMultiple(int userID, string custIDList, string remark);

        IEnumerable<AssociatedAccountMonitorModel> GetAssociatedAccountMonitor();
    }
}
