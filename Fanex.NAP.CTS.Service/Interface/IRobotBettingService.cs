namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.Account.RobotBetting;
    using System;
    using System.Collections.Generic;

    public interface IRobotBettingService
    {
        RobotBettingModel GetRobotBetting(long custID);      

        IEnumerable<RobotUserDetailModel> GetRobotDetectionDetails(long custID, int periodRangeType);

        IEnumerable<RobotUserDetailSceModel> GetRobotDetectionSCEDetails(RobotBettingSceDetailParaModel parameters);

        IEnumerable<RobotBettingTicketDetailModel> GetRobotBettingTicketDetails(string tranIdList, bool isSCETicketDetails);

        IEnumerable<string> GetTranIds(long custID, int robotTypeOrigin, int? robotTypeDisplay, int? clusterId, string createdTime);

        string GetTransFromList(IEnumerable<string> suspiciousTransList);
    }
}
