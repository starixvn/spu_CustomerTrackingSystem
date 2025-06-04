namespace Fanex.NAP.CTS.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertManualAssociationMultipleCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ManualAssociationList", AllowNull = true)]
        public string AssociationListJson { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_Association_InsertManualMultiple";

        public override bool IsValid()
            => true;

    }
}
