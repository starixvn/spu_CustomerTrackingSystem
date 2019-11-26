namespace Fanex.NAP.CTS.Service.Test.Implement
{
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.Service.Test.DbSetting;
    using System.Configuration;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class SuspiciousCustomerReportServiceTest
    {
        private readonly ISuspiciousCustomerReportService _runner;

        public SuspiciousCustomerReportServiceTest()
        {
            _runner = new SuspiciousCustomerReportService();
            ConfigurationManager.AppSettings[Consts.SettingEnvironmentKey] = Consts.SettingEnvironmentLocalValue;
        }

        [Fact]
        public void Can_GetSuspiciousCustomerReport()
        {
            // Act
            var actualResult = _runner.GetSuspiciousCustomerReport(ExpectedCriteria.ExpectedSuspiciousCustomerReportGetCriteria);

            // Assert
            Assert.NotNull(actualResult);
        }

        [Theory]
        [InlineData(24734887)]
        public void Can_GetNotesData(int custId)
        {
            // Act
            var actualResult = _runner.GetNotesData(custId);

            // Assert
            Assert.NotNull(actualResult);
        }

    }
}