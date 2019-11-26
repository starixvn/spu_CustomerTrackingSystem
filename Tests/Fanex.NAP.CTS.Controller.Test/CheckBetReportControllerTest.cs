namespace Fanex.NAP.CTS.Controller.Test
{
    using Fanex.NAP.CTS.Controller;
    using Fanex.NAP.CTS.Controller.Test.DbSetting;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.Test.Fake;
    using System.Web.Mvc;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class CheckBetReportControllerTest
    {
        private readonly CheckBetReportController _runner;

        public CheckBetReportControllerTest()
        {
            FakeHttpContext.SetCurrentContext();
            _runner = new CheckBetReportController(FakeSession.CurrentSession);
        }

        [Fact]
        public void CanView_Index()
        {
            // Act
            var actualResult = _runner.Index() as ViewResult;

            // Assert
            Assert.Equal("", actualResult.ViewName);
        }

        [Fact]
        public void Can_GenerateReport()
        {
            // Act
            var actualResult = _runner.GenerateReport(ExpectedCriteria.ExpectedCheckBetReportGetCriteria) as JsonResult;

            // Assert
            Assert.NotNull(actualResult.Data);
        }

        [Fact]
        public void Can_UpdateNote()
        {
            // Act
            var actualResult = _runner.UpdateNote(ExpectedCriteria.ExpectedCheckBetReportUpdateNoteCriteria) as JsonResult;
            var actualDBResult = actualResult.Data as DBResult;

            // Assert
            Assert.True(actualDBResult.ErrorCode == 0);
        }
    }
}