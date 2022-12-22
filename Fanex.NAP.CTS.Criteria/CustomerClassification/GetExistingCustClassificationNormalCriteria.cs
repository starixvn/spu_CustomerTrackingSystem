namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetExistingCustClassificationNormalCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIDList { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustClassification_Normal_GetExisting";

        public override bool IsValid()
            => true;
    }
}
