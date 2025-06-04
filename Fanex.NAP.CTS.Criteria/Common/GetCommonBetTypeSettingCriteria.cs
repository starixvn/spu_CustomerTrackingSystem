namespace Fanex.NAP.CTS.Criteria.Common
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCommonBetTypeSettingCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FunctionID", AllowNull = true)]
        public int? FunctionID { get; set; }

        [SpParam(SqlParamName = "ip_SportTypeIDList", AllowNull = true)]
        public string SportTypeIDList { get; set; }

        [SpParam(SqlParamName = "ip_BettypeIDList", AllowNull = true)]
        public string BettypeIDList { get; set; }

        [SpParam(SqlParamName = "ip_IsFilterMMDetection", AllowNull = true)]
        public bool? IsFilterMMDetection { get; set; }

        [SpParam(SqlParamName = "ip_IsFilterMMParlayDetection", AllowNull = true)]
        public bool? IsFilterMMParlayDetection { get; set; }

        [SpParam(SqlParamName = "ip_IsShowDetailsX", AllowNull = true)]
        public bool? IsShowDetailsX { get; set; }

        [SpParam(SqlParamName = "ip_Status", AllowNull = true)]
        public int Status { get; set; }

        public override string GetSettingKey()
                => "CTS_DC_Common_BettypeSetting_Get";

        public override bool IsValid()
            => true;
    }
}
