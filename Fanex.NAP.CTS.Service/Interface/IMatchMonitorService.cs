namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using System;
    using System.Collections.Generic;

    public interface IMatchMonitorService
    {
        IEnumerable<GetMatchMonitorModel> GetMatchMonitor(MatchMonitorFilterViewModel filter);

        IEnumerable<MatchGroupModel> GetMatchGroup(Int64 matchID, int betTypeID, bool liveIndicator, Int64 betId, int reason);

        IEnumerable<MatchScoreModel> GetMatchScore(Int64 matchID, int betTypeID, bool liveIndicator, Int64 betId, int reason);

        DBResult VerifyTransMatchMonitorDetail(MatchMonitorDetailVerifyTransParaModel para);

        MatchMonitorDetailResultModel GetMatchMonitorDetail(MatchMonitorDetailFilterViewModel filter);

        IEnumerable<CustInfoModel> GetCustInfoByCustID(string custIDs);

        IEnumerable<TicketDetailModel> GetTicketDetail(DateTime eventDate, string transIds);

        IEnumerable<RobotStatusModel> GetRobotStatus(string transJson);
     
        MatchMonitorStaticListModel GetMMStaticList();

        byte[] ExportExcelMMDetail(IEnumerable<ExExcelTicketInfoViewModel> homeMItems, IEnumerable<ExExcelTicketInfoViewModel> awayMItems
            , string fileName, string templatePath);
    }
}