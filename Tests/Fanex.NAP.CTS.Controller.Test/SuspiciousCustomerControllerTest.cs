namespace Fanex.NAP.CTS.Controller.Test
{
    using Fanex.NAP.CTS.Controller;
    using Fanex.NAP.CTS.Controller.Test.DbSetting;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.Test.Fake;
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class SuspiciousCustomerControllerTest
    {
        private readonly SuspiciousCustomerController _runner;

        public SuspiciousCustomerControllerTest()
        {
            ConfigurationManager.AppSettings[Consts.SettingEnvironmentKey] = Consts.SettingEnvironmentLocalValue;
            ConfigurationManager.AppSettings[Consts.SettingSiteIdKey] = Consts.SettingSiteIdValue;
            FakeHttpContext.SetCurrentContext();
            _runner = new SuspiciousCustomerController(FakeSession.CurrentSession);
        }

        [Fact]
        public void Index_Test()
        {
            // Act
            var actualResult = _runner.Index() as ViewResult;

            // Assert
            Assert.Equal("", actualResult.ViewName);
        }

        [Fact]
        public void Can_Generate()
        {
            // Act
            var actualResult = _runner.Generate(ExpectedCriteria.ExpectedDataTableAjaxPostCriteria) as JsonResult;

            // Assert
            Assert.NotNull(actualResult.Data);
        }

        [Fact]
        public void Can_FilterData()
        {
            // Arrange
            int filteredResultsCount = 0;
            int totalResultsCount = 0;

            // Act
            var actualResult = _runner.FilterData(ExpectedCriteria.ExpectedDataTableAjaxPostCriteria, out filteredResultsCount, out totalResultsCount);

            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void Can_GetDataFromDbase()
        {
            // Arrange
            int filteredResultsCount = 0;
            int totalResultsCount = 0;

            // Act
            var actualResult = _runner.GetDataFromDbase(ExpectedCriteria.ExpectedSuspiciousCustomerReportGetCriteria, 1, 50, out filteredResultsCount, out totalResultsCount);

            // Assert
            Assert.NotNull(actualResult);
        }
        
        [Fact]
        public void Can_GetFraudCases()
        {
            // Act
            var actualResult = _runner.GetFraudCases() as JsonResult;

            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public async Task Can_GetUsers()
        {
            // Act
            var actualResult = await _runner.GetUsers().ConfigureAwait(false) as JsonResult;

            // Assert
            Assert.NotNull(actualResult);
        }

        [Theory]
        [InlineData(24734887)]
        public void Can_GetSuspiciousCustomerNote(int custId)
        {
            // Act
            var actualResult = _runner.GetSuspiciousCustomerNote(custId) as JsonResult;

            // Assert
            Assert.NotNull(actualResult);
        }

    }
}