namespace Fanex.NAP.CTS.Criteria.TWProblemAccountDetection
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class NapProblemAccInsertCriteria : NonQueryCommand
    {        
        public string CustomerList { get; set; }

        public int RoleId { get; set; }

        public int ReasonId { get; set; }

        public int PreventActionId { get; set; }

        public string Comment { get; set; }

        public string VoidTime { get; set; }

        public decimal LimitAmount { get; set; }

        public int UserId { get; set; }

        public string IP { get; set; }


        public override string GetSettingKey()
            => "Nap_ProblemAccount_AddCustomer";

        public override bool IsValid()
            => !string.IsNullOrEmpty(CustomerList);
    }
}