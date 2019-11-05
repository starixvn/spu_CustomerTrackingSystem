namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Criteria.CheckBetReport;
    using Fanex.NAP.CTS.ViewModel.CheckBetReport;
    using System.Collections.Generic;

    public interface ICheckBetReportService
    {
        IEnumerable<CheckBetTicket> Generate(CheckBetReportGetCriteria criteria);

        DBResult UpdateNote(CheckBetReportUpdateNoteCriteria criteria);
    }
}