namespace Fanex.NAP.CTS.Criteria.CustomerClass
{
    using Fanex.Data.Repository;

    public class SyncCustomerClassToMainDbCriteria : NonQueryCommand
    {
        public string custList { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_CustomerUpdateCustomerClass";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
