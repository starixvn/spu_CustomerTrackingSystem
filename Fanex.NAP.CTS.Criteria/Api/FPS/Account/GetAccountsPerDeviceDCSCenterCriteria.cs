using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.NAP.CTS.Criteria.Api.FPS.Account
{
    public class GetAccountsPerDeviceDCSCenterCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_LastDay")]
        public long TimeRange { get; set; }

        [SpParam(SqlParamName = "ip_NoOfAccount")]
        public int NoOfAccount { get; set; }

        [SpParam(SqlParamName = "ip_PageSize")]
        public int PageSize { get; set; }

        [SpParam(SqlParamName = "ip_PageIndex")]
        public int PageIndex { get; set; }
        
        [SpParam(SqlParamName = "op_TotalPages")]
        public int TotalPages { get; set; }
        
        [SpParam(SqlParamName = "op_TotalRows")]
        public int TotalRows { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Association_GetAccountsPerDevice";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
