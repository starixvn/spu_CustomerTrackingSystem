namespace Fanex.NAP.CTS.Service
{    
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;
    using System;    
    using System.Collections.Generic;

    public interface IProblemAccountMonitorService
    {
        DateTime GetMoveBetBalanceUpDate();

        IEnumerable<SportModel> GetSports();

        IEnumerable<LeagueModel> GetLeagues(DateTime eventDate, int sportId);

        ProblemAccountMonitorModel GetProblemAccountMonitor(DateTime eventDate, string leagueIds, string matchStatus);

        int CountTicketFraud(IEnumerable<SuspiciousTicketInfo> suspiciousTicketInfos, int matchId, int fraudCaseId);

        DetailMatchInfoModel GetDetailMatchInfo(int matchId, int fraudCaseId);

        IEnumerable<DetailTicketsModel> GetDetailTickets(int matchId, int fraudCaseId);

        IEnumerable<DetailTicketVoidingInRequestModel> GetTicketVoidingInRequest(int matchId, string tranIdList);

        string GetHtmlChoice(DetailTicketsModel ticket);

    }
}