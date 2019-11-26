namespace Fanex.NAP.CTS.Controller.Test
{
    using Fanex.NAP.CTS.Controller.Test.DbSetting;
    using System;
    using System.Web.Mvc;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class DailyDataCaptureControllerTest
    {
        private readonly DailyDataCaptureController _runner;

        public DailyDataCaptureControllerTest()
        {
            _runner = new DailyDataCaptureController();
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
        public void Can_GetMatches()
        {
            // Arrange
            var dateFrom = new DateTime(2019, 1, 1);
            var dateTo = new DateTime(2019, 11, 26);

            // Act
            var actualResult = _runner.GetMatches(dateFrom, dateTo) as JsonResult;

            // Assert
            Assert.NotNull(actualResult);
        }
    }
}