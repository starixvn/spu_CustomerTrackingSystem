namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.TicketHelper;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Criteria.ScannedOfficeIP;
    using Fanex.NAP.CTS.Model.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.FraudCaseDetails;  
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ScannedOfficeIpReport : IScannedOfficeIpReportService
    {
        public IEnumerable<TicketDetails> GetTickets(DateTime fromDate, DateTime toDate, bool isShowSpecialCurrencies)
        {
            var html_builder = new BetListHTMLBuilder();
            var ticketHelper = new TicketHelper();

            var null_winloss = new HashSet<string>
            {
                "running",
                "reject"
            };

            return GetRawTickets(fromDate, toDate, isShowSpecialCurrencies)
                .Select(a =>
                {
                    var bettype = CachedRef.GetBettypeName(a.BetTypeId);

                    return new TicketDetails
                    {
                        Member = a.UserName,
                        TransDate = a.TransDate.ToString("MMM dd, yyyy hh:mm:ss tt"),
                        BetID = a.TransId.ToString(),
                        IP = a.IP,
                        Odds = new OddsDetails { Value = (double)a.Odds, Type = CachedRef.GetOddsTypeName(a.OddsType), Html = html_builder.BuildOdds(a, null, ticketHelper.GetOddsTypeNameById) },
                        Score = string.Format("{0}-{1}", a.LiveHomeScore, a.LiveAwayScore),
                        Stake = a.Stake,
                        Status = a.Status,
                        WinLoss = null_winloss.Contains(a.Status.ToLowerInvariant()) ? null : new decimal?(a.Winlost),
                        Choice = html_builder.BuildChoice(a, ticketHelper, null, false),
                        ChoiceExcel = null,
                        BettypeRealName = bettype,
                        CurrencyName = a.CurrencyName,
                        RawData = null
                    };
                });
        }

        public IEnumerable<TicketInfo> GetRawTickets(DateTime fromDate, DateTime toDate, bool isShowSpecialCurrencies)
        {
            var criteria = new TicketFromOfficeIpGetCriteria
            {
                FromDate = fromDate,
                ToDate = toDate,
                IsShowSpecialCurrencies = isShowSpecialCurrencies
            };

            return DataService.GetData<TicketInfo>(criteria)
                .OrderBy(t => t.UserName)
                .ThenBy(t => t.TransDate);
        }
    }
}