namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Model.ProblemMatchMonitor;
    using System;    
    using System.Collections.Generic;

    public interface IProblemMatchMonitorService
    {
        DateTime GetMoveBetBalanceUpDate();

        IEnumerable<SportModel> GetSports();

        IEnumerable<LeagueModel> GetLeagues(DateTime eventDate, int sportId);

        ProblemMatchMonitorModel GetProblemMatchMonitor(DateTime eventDate, string leagueIds, string matchStatus);

        int CountTicketFraud(IEnumerable<SuspiciousTicketInfo> suspiciousTicketInfos, int matchId, int fraudCaseId);


    }
}