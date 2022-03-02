namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Model.Account;
    using System;
    using System.Collections.Generic;

    public interface IRobotClassificationService
    {
        IEnumerable<RobotClassification> GetHistoryRobotClassification(long custID);

        CustomerRobotUserStatusModel GetStatusRobotClassification(long custID);
    }
}
