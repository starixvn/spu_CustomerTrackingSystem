namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Model.Account;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface IRobotClassificationService
    {
        IEnumerable<RobotClassification> GetHistoryRobotClassification(long custID);

        CustomerRobotUserStatusModel GetStatusRobotClassification(long custID, ROLE_CUSTOMER RoleID);
        
    }
}
