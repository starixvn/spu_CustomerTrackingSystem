namespace Fanex.NAP.CTS.Criteria.MonitorFunction
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class StatsAccessmentInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_SystemId", AllowNull = true)]
        public long SystemId { get; set; }

        [SpParam(SqlParamName = "ip_FunctionId", AllowNull = true)]
        public int FunctionId { get; set; }

        [SpParam(SqlParamName = "ip_UserId", AllowNull = true)]
        public int UserId { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage", AllowNull = true)]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "SPU_Stats_Accessment_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
