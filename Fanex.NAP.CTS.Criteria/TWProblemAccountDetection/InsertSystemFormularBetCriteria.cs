namespace Fanex.NAP.CTS.Criteria.ProblemAccountMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertSystemFormulaBetCriteria : NonQueryCommand
    {        
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIdList { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_SystemFormulaBetMonitor_Insert";

        public override bool IsValid()
            => !string.IsNullOrEmpty(CustIdList);
    }
}