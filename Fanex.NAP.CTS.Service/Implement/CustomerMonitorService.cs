namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.BetList.Core.Builder;
    using Fanex.BetList.Core.TicketHelper;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.ExportExcel;
    using Fanex.NAP.CTS.Criteria.CustomerMonitor;
    using Fanex.NAP.CTS.Criteria.CustomerMonitor.CustomerDetails;
    using Fanex.NAP.CTS.Model.CustomerMonitor;
    using Fanex.NAP.CTS.Model.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.CustomerMonitor.CustomerDetails;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.Criteria.MatchMonitor;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Service.ExcelConfig.CustomerMonitor;

    public class CustomerMonitorService : ICustomerMonitorService
    {
        public BalanceUpDate GetBalanceUpDate()
        {
            return DataService.GetData<Model.MatchMonitor.BalanceUpDate>(new BalanceUpDateGetCriteria()).First();
        }

        public IEnumerable<Customer> GetCustomers(CustomerGetCriteria criteria)
        {
            return DataService.GetData<Customer>(criteria);
        }

        public DBResult UpdateNote(UpdateNoteCriteria criteria)
        {
            return DataService.UpdateData(criteria, "CustomerMonitor");
        }

        public IEnumerable<TicketDetails> GetTicketDetails(GetCustomerTicketsCriteria criteria, bool saveChoice = false)
        {
            var html_builder = new BetListHTMLBuilder();
            var ticketHelper = new TicketHelper();

            var null_winloss = new HashSet<string>
            {
                "running",
                "reject"
            };

            return DataService.GetData<TicketInfo>(criteria)
                .Select(a =>
                {
                    return new TicketDetails
                    {
                        MatchString = string.Format("{0} -vs- {1}", CachedRef.GetTeamName(int.Parse(a.HomeId.ToString())), CachedRef.GetTeamName(int.Parse(a.AwayId.ToString()))),
                        TransDate = a.TransDate.ToString("MMM dd, yyyy hh:mm:ss tt"),
                        BetID = a.TransId.ToString(),
                        BettypeRealName = CachedRef.GetBettypeName(a.BetTypeId),
                        IP = a.IP,
                        Odds = new ViewModel.MatchMonitor.FraudCaseDetails.OddsDetails { Value = (double)a.Odds, Type = CachedRef.GetOddsTypeName(int.Parse(a.OddsType)), Html = html_builder.BuildOdds(a, null, ticketHelper.GetOddsTypeNameById) },
                        Score = string.Format("{0}-{1}", a.LiveHomeScore, a.LiveAwayScore),
                        Stake = a.Stake,
                        Status = a.Status,
                        WinLoss = null_winloss.Contains(a.Status.ToLowerInvariant()) ? null : new decimal?(a.Winlost),
                        Choice = html_builder.BuildChoice(a, ticketHelper, null, false),
                        ChoiceExcel = saveChoice ? html_builder.BuildChoiceBlock(a, ticketHelper, null, false) : null,
                        RawData = saveChoice ? a : null,
                        League = CachedRef.GetLeagueName(a.LeagueId),
                        LeagueMatch = string.Format("{0} (<div class='text-blue'> {1} </div>)", string.Format("{0} -vs- {1}", CachedRef.GetTeamName(int.Parse(a.HomeId.ToString())), CachedRef.GetTeamName(int.Parse(a.AwayId.ToString()))), CachedRef.GetLeagueName(a.LeagueId))
                    };
                }).OrderBy(t => t.MatchString)
                .ThenBy(t => DateTime.Parse(t.TransDate));
        }

        public byte[] ExportExcel(GetCustomerTicketsCriteria criteria, CustomerDetailsInfo detailsInfo, string templatePath)
        {
            var tickets = GetTicketDetails(criteria, true);

            var wb =
                ExcelUtilities.GetWorkbook(templatePath)
                    .RenderSheet(new CustomerMonitorTicketDetailConfig(tickets.Count()), detailsInfo, tickets, null);

            return wb.ToBytes();
        }
    }
}