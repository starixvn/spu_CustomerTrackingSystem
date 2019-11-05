namespace Fanex.NAP.CTS.Criteria.SuspiciousCustomerDetails
{
    using System.Configuration;

    public class SuspiciousCustomerDetailsViolatedTicketGetCriteria : SuspiciousCustomerDetailsGeneralCriteria
    {
        public int Environment => int.Parse(ConfigurationManager.AppSettings["Environment"]);

        public override string GetSettingKey() => "FS_Rpt_SuspicousCustomerDetails_ViolatedTickets_Get";
    }
}