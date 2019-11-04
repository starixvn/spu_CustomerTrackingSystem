namespace Fanex.NAP.CTS.Library.Services.Interface
{
    using Fanex.NAP.CTS.Library.Criteria.CheckBetReport;
    using Fanex.NAP.CTS.Library.Models;
    using Fanex.NAP.CTS.Library.ViewModels.CheckBetReport;
    using System.Collections.Generic;

    public interface ICheckBetReportService
    {
        IEnumerable<CheckBetTicket> Generate(CheckBetReportGetCriteria criteria);

        DBResult UpdateNote(CheckBetReportUpdateNoteCriteria criteria);
    }
}