namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.Account.RobotBetting;
    using System;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public interface IRobotBettingService
    {
        IEnumerable<RobotBettingModel> GetRobotBetting(long custID, out RobotDateAndTypeModel robotDateAndType);

        IEnumerable<RobotUserDetailModel> GetRobotDetectionDetails(long custID, int periodRangeType);

        IEnumerable<RobotUserDetailSceModel> GetRobotDetectionSCEDetails(long custID, int periodRangeType, RobotDetectionQueryType queryType, DateTime? scannedTime = null);

        IEnumerable<RobotBettingTicketDetailModel> GetRobotBettingTicketDetails(string tranIdList);

        IEnumerable<string> GetTranIds(long custID, int periodRangeType, string createdTime);

        string GetTransFromList(IEnumerable<string> suspiciousTransList);
    }
}
