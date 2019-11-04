namespace Fanex.NAP.CTS.Library.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.NAP.CTS.Library.Core.BaseClasses;

    public class GetCustomerFraudInfoCriteria : CommandBase
    {
        public string custIds { get; set; }

        [SpParam(Ignored = true)]
        public override int ErrCode { get; set; }

        public override string GetSettingKey()
        {
            return "FS_CustomerFraudInfo_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
