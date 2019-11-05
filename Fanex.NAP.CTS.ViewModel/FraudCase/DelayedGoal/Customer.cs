namespace Fanex.NAP.CTS.ViewModel.FraudCase.DelayedGoal
{
    public class Customer
    {
        public Customer(Fanex.NAP.CTS.Model.FraudCase.DelayedGoal.Customer customer)
        {
            Id = customer.CustId;
            Name = customer.AccountName ?? "-";
            Creator = customer.CreatedBy ?? "-";
            AddedDate = customer.CreatedDate.ToString("MMM dd, yyyy hh:mm:ss tt");
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Creator { get; set; }

        public string AddedDate { get; set; }
    }
}