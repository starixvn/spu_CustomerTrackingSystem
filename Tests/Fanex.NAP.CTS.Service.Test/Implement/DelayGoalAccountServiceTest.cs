namespace Fanex.NAP.CTS.Service.Test.Implement
{
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.Service.Test.DbSetting;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class DelayGoalAccountServiceTest
    {
        private readonly IDelayGoalAccountService _runner;

        public DelayGoalAccountServiceTest()
        {
            _runner = new DelayGoalAccountService();
        }

        [Fact]
        public void Can_InsertAccounts()
        {
            // Act
            var actualResult = _runner.InsertAccounts(ExpectedCriteria.ExpectedDelayGoalAssociationAccountInsertCriteria);

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_GetManualAccounts()
        {
            // Act
            var actualResult = _runner.GetManualAccounts();

            // Assert
            Assert.NotNull(actualResult);
        }
    }
}