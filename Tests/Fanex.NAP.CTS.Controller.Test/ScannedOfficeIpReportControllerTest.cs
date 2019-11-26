namespace Fanex.NAP.CTS.Controller.Test
{
    using Fanex.NAP.CTS.Controller.Test.DbSetting;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Test.Fake;
    using System;
    using System.Configuration;
    using System.Web.Mvc;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class ScannedOfficeIpReportControllerTest
    {
        private readonly ScannedOfficeIpReportController _runner;

        public ScannedOfficeIpReportControllerTest()
        {
            ConfigurationManager.AppSettings[Consts.SettingEnvironmentKey] = Consts.SettingEnvironmentLocalValue;
            FakeHttpContext.SetCurrentContext();
            _runner = new ScannedOfficeIpReportController(FakeSession.CurrentSession);
        }

        [Fact]
        public void CanView_Index()
        {
            // Act
            var actualResult = _runner.Index() as ViewResult;

            // Assert
            Assert.True(actualResult.ViewName == string.Empty);
        }

        [Fact]
        public void Can_GetTickets()
        {
            // Arrange
            var dateFrom = new DateTime(2019, 01, 25);
            var dateTo = new DateTime(2019, 11, 25);

            // Act
            var actualResult = _runner.GetTickets(dateFrom, dateTo) as JsonResult;

            // Assert
            Assert.NotNull(actualResult.Data);
        }
    }
}