namespace Fanex.NAP.CTS.Library.Criteria.SuspiciousCustomer
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Library.Core.BaseClasses;

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