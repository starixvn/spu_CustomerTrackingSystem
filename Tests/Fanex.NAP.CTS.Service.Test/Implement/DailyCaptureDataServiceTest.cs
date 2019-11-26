namespace Fanex.NAP.CTS.Service.Test.Implement
{
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Test.DbSetting;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class DailyCaptureDataServiceTest
    {
        private readonly IDailyDataCaptureService _runner;
        public DailyCaptureDataServiceTest()
        {
            _runner = new DailyCaptureDataService();
        }

        [Fact]
        public void Can_GetMatches()
        {
            // Act
            var actualResult = _runner.GetMatches(ExpectedCriteria.ExpectedDailyDataGetCriteria);

            // Assert
            Assert.NotNull(actualResult);
        }
    }
}