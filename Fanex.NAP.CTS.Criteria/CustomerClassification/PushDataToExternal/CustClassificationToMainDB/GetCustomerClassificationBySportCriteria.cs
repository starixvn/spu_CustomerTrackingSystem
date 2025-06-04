namespace Fanex.NAP.CTS.Criteria.CustomerClassification.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerClassificationBySportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_Custs", AllowNull = true)]
        public string CustInfos { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustClassification_BySport_GetCC";

        public override bool IsValid()
            => true;
    }
}
