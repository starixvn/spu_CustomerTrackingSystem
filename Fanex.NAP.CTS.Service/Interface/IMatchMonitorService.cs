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

        string GetTransFilterSuspicousMGr(MMDetailFilterViewModel filter, TicketDetailViewModel fraudTicket);

        TicketDetailViewModel GetTicketDetailFraud(MMDetailResultModel matchDetailInfo, MMDetailInfoViewModel para);

        TicketDetailViewModel GetTicketDetailByFraudTranIDs(MMDetailInfoViewModel para, MMDetailFilterViewModel filter, string transIds);

        TicketDetailViewModel GetTicketDetailAll(MMDetailInfoViewModel info, MMDetailFilterViewModel filter, string transJson);

        TicketDetailViewModel GetMainTicketDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter, int? batchSize, string transIds = null);

        int GetTotalTicketDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter, int? batchSize, string transIds);

        IEnumerable<MatchMonitorDetailItemViewModel> MergeFraudToAllMMDetail(IEnumerable<MatchMonitorDetailItemViewModel> mdItemList, IEnumerable<MatchMonitorDetailItemViewModel> fraudMDItemList);

        IEnumerable<MatchMonitorDetailItemViewModel> MergeFraudReasonAGroup(TicketDetailViewModel fraudTicket);

        IEnumerable<MatchMonitorDetailItemViewModel> DrawColorFraudMMDetailWithMergeReasonAGroup(IEnumerable<MatchMonitorDetailVerifyTransResultModel> verifiedTrans,
            IEnumerable<CustInfoModel> custInfo,
            IEnumerable<TicketDetailModel> ticket,
            MMDetailMergeGroupViewModel mergeGroup,
            IEnumerable<MatchMonitorReasonModel> reasons);

        string GetReasonName(IEnumerable<MatchMonitorReasonModel> reasonList, string reasonNameList);

        MMDetailFilterViewModel GetFraudFilterInfoDetail(MMDetailInfoViewModel para, TicketDetailViewModel fraudTicket);

        MMDetailFilterViewModel GetAllFilterInfoDetail(MMDetailInfoViewModel para);

        MMTicketDetailInfoModel GetMMTicketDetailInfo(MMDetailInfoViewModel para, IEnumerable<SuspiciousTransInfoModel> suspiciousTranInfos);

        IEnumerable<MMDetailMergeGroupModel> GetMMDetailMergeGroup(IEnumerable<MMDetailBetTeamModel> mmDetailBetTeams);

        IEnumerable<RobotStatusModel> GetRobotStatus(string transJson);

        MatchMonitorStaticListModel GetMMStaticList();

        bool IsDisplaySingleGrid(int betType);

        byte[] ExportExcelMMDetailSG(IEnumerable<ExExcelTicketInfoViewModel> items, string fileName, string templatePath);

        byte[] ExportExcelMMDetailDG(IEnumerable<ExExcelTicketInfoViewModel> homeMItems, IEnumerable<ExExcelTicketInfoViewModel> awayMItems
            , string fileName, string templatePath);       
    }
}