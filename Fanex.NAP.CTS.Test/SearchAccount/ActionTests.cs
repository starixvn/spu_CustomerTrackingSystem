namespace Fanex.NAP.CTS.Test.Site.SearchAccount
{
    using System.Web.Mvc;
    using System.Collections.Generic;
    using Xunit;
    using Fanex.NAP.CTS.ViewModel.Account.Search;
    using Fanex.NAP.CTS.Test.Mocks.Controllers;    
    using Fanex.NAP.CTS.Model.Subscriber;

    [Collection(nameof(DbSettingProviderCollection))]
    public class ActionTests
    {
        private readonly SearchAccountControllerMock _runner;
        private const string ExpectedActionIndex = "Index";

        public ActionTests()
        {
            _runner = new SearchAccountControllerMock();
            _runner.SetPrincipal("sa");
            _runner.SetAppSetting("Environment", "1");
        }

        [Theory]
        [InlineData("")]       
        public void Index_Test(string viewName)
        {           
            // Act
            var result = _runner.Index() as ViewResult;

            // Assert
            Assert.Equal(viewName, result.ViewName);
        }

        [Theory]
        [InlineData("")]
        public void SearchAccount_Test(string keySearch)
        {
            // Act
            var result = _runner.SearchAccount(keySearch) as ViewResult;

            // Assert
            Assert.Equal(ExpectedActionIndex, result.ViewName);
        }

        [Fact]
        public void SearchSubscriberAccount_Test()
        {
            // Act 
            _runner.GenerateSearchAccountFilter();
            var result = _runner.SearchSubscriberAccount() as JsonResult;

            // Assert
            Assert.IsType<List<SubscriberAccount>>(result.Data);
        }

        [Fact]
        public void GetSubscribers_Test()
        {
            // Act
            var result = _runner.GetSubscribers() as JsonResult;

            // Assert
            Assert.IsType<List<SubscriberAssignment>>(result.Data);
        }

    }
}