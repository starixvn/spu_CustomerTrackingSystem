namespace Fanex.NAP.CTS.Controller.Test
{
    using System.Web.Mvc;
    using System.Collections.Generic;
    using Xunit;
    using Fanex.NAP.CTS.ViewModel.Account.Search;   
    using Fanex.NAP.CTS.Model.Subscriber;    
    using Fanex.NAP.CTS.Controller.Test.DbSetting;
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.Test.Fake;
    using System.Configuration;
    using Fanex.NAP.CTS.Resource.Consts;

    [Collection(nameof(DbSettingProviderCollection))]
    public class SearchAccountControllerTest
    {
        private readonly SearchAccountController _runner;
        private const string ExpectedActionIndex = "Index";
        private const string IdentityName = "sa";

        public SearchAccountControllerTest()
        {
            _runner = new SearchAccountController();
            ConfigurationManager.AppSettings[Consts.SettingEnvironmentKey] = Consts.SettingEnvironmentLocalValue;
        }

        [Theory]
        [InlineData("")]       
        public void CanView_Index(string viewName)
        {
            // Arrange
            var controllerContextMock = new FakeControllerContext(IdentityName, _runner);
            _runner.ControllerContext = controllerContextMock.Instance;

            // Act
            var actualResult = _runner.Index() as ViewResult;

            // Assert
            Assert.Equal(viewName, actualResult.ViewName);
        }

        [Theory]
        [InlineData("")]
        public void Can_SearchAccount(string keySearch)
        {
            // Act
            var actualResult = _runner.SearchAccount(keySearch) as ViewResult;

            // Assert
            Assert.Equal(ExpectedActionIndex, actualResult.ViewName);
        }

        [Fact]
        public void Can_SearchSubscriberAccount()
        {
            // Arrange            
            var controllerContextMock = new FakeControllerContext(IdentityName, _runner);
            _runner.ControllerContext = controllerContextMock.Instance;

            // Act
            var actualResult = _runner.SearchSubscriberAccount(ExpectedFilter.ExpectedSearchAccountFilter) as JsonResult;

            // Assert
            Assert.IsType<List<SubscriberAccount>>(actualResult.Data);
        }

        [Fact]
        public void Can_GetSubscribers()
        {
            // Arrange            
            var fakecontrollerContext = new FakeControllerContext(IdentityName, _runner);
            _runner.ControllerContext = fakecontrollerContext.Instance;

            // Act
            var actualResult = _runner.GetSubscribers() as JsonResult;

            // Assert
            Assert.IsType<List<SubscriberAssignment>>(actualResult.Data);
        }

    }
}