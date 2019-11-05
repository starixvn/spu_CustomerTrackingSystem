namespace Fanex.NAP.CTS.Service.Interface
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Criteria.CustomerMonitor;
    using Fanex.NAP.CTS.Criteria.CustomerMonitor.CustomerDetails;
    using Fanex.NAP.CTS.Model.CustomerMonitor;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.ViewModel.CustomerMonitor.CustomerDetails;
    using System.Collections.Generic;    

    public interface ICustomerMonitorService
    {
        BalanceUpDate GetBalanceUpDate();

        IEnumerable<Customer> GetCustomers(CustomerGetCriteria criteria);

        DBResult UpdateNote(UpdateNoteCriteria criteria);

        IEnumerable<TicketDetails> GetTicketDetails(GetCustomerTicketsCriteria criteria, bool saveChoice = false);

        byte[] ExportExcel(GetCustomerTicketsCriteria criteria, CustomerDetailsInfo detailsInfo, string templatePath);
    }
}