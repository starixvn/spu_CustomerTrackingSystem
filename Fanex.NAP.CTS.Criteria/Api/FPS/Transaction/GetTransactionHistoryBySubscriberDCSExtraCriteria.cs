namespace Fanex.NAP.CTS.Criteria.Api.FPS.Transaction
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetTransactionHistoryBySubscriberDCSExtraCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_LoginName")]
        public string UserName { get; set; }

        [SpParam(SqlParamName = "ip_DeviceCode")]
        public string DeviceCode { get; set; }

        [SpParam(SqlParamName = "ip_IP")]
        public string IP { get; set; }

        [SpParam(SqlParamName = "ip_Country")]
        public string Country { get; set; }

        [SpParam(SqlParamName = "ip_Region")]
        public string Region { get; set; }

        [SpParam(SqlParamName = "ip_City")]
        public string City { get; set; }

        [SpParam(SqlParamName = "ip_IsPartial")]
        public bool IsPartial { get; set; }

        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "ip_Skip")]
        public int Skip { get; set; }

        [SpParam(SqlParamName = "ip_Take")]
        public int Take { get; set; }

        [SpParam(SqlParamName = "op_TotalItem")]
        public int TotalItem { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_ET_Transaction_GetHistoryBySubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
