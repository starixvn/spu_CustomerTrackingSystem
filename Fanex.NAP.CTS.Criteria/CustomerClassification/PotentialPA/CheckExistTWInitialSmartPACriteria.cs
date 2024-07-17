namespace Fanex.NAP.CTS.Criteria.CustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CheckExistTWInitialSmartPACriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIDs { get; set; }

        public override string GetSettingKey() => "CTS_DC_CustClassification_InitialSmart_CheckExist";

        public override bool IsValid() => true;
    }
}
