namespace Fanex.NAP.CTS.Service.Test.Implement
{
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Test.DbSetting;
    using System;
    using System.Configuration;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class SuspiciousCustomerReportDetailsServiceTest
    {
        private readonly SuspiciousCustomerReportDetailsService _runner;

        public SuspiciousCustomerReportDetailsServiceTest()
        {
            _runner = new SuspiciousCustomerReportDetailsService();
            ConfigurationManager.AppSettings[Consts.SettingEnvironmentKey] = Consts.SettingEnvironmentLocalValue;            
        }

        [Fact]
        public void Can_GetAssignee()
        {
            // Act
            var actualResult = _runner.GetAssignee(24734887);

            // Assert
            Assert.True(!string.IsNullOrEmpty(actualResult));
        }

        [Fact]
        public void Can_UpdateAssignee()
        {
            // Act
            var actualResult = _runner.UpdateAssignee(24734887, 1, 8);

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_GetSuspiciousCustomerInfo()
        {
            // Act
            var actualResult = _runner.GetSuspiciousCustomerInfo(24734887, false);

            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void Can_GetSuspiciousCustomerFraudCase()
        {
            // Act
            var actualResult = _runner.GetSuspiciousCustomerFraudCase(24734887);

            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void Can_VerifySuspiciousCustomerFraudCase()
        {
            // Act
            var actualResult = _runner.VerifySuspiciousCustomerFraudCase(24734887, 8, 6);

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_InsertCustomerLogNotes()
        {
            // Act
            var actualResult = _runner.InsertCustomerLogNotes(24734887, 8, "test");

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
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
        public void Can_DeleteCustomerLogNotes()
        {
            // Act
            var actualResult = _runner.DeleteCustomerLogNotes(8, 24734887);

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }

        [Theory]
        [InlineData(false, false, false)]
        [InlineData(true, true, true)]
        public void Can_UpdateSuspiciousCustomerInfo(bool isDelinquent, bool isDirty, bool isHedge)
        {
            // Act
            var actualResult = _runner.UpdateSuspiciousCustomerInfo(24734887, 8, isDelinquent, isDirty, isHedge);

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_GetCustomerCurrency()
        {
            // Act
            var actualResult = _runner.GetCustomerCurrency(24734887);

            // Assert
            Assert.True(!string.IsNullOrEmpty(actualResult));
        }

        [Fact]
        public void Can_GetStatistics()
        {
            // Arrange
            var custId = 24734887;
            var dateFrom = new DateTime(2019, 01, 25);
            var dateTo = new DateTime(2019, 11, 25);

            // Act
            var actualResult = _runner.GetStatistics(custId, dateFrom, dateTo, false);

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
            var actualResult = _runner.GetViolatedTickets(custId, dateFrom, dateTo, false);

            // Assert
            Assert.NotNull(actualResult);
        }
    }
}