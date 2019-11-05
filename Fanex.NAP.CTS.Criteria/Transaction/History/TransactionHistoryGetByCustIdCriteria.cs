namespace Fanex.NAP.CTS.Criteria.Transaction.History
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Core.BaseClasses;

    public class TransactionHistoryGetByCustIdCriteria : CommandBase
    {
        public int ip_accountId { get; set; }

        public int ip_historyRange { get; set; }
   
        [SpParam(Ignored = true)]
        public override int ErrCode { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_DC_GetTransactionHistoryByCustId";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}