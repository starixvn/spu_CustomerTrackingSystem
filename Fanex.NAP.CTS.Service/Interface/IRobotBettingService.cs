namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.Account.RobotBetting;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using System;
    using System.Collections.Generic;

    public interface IRobotBettingService
    {
        IEnumerable<RobotBettingModel> GetRobotBetting(long custID, out RobotDateModel robotDate);

        IEnumerable<RobotUserDetailModel> GetRobotUserDetails(long custID, int periodRangeType);

        IEnumerable<RobotBettingTicketDetailModel> GetRobotBettingTicketDetails(string tranIdList);
    }
}
