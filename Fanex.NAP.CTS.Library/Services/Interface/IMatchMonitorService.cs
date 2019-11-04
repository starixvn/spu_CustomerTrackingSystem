namespace Fanex.NAP.CTS.Library.Service
{
    using Fanex.NAP.CTS.Library.Criteria.MatchMonitor;
    using Fanex.NAP.CTS.Library.Models;
    using Fanex.NAP.CTS.Library.ViewModels.MatchMonitor;
    using System.Collections.Generic;

    public interface IMatchMonitorService
    {
        MatchList GetMatches(MatchGetAllCriteria criteria);

        IEnumerable<Library.Models.MatchMonitor.League> GetLeagues(LeagueGetCriteria criteria);

        object VerifyMatch(VerifyMatchCriteria criteria);

        Library.Models.MatchMonitor.BalanceUpDate GetBalanceUpDate();

        byte[] ExportExcel(MatchGetAllCriteria criteria, string template);

        DBResult UpdateMatchNote(MatchNoteUpdateCriteria criteria);

        Match GetMatch(Library.Models.MatchMonitor.Match match, IEnumerable<Library.Models.MatchMonitor.FraudCase> fraudCases);
    }
}