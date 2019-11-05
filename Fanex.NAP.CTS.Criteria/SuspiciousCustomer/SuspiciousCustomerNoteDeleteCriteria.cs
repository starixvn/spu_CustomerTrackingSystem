namespace Fanex.NAP.CTS.Criteria.SuspiciousCustomer
{
    using Fanex.NAP.CTS.Core.BaseClasses;

    public class SuspiciousCustomerNoteDeleteCriteria : CommandBase
    {
        public int CustomerLogId { get; set; }

        public int UserId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Tbl_CustomerLog_Delete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}