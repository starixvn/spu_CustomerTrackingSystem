namespace Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AddAssociatedGroupMonitorCriteria : NonQueryCommand
    {
        //[SpParam(SqlParamName = "CustIDList")]
        //public string CustIDList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedGroup_Create";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
