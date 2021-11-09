namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.Account.RobotBetting;
    using System;
    using System.Collections.Generic;

    public interface IRobotBettingService
    {
        IEnumerable<RobotBettingModel> GetRobotBetting(long custID);

        IEnumerable<RobotUserDetailModel> GetRobotUserDetails(long custID, int periodRangeType);
    }
}
