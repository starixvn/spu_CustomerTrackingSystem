using Fanex.NAP.CTS.Model.DTO;
using System;

namespace Fanex.NAP.CTS.Model.Home
{
    public class SearchResultModel
    {
        public SearchResultModel() { }

        public SearchResultModel(SearchAccountResultModel model)
        {
            CTSCustId = model.CTSCustId;
            CustId = model.CustId;
            Username = model.Username;
            Username2 = model.Username2;
            Role = new Role
            {
                Id = model.RoleID
            };
            Currency = new Currency
            {
                Id = model.CurrencyID,
                Name = model.Currency
            };
            Subscriber = new DTO.Subscriber
            {
                Id = model.SubscriberID,
                Name = model.SubscriberName
            };
            LastLogin = model.LastLogin;
        }

        public Int64 CTSCustId { get; set; }

        public int CustId { get; set; }

        public string Username { get; set; }

        public string Username2 { get; set; }

        public Role Role { get; set; }

        public Currency Currency { get; set; }

        public DTO.Subscriber Subscriber { get; set; }

        public DateTime LastLogin { get; set; }
    }
}
