namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertManualAssociationMultipleCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ManualAssociationList")]
        public string AssociationList { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_AddManualAssociationMultiple";

        public override bool IsValid()
            => true;

    }
}
