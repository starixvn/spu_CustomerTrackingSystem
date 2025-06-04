namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssociationProfileCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustID", AllowNull = true)]
        public long CTSCustId { get; set; }

        [SpParam(SqlParamName = "ip_RoleIDList", AllowNull = true)]
        public string RoleIDs { get; set; }

        [SpParam(SqlParamName = "ip_AssociationType", AllowNull = true)]
        public int AssociationType { get; set; }

        [SpParam(SqlParamName = "ip_AssociationStatus", AllowNull = true)]
        public int AssociationStatus { get; set; }

        [SpParam(SqlParamName = "ip_AccountStatusIDs", AllowNull = true)]
        public string AccountStatusIds { get; set; }

        [SpParam(SqlParamName = "ip_SubscriberIDs", AllowNull = true)]
        public string SubscriberIDs { get; set; }

        [SpParam(SqlParamName = "ip_CategoryIDs", AllowNull = true)]
        public string CategoryIDs { get; set; }

        [SpParam(SqlParamName = "ip_Skip", AllowNull = true)]
        public int Skip { get; set; }

        [SpParam(SqlParamName = "ip_Take", AllowNull = true)]
        public int Take { get; set; }

        [SpParam(SqlParamName = "op_TotalItems", AllowNull = true)]        
        public int TotalItems { get; set; }
       
        public override string GetSettingKey() => "CTS_DC_Association_Profile_Get";
        
        public override bool IsValid()
        {
            return true;
        }
    }
}
