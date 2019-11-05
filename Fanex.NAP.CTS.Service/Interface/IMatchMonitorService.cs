namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Criteria.MatchMonitor;   
    using Fanex.NAP.CTS.ViewModel.MatchMonitor;
    using System.Collections.Generic;

    public interface IMatchMonitorService
    {
        MatchList GetMatches(MatchGetAllCriteria criteria);

        IEnumerable<Model.MatchMonitor.League> GetLeagues(LeagueGetCriteria criteria);

        object VerifyMatch(VerifyMatchCriteria criteria);

        Model.MatchMonitor.BalanceUpDate GetBalanceUpDate();

        byte[] ExportExcel(MatchGetAllCriteria criteria, string template);

        DBResult UpdateMatchNote(MatchNoteUpdateCriteria criteria);

        Match GetMatch(Model.MatchMonitor.Match match, IEnumerable<Model.MatchMonitor.FraudCase> fraudCases);
    }
}