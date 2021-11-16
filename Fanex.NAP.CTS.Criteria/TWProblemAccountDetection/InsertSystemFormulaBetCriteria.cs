namespace Fanex.NAP.CTS.Criteria.TWProblemAccountDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertSystemFormulaBetCriteria : NonQueryCommand
    {        
        [SpParam(SqlParamName = "ip_CustDetectInfo")]
        public string CustDetectInfo { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_TWProblemDetectionMonitor_Insert";

        public override bool IsValid()
            => !string.IsNullOrEmpty(CustDetectInfo);
    }
}