namespace Fanex.NAP.CTS.Model.Account
{
    public class CustomerPoliciesModel
    { 
        public decimal MaxPerMatch { get; set; }

        public decimal MaxBet { get; set; }

        public bool IsVIP { get; set; }

        public int  CustomerClass { get; set; }
    }
}