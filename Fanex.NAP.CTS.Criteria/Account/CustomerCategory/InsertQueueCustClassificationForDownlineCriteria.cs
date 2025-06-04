namespace Fanex.NAP.CTS.Criteria.Account.CustomerCategory
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertQueueCustClassificationForDownlineCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CTSCustID", AllowNull = true)]
        public long CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_CustID", AllowNull = true)]
        public long CustId { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_CategoryID", AllowNull = true)]
        public int CategoryId { get; set; }

        [SpParam(SqlParamName = "ip_Remark", AllowNull = true)]
        public string Remark { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy", AllowNull = true)]
        public int CreatedBy { get; set; }

        [SpParam(SqlParamName = "ip_RoleID", AllowNull = true)]
        public int RoleId { get; set; }

        [SpParam(SqlParamName = "ip_ActionType", AllowNull = true)]
        public int ActionType { get; set; }       

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Queue_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
