namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using System;
    using System.Collections.Generic;

    public interface IMatchMonitorService
    {
        IEnumerable<MatchFromMainInfoModel> GetMatchFromMainInfos(MatchMonitorSearchViewModel filter, string matchIDs);

        IEnumerable<GetMatchMonitorModel> GetMatchMonitor(MatchMonitorSearchViewModel filter, string matchInfoFromMainJson);

        IEnumerable<MatchGroupModel> GetMatchGroup(Int64 matchID, int betTypeID, bool liveIndicator, Int64 betId, int reason);

        IEnumerable<MatchScoreModel> GetMatchScore(Int64 matchID, int betTypeID, bool liveIndicator, Int64 betId, int reason);

        DBResult VerifyTransMatchMonitorDetail(MatchMonitorDetailVerifyTransParaModel para);

        MMDetailResultModel GetMatchMonitorDetail(MMDetailInfoViewModel filter);

        IEnumerable<CustInfoModel> GetCustInfoByCustID(string custIDs);

        TicketDetailViewModel GetFraudTicketDetail(MMDetailInfoViewModel para, string transIds);

        TicketDetailViewModel GetAllMMTicketDetail(MMDetailInfoViewModel info, MMDetailFilterViewModel filter, bool isLoop = false);

        TicketDetailViewModel GetAllTicketDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter, int? batchSize, string transIds = null);

        int GetTotalTicketDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter, int? batchSize, string transIds);

        MMTicketDetailInfoModel GetMMTicketDetailInfo(MMDetailInfoViewModel para, IEnumerable<SuspiciousTransInfoModel> suspiciousTranInfos);

        IEnumerable<MMDetailMergeGroupModel> GetMMDetailMergeGroup(IEnumerable<MMDetailBetTeamModel> mmDetailBetTeams);

        IEnumerable<RobotStatusModel> GetRobotStatus(string transJson);

        MatchMonitorStaticListModel GetMMStaticList();

        bool IsDisplaySingleGrid(int betType);

        byte[] ExportExcelMMDetail(IEnumerable<ExExcelTicketInfoViewModel> homeMItems, IEnumerable<ExExcelTicketInfoViewModel> awayMItems
            , string fileName, string templatePath);
    }
}