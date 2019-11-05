namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Criteria.DailyDataCapture;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.DailyDataMonitor;    
    using System.Collections.Generic;
    using System.Linq;

    public class DailyCaptureDataService : IDailyDataCaptureService
    {
        public IEnumerable<CapturedMatch> GetMatches(DailyDataGetCriteria criteria)
        {
            return DataService.GetData<Model.DailyDataCapture.CapturedMatch>(criteria)
                .Select(m => new CapturedMatch
                {
                    MatchId = m.MatchId,
                    MatchCode = m.MatchCode,
                    League = CachedRef.GetLeagueName(m.LeagueId),
                    Home = CachedRef.GetTeamName(m.HomeId),
                    Away = CachedRef.GetTeamName(m.AwayId),
                    AgentTO = new ViewModel.History<double?>
                    {
                        New = m.NewTotalAgentTO,
                        Old = m.OldTotalAgentTO
                    },
                    CancelledTickets = new ViewModel.History<int?>
                    {
                        New = m.NewTotalCancelledTickets,
                        Old = m.OldTotalCancelledTickets
                    },
                    CompanyTO = new ViewModel.History<double?>
                    {
                        New = m.NewTotalCompanyTO,
                        Old = m.OldTotalCompanyTO
                    },
                    EventDate = m.EventDate.ToString("MMM dd, yyyy HH:mm:ss"),
                    MasterTO = new ViewModel.History<double?>
                    {
                        New = m.NewTotalMasterTO,
                        Old = m.OldTotalMasterTO
                    },
                    ScannedDate = m.CapturedDate.ToString("MMM dd, yyyy HH:mm:ss"),
                    SettledTickets = new ViewModel.History<int?>
                    {
                        New = m.NewTotalSettledTickets,
                        Old = m.OldTotalSettledTickets
                    },
                    SuperTO = new ViewModel.History<double?>
                    {
                        New = m.NewTotalSuperTO,
                        Old = m.OldTotalSuperTO
                    },
                    Tickets = new ViewModel.History<int?>
                    {
                        New = m.NewTotalTickets,
                        Old = m.OldTotalTickets
                    }
                });
        }
    }
}