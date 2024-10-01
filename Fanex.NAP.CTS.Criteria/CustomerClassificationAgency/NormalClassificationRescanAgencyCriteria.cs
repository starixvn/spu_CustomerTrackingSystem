namespace Fanex.NAP.CTS.Criteria.CustomerClassificationAgency
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class NormalClassificationRescanAgencyCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ListCustId")]
        public string ListCustId { get; set; }

        [SpParam(SqlParamName = "FunctionId")]
        public int FunctionId { get; set; }

        [SpParam(SqlParamName = "RescanTypeId")]
        public int RescanTypeId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GeneralNormalClassificationAgency_RescanNormal_Preprocess"; // new
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
