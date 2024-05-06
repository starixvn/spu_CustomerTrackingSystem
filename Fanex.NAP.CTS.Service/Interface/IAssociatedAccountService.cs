using Fanex.NAP.CTS.Core;
using Fanex.NAP.CTS.Model.Account;
using Fanex.NAP.CTS.Model.AssociatedAccountMonitor;
using Fanex.NAP.CTS.Model.DTO;
using System.Collections.Generic;

namespace Fanex.NAP.CTS.Service.Interface
{
    public interface IAssociatedAccountService
    {
        IEnumerable<ValidateAssociatedAccountModel> ValidateAssociatedAccount(string toUserNameList, out bool isException);

        DBResult InsertAssociatedAccountMultiple(int userID, string custIDList, string remark);

        IEnumerable<AssociatedAccountMonitorModel> GetAssociatedAccountMonitor();

        DBResult RemoveAssociatedAccount(long ctsCustId, int userID);

        IEnumerable<CustomerEvidenceForTW> GetFlaggedEvidenceByDirectAssForAPI(string custIdList);

        IEnumerable<CustomerEvidenceForTW> GetFlaggedEvidenceByLevel2AssForAPI(string custIdList);

        IEnumerable<AssociatedAccountModel> GetAssociationForExtAcc(int custId);
    }
}
