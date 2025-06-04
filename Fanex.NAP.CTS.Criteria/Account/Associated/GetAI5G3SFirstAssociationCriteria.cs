
namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    public class GetAI5G3SFirstAssociationCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustID1", AllowNull = true)]
        public long CustId1 { get; set; }

        [SpParam(SqlParamName = "ip_CustID2", AllowNull = true)]
        public long CustId2 { get; set; }

        [SpParam(SqlParamName = "ip_AssType", AllowNull = true)]
        public int AssType { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociationByAI_GetFirstAssociationDate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
