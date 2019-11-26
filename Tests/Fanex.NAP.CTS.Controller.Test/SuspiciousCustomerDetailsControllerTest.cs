namespace Fanex.NAP.CTS.Controller.Test
{
    using Fanex.NAP.CTS.Controller.Test.DbSetting;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Test.Fake;
    using System;
    using System.Configuration;
    using System.Web.Mvc;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class SuspiciousCustomerDetailsControllerTest
    {
        private readonly SuspiciousCustomerDetailsController _runner;

        public SuspiciousCustomerDetailsControllerTest()
        {
            ConfigurationManager.AppSettings[Consts.SettingEnvironmentKey] = Consts.SettingEnvironmentLocalValue;
            FakeHttpContext.SetCurrentContext();
            _runner = new SuspiciousCustomerDetailsController(FakeSession.CurrentSession);
        }

        [Fact]
        public void CanView_Index()
        {
            // Arrange
            var custId = 24734887;
            var custName = "";
            var dateFrom = new DateTime(2019, 01, 25);
            var dateTo = new DateTime(2019, 11, 25);

            // Act
            var actualResult = _runner.Index(custId, custName, dateFrom, dateTo) as ViewResult;

            // Assert
            Assert.NotNull(actualResult.Model);
        }

        [Fact]
        public void Can_UpdateAssignee()
        {
            // Arrange
            var custId = 24734887;
            var assigneeId = 8;

            // Act
            var actualResult = _runner.UpdateAssignee(custId, assigneeId) as JsonResult;
            var actualDBResult = actualResult.Data as DBResult;

            // Assert
            Assert.True(actualDBResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_UpdateCustomerFraudCaseVerify()
        {
            // Act
            var actualResult = _runner.UpdateCustomerFraudCaseVerify(24734887, 6) as JsonResult;
            var actualDBResult = actualResult.Data as DBResult;

            // Assert
            Assert.True(actualDBResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_RemoveLogNote()
        {
            // Act
            var actualResult = _runner.RemoveLogNote(8, 24734887) as JsonResult;
            var actualDBResult = actualResult.Data as DBResult;

            // Assert
            Assert.True(actualDBResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_InsertLogNote()
        {
            // Act
            var actualResult = _runner.InsertLogNote(24734887, "test") as JsonResult;
            var actualDBResult = actualResult.Data as DBResult;

            // Assert
            Assert.True(actualDBResult.ErrorCode == 0);
        }

        [Theory]
        [InlineData(false, false, false)]
        [InlineData(true, true, true)]
        public void Can_UpdateCustomerInfo(bool isDelinquent, bool isDirty, bool isHedge)
        {
            // Act
            var actualResult = _runner.UpdateCustomerInfo(24734887, isDelinquent, isDirty, isHedge) as JsonResult;
            var actualDBResult = actualResult.Data as DBResult;

            // Assert
            Assert.True(actualDBResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_GetCustomerLogNotes()
        {
            // Act
            var actualResult = _runner.GetCustomerLogNotes(24734887);

            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void Can_GetStatistics()
        {
            // Arrange
            var custId = 24734887;
            var dateFrom = new DateTime(2019, 01, 25);
            var dateTo = new DateTime(2019, 11, 25);

            // Act
            var actualResult = _runner.GetStatistics(custId, dateFrom, dateTo);

            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void Can_GetViolatedTickets()
        {
            // Arrange
            var custId = 24734887;
            var dateFrom = new DateTime(2019, 01, 25);
            var dateTo = new DateTime(2019, 11, 25);

            // Act
            var actualResult = _runner.GetViolatedTickets(custId, dateFrom, dateTo);

            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void Can_GetFraudCases()
        {
            // Act
            var actualResult = _runner.GetFraudCases(24734887);

            // Assert
            Assert.NotNull(actualResult);
        }
    }
}