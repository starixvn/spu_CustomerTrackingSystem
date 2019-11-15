namespace Fanex.NAP.CTS.Controller.Test
{
    using System.Web.Mvc;
    using System.Collections.Generic;
    using Xunit;
    using Fanex.NAP.CTS.ViewModel.Account.Search;   
    using Fanex.NAP.CTS.Model.Subscriber;    
    using Fanex.NAP.CTS.Controller.Test.DbSetting;
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.Test.Mocks;
    using System.Configuration;

    [Collection(nameof(DbSettingProviderCollection))]
    public class SearchAccountControllerTest
    {
        private readonly SearchAccountController _runner;
        private const string ExpectedActionIndex = "Index";
        private const string IdentityName = "sa";
        private const string Environment = "Environment";
        private const string EnvironmentLocal = "1";

        public SearchAccountControllerTest()
        {
            _runner = new SearchAccountController();
        }

        [Theory]
        [InlineData("")]       
        public void Index_Test(string viewName)
        {
            // Act
            var actualResult = _runner.Index() as ViewResult;

            // Assert
            Assert.Equal(viewName, actualResult.ViewName);
        }

        [Theory]
        [InlineData("")]
        public void SearchAccount_Test(string keySearch)
        {
            // Act
            var actualResult = _runner.SearchAccount(keySearch) as ViewResult;

            // Assert
            Assert.Equal(ExpectedActionIndex, actualResult.ViewName);
        }

        [Fact]
        public void SearchSubscriberAccount_Test()
        {
            // Arrange
            ConfigurationManager.AppSettings[Environment] = EnvironmentLocal;
            var controllerContextMock = new ControllerContextMock(IdentityName, _runner);
            _runner.ControllerContext = controllerContextMock.Instance;

            // Act
            var actualResult = _runner.SearchSubscriberAccount(ExpectedFilter.ExpectedSearchAccountFilter) as JsonResult;

            // Assert
            Assert.IsType<List<SubscriberAccount>>(actualResult.Data);
        }

        [Fact]
        public void GetSubscribers_Test()
        {
            // Act
            var actualResult = _runner.GetSubscribers() as JsonResult;

            // Assert
            Assert.IsType<List<SubscriberAssignment>>(actualResult.Data);
        }

    }
}