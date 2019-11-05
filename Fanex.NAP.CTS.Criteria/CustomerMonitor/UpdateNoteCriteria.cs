namespace Fanex.NAP.CTS.Criteria.CustomerMonitor
{   
    using Fanex.NAP.CTS.Core.BaseClasses;

    public class UpdateNoteCriteria : CommandBase
    {
        public int UserId { get; set; }

        public int CustId { get; set; }

        public string Notes { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Tbl_SuspiciousCustomerNotes_Update";
        }

        public override bool IsValid() => true;
    }
}