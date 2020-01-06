namespace Fanex.NAP.CTS.ViewModel.Account.Associated
{
    public class GraphCustomerItemViewModel
    {
        public GraphCustomerItemViewModel() { }

        public int CustId { get; set; }

        public string Username { get; set; }

        public string Username2 { get; set; }

        public string AssociationType { get; set; }

        public int id { get; set; }

        public int index { get; set; }

        public int level { get; set; }

        public GraphCustomerItemViewModel[] children { get; set; }
    }
}