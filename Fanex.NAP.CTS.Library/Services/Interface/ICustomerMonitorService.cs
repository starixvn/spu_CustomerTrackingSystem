namespace Fanex.NAP.CTS.Library.Services.Interface
{
    using Fanex.NAP.CTS.Library.Criteria.CustomerMonitor;
    using Fanex.NAP.CTS.Library.Criteria.CustomerMonitor.CustomerDetails;
    using Fanex.NAP.CTS.Library.Models;
    using Fanex.NAP.CTS.Library.Models.CustomerMonitor;
    using Fanex.NAP.CTS.Library.ViewModels.CustomerMonitor.CustomerDetails;
    using System.Collections.Generic;
    using Library.Models.MatchMonitor;

    public interface ICustomerMonitorService
    {
        BalanceUpDate GetBalanceUpDate();

        IEnumerable<Customer> GetCustomers(CustomerGetCriteria criteria);

        DBResult UpdateNote(UpdateNoteCriteria criteria);

        IEnumerable<TicketDetails> GetTicketDetails(GetCustomerTicketsCriteria criteria, bool saveChoice = false);

        byte[] ExportExcel(GetCustomerTicketsCriteria criteria, CustomerDetailsInfo detailsInfo, string templatePath);
    }
}