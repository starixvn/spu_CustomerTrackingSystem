namespace Fanex.NAP.CTS.Criteria.SuspiciousCustomer
{
    public class DataTableAjaxPostCriteria : SuspiciousCustomerReportGetCriteria
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
    }
}