namespace Fanex.NAP.CTS.Service
{
    using Fanex.NAP.CTS.Criteria.MatchMonitor.FraudCaseDetails;    
    using Fanex.NAP.CTS.Model.DTO;
    using Fanex.NAP.CTS.Model.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.CTS.ViewModel.MatchMonitor.FraudCaseDetails;
    using System;
    using System.Collections.Generic;    
    public interface IFraudCaseDetailsService
    {
        IEnumerable<TicketDetails> GetTicketDetails(int matchId, int fraudCaseId, DateTime eventDate, bool isShowSpecialCurrencies, bool saveChoice = false);

        IEnumerable<CustomerDetails> GetCustomerDetails(int matchId, int fraudCaseId, DateTime eventDate, bool isShowSpecialCurrencies);

        MatchFraudCaseInfo GetInfo(int matchId, int fraudCaseId, DateTime eventDate);

        IEnumerable<TicketDetails> GetLast7DaysDetails(int custId, bool isShowSpecialCurrencies);

        IEnumerable<AssociatedCustomer> GetAssociatedAccounts(int custId);

        BettypeOrder GetOrder(string bettype);

        IEnumerable<TicketInfo> GetLast7DaysInfo(int custId, bool isShowSpecialCurrencies);

        IEnumerable<CustomerTotalAssociation> GetTotalAssociations(IEnumerable<int> custIds);

        IEnumerable<CustomerAssociation> GetAssociations(int custId);

        IEnumerable<TicketDetails> GetTicketFromIP(string ip, bool isShowSpecialCurrencies);
              
        IEnumerable<CustomerDetails> GetArbitrageCustomerDetails(GetArbitrageCustomerCriteria criteria);       

        IEnumerable<ArbitrageGroupTicketList> GetArbitrageTickets(GetArbitrageTicketCriteria criteria, bool isSummary);

        IEnumerable<Evidence> GetListEvidences();

        IEnumerable<Model.MatchMonitor.FraudCaseDetails.Currency> GetAllCurrency(bool isShowSpecialCurrencies);

        IEnumerable<ScoreHDP> GetScoreHDP(int matchId, bool isShowSpecialCurrencies);

        IEnumerable<Site> GetSites();

        string ReplaceEvidences(string evidences);

        IEnumerable<FlaggedAccountTicket> GetFlaggedAccountTickets(FlaggedAccountCriteria criteria);

        IEnumerable<CustomerDetails> GetFlaggedAccountCustomers(FlaggedAccountCriteria criteria);

        IEnumerable<BetTypes> GetBetTypes();
    }
}