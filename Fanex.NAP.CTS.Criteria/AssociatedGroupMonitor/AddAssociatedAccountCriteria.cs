namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AddAssociatedAccountCriteria : NonQueryCommand
    {
        //[SpParam(SqlParamName = "CustIDList")]
        //public string CustIDList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedGroup_Account_Add";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
