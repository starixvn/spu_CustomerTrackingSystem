namespace Fanex.NAP.CTS.Service.Test.Implement
{
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Test.DbSetting;
    using System;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class ScannedOfficeIpReportServiceTest
    {
        private readonly ScannedOfficeIpReportService _runner;

        public ScannedOfficeIpReportServiceTest()
        {
            _runner = new ScannedOfficeIpReportService();
        }

        [Fact]
        public void Can_GetTickets()
        {
            // Arrange
            var dateFrom = new DateTime(2019, 01, 25);
            var dateTo = new DateTime(2019, 11, 25);
            var isShowSpecialCurrencies = false;

            // Act
            var actualResult = _runner.GetTickets(dateFrom, dateTo, isShowSpecialCurrencies);

            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void Can_GetRawTickets()
        {
            // Arrange
            var dateFrom = new DateTime(2019, 01, 25);
            var dateTo = new DateTime(2019, 11, 25);
            var isShowSpecialCurrencies = false;

            // Act
            var actualResult = _runner.GetRawTickets(dateFrom, dateTo, isShowSpecialCurrencies);

            // Assert
            Assert.NotNull(actualResult);
        }
    }
}