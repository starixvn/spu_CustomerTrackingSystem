namespace Fanex.NAP.CTS.Service
{    
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;
    using System;    
    using System.Collections.Generic;

    public interface IProblemAccountMonitorService
    {
        DateTime GetMoveBetBalanceUpDate();

        IEnumerable<SportModel> GetSports();

        IEnumerable<LeagueModel> GetLeagues(DateTime eventDate, string sportIds);

        IEnumerable<SubscriberGroupModel> GetAllSubscriberGroups();

        ProblemAccountMonitorModel GetProblemAccountMonitor(DateTime eventDate, string leagueIds, string matchStatus, string siteIds);

        int CountTicketFraud(IEnumerable<SuspiciousTicketInfo> suspiciousTicketInfos, int matchId, int fraudCaseId);

        DetailMatchInfoModel GetDetailMatchInfo(int matchId, int fraudCaseId);

        IEnumerable<DetailTicketsModel> GetDetailTickets(int matchId, int fraudCaseId, string stakeFrom, string stakeTo, string siteIds);

        IEnumerable<DetailTicketVoidingInRequestModel> GetTicketVoidingInRequest(int matchId, string tranIdList);

        string GetHtmlChoice(DetailTicketsModel ticket);

        string GetVoidRequestId(IEnumerable<DetailTicketVoidingInRequestModel> ticketList, long tranId);

    }
}