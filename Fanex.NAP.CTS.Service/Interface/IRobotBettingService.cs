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

        IEnumerable<RobotUserDetailSceModel> GetRobotDetectionSCEDetails(RobotBettingSceDetailParaModel parameters);

        IEnumerable<RobotBettingTicketDetailModel> GetRobotBettingTicketDetails(string tranIdList);

        IEnumerable<string> GetTranIds(long custID, int robotTypeOrigin, int? robotTypeDisplay, int? clusterId, string createdTime);

        string GetTransFromList(IEnumerable<string> suspiciousTransList);
    }
}
