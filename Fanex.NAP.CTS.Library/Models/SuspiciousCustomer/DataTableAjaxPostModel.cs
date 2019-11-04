using Fanex.NAP.CTS.Library.Criteria.SuspiciousCustomer;

namespace Fanex.NAP.CTS.Library.Models.SuspiciousCustomer
{
    public class DataTableAjaxPostModel : SuspiciousCustomerReportGetCriteria
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        //public SuspiciousCustomerReportGetCriteria Search { get; set; }
    }
}
