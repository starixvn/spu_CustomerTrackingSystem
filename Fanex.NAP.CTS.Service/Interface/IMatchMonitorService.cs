namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using System;
    using System.Collections.Generic;

    public interface IMatchMonitorService
    {
        List<MatchMonitorModel> GetMatchMonitor(MatchMonitorFilterViewModel filter);

        List<MatchReasonGroupModel> GetMatchReasonGroup(Int64 matchID, int betTypeID, bool liveIndicator, bool isHedging);

        List<MatchScoreModel> GetMatchScore(Int64 matchID, int betTypeID, bool liveIndicator, bool isHedging);

        DBResult UpdateVerificaton(Int64 matchID, Int64 userID);

        List<MatchMonitorDetailModel> GetMatchMonitorDetail(MatchMonitorDetailFilterViewModel filter);

        List<CustInfoModel> GetCustInfoByCustID(string custIDs);

        List<TicketDetailModel> GetTicketDetail(DateTime eventDate, string transIds);
    }
}