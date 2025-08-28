namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Model.DangerMonitor;
    using Fanex.NAP.CTS.ViewModel.DangerMonitor;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using System;
    using System.Collections.Generic;

    public interface IDangerMonitorService
    {
        IEnumerable<DangerMonitorSporTypeBetTypeViewModel> GetSportBettypesFromCache(bool isGetNew = false);

        IEnumerable<DangerMonitorSporTypeBetTypeViewModel> GetBettypesFromCache();

        IEnumerable<DangerMonitorLeagueGroupViewModel> GetLeagueGroups();

        DangerMonitorValidateUsernameViewModel ValidateUsernameForMain(string usernames);

        GetDangerMonitorResultViewModel GetDangerMonitor(DangerMonitorSearchModel filter);

        byte[] ExportExcelMMDetailSG(IEnumerable<ExExcelTicketInfoViewModel> items, string fileName, string templatePath, int sportType);

        byte[] ExportExcelMMDetailDG(IEnumerable<ExExcelTicketInfoViewModel> homeMItems, IEnumerable<ExExcelTicketInfoViewModel> awayMItems
            , string fileName, string templatePath, int sportType);

        MMDetailTicketListViewModel GetMatchMonitorDetail(MMDetailInfoViewModel para, MMDetailFilterViewModel filter);

        MMDetailFilterViewModel GetMatchMonitorFilter(MMDetailInfoViewModel para);
    }
}
