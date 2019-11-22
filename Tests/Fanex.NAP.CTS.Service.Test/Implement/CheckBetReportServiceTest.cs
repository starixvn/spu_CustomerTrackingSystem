namespace Fanex.NAP.CTS.Service.Test.Implement
{
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Test.DbSetting;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class CheckBetReportServiceTest
    {
        private readonly CheckBetReportService _runner;

        public CheckBetReportServiceTest()
        {
            _runner = new CheckBetReportService();
        }

        [Fact]
        public void Can_Generate()
        {
            // Act
            var actualResult = _runner.Generate(ExpectedCriteria.ExpectedCheckBetReportGetCriteria);

            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void Can_UpdateNote()
        {
            // Act
            var actualResult = _runner.UpdateNote(ExpectedCriteria.ExpectedCheckBetReportUpdateNoteCriteria);

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }
    }
}