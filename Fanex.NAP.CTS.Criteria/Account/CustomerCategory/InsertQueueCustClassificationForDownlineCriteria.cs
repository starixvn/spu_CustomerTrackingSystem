namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategory
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertQueueCustClassificationForDownlineCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustID")]
        public long CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_CustID")]
        public long CustId { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_CategoryID")]
        public int CategoryId { get; set; }

        [SpParam(SqlParamName = "ip_Remark")]
        public string Remark { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy")]
        public int CreatedBy { get; set; }

        [SpParam(SqlParamName = "ip_RoleID")]
        public int RoleId { get; set; }

        [SpParam(SqlParamName = "ip_ActionType")]
        public int ActionType { get; set; }       

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_VVIPQueue_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
