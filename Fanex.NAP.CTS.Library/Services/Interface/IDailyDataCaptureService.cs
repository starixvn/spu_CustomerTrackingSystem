namespace Fanex.NAP.CTS.Library.Services.Interface
{
    using Fanex.NAP.CTS.Library.Criteria.DailyDataCapture;
    using Fanex.NAP.CTS.Library.ViewModels.DailyDataMonitor;
    using System.Collections.Generic;

    public interface IDailyDataCaptureService
    {
        IEnumerable<CapturedMatch> GetMatches(DailyDataGetCriteria criteria);
    }
}