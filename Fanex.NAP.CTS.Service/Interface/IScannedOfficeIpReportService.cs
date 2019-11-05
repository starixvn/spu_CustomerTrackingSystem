namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Model.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.FraudCaseDetails;
    using System;
    using System.Collections.Generic;

    public interface IScannedOfficeIpReportService
    {
        IEnumerable<TicketDetails> GetTickets(DateTime fromDate, DateTime toDate, bool isShowSpecialCurrencies);

        IEnumerable<TicketInfo> GetRawTickets(DateTime fromDate, DateTime toDate, bool isShowSpecialCurrencies);
    }
}