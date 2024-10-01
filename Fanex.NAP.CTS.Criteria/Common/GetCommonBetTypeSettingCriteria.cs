namespace Fanex.NAP.CTS.Criteria.Common
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCommonBetTypeSettingCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FunctionID")]
        public int? FunctionID { get; set; }

        [SpParam(SqlParamName = "ip_SportTypeIDList")]
        public string SportTypeIDList { get; set; }

        [SpParam(SqlParamName = "ip_BettypeIDList")]
        public string BettypeIDList { get; set; }

        [SpParam(SqlParamName = "ip_IsFilterMMDetection")]
        public bool? IsFilterMMDetection { get; set; }

        [SpParam(SqlParamName = "ip_IsFilterMMParlayDetection")]
        public bool? IsFilterMMParlayDetection { get; set; }

        [SpParam(SqlParamName = "ip_IsShowDetailsX")]
        public bool? IsShowDetailsX { get; set; }

        [SpParam(SqlParamName = "ip_Status")]
        public int Status { get; set; }

        public override string GetSettingKey()
                => "CTS_DC_Common_BettypeSetting_Get";

        public override bool IsValid()
            => true;
    }
}
