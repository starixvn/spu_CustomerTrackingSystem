namespace Fanex.NAP.CTS.Service.Test.Mocks
{
    using Fanex.NAP.CTS.ViewModel.Account;
    using Fanex.NAP.CTS.ViewModel.Account.GeneralInformation;
    using Fanex.NAP.CTS.ViewModel.Account.Search;
    using Moq;
    using System.Collections.Generic;

    public class AccountServiceMock : AccountService
    {
        private Mock<SearchAccountFilter> _searchAccountFilter;
        private Mock<GeneralInformationFilter> _generalInformationFilter;

        public AccountServiceMock() : base()
        { }

        public void GenerateSearchAccountFilter()
        {
            _searchAccountFilter = new Mock<SearchAccountFilter>();
            _searchAccountFilter.Object.Key = "12b";
            _searchAccountFilter.Object.Subscribers = "-1";
        }

        public void GenerateGeneralInformationFilter()
        {
            _generalInformationFilter = new Mock<GeneralInformationFilter>();            
        }

        public IEnumerable<SubscriberAccount> SearchAccounts()
        {
            return base.SearchAccounts(_searchAccountFilter.Object);
        }
        
        public AccountDetail GetAccountDetailGeneralInformation()
        {
            return base.GetAccountDetailGeneralInformation(_generalInformationFilter.Object);
        }
    }
}