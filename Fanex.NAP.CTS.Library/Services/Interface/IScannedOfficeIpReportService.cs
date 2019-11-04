namespace Fanex.NAP.CTS.Library.Services.Interface
{
    using Fanex.NAP.CTS.Library.Models.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.CTS.Library.ViewModels.MatchMonitor.FraudCaseDetails;
    using System;
    using System.Collections.Generic;

    public interface IScannedOfficeIpReportService
    {
        IEnumerable<TicketDetails> GetTickets(DateTime fromDate, DateTime toDate, bool isShowSpecialCurrencies);

        IEnumerable<TicketInfo> GetRawTickets(DateTime fromDate, DateTime toDate, bool isShowSpecialCurrencies);
    }
}