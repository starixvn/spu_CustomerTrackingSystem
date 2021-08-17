namespace Fanex.NAP.CTS.Criteria.Account
{
    using Fanex.Data.Repository;

    public class CustomerGeneralInfoCriteria : CriteriaBase
    {
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_CustomerGeneralStatus_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
