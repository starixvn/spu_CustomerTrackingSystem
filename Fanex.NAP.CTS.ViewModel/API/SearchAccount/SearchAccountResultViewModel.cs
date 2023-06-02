namespace Fanex.NAP.CTS.ViewModel.API.SearchAccount
{
    using Fanex.NAP.CTS.Model.API.FPS.SearchAccount;

    public class SearchAccountResultViewModel
    {
        public SearchAccountResultViewModel(SearchAccountItemModel data)
        {
            this.CTSCustID = data.CTSCustID;
            this.Username = data.Username;
            this.RegisterName = data.RegisterName;
            this.SubscriberID = data.SubscriberID;
        }

        public long CTSCustID { get; set; }

        public string Username { get; set; }

        public string RegisterName { get; set; }

        public int SubscriberID { get; set; }
    }
}
