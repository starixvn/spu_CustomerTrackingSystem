namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Criteria.DailyDataCapture;
    using Fanex.NAP.CTS.ViewModel.DailyDataMonitor;
    using System.Collections.Generic;

    public interface IDailyDataCaptureService
    {
        IEnumerable<CapturedMatch> GetMatches(DailyDataGetCriteria criteria);
    }
}