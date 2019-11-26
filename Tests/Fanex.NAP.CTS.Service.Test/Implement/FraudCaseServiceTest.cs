namespace Fanex.NAP.CTS.Service.Test.Implement
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Service.Test.DbSetting;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class FraudCaseServiceTest
    {
        private readonly IFraudCaseService _runner;

        public FraudCaseServiceTest()
        {
            _runner = new FraudCaseService();
        }

        [Fact]
        public void Can_Get()
        {
            // Arrange
            var isInternal = 1;
            var isActive = 1;
            var environment = 1;

            // Act
            var actualResult = _runner.Get(isInternal, isActive, environment);

            // Assert
            Assert.NotNull(actualResult);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(6)]
        [InlineData(8)]
        [InlineData(10)]
        [InlineData(11)]
        public void Can_GetByFraudCaseId(int fraudCaseId)
        {
            // Act
            var actualResult = _runner.GetByFraudCaseId(fraudCaseId);

            // Assert
            Assert.NotNull(actualResult);
        }

        [Theory]
        [InlineData(FraudCaseId.BSNG)]
        [InlineData(FraudCaseId.ScannedOfficeIP)]
        [InlineData(FraudCaseId.CheckBet)]
        [InlineData(FraudCaseId.DailyDataCapture)]
        [InlineData(FraudCaseId.FlaggedAccount)]
        [InlineData(FraudCaseId.DelayedGoal)]
        [InlineData(FraudCaseId.Arbitrage)]
        [InlineData(FraudCaseId.BetsNearGoal)]
        public void Can_CheckActiveFraudCase(FraudCaseId fraudCaseId)
        {
            // Act
            var actualResultLocal = _runner.CheckActiveFraudCase(fraudCaseId, 1);
            var actualResultUAT = _runner.CheckActiveFraudCase(fraudCaseId, 2);
            var actualResultPro = _runner.CheckActiveFraudCase(fraudCaseId, 3);

            // Assert
            Assert.IsType<bool>(actualResultLocal);
            Assert.IsType<bool>(actualResultUAT);
            Assert.IsType<bool>(actualResultPro);
        }

        [Theory]
        [InlineData(FraudCaseId.BSNG)]
        [InlineData(FraudCaseId.ScannedOfficeIP)]
        [InlineData(FraudCaseId.CheckBet)]
        [InlineData(FraudCaseId.DailyDataCapture)]
        [InlineData(FraudCaseId.FlaggedAccount)]
        [InlineData(FraudCaseId.DelayedGoal)]
        [InlineData(FraudCaseId.Arbitrage)]
        [InlineData(FraudCaseId.BetsNearGoal)]
        public void Can_ChangeFraudCaseStatus(FraudCaseId fraudCaseId)
        {
            // Act
            _runner.ChangeFraudCaseStatus(fraudCaseId, 8);          
        }

        [Fact]
        public void Can_GetDelayedGoalCustomers()
        {
            // Act
            var actualResult = _runner.GetDelayedGoalCustomers();

            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void Can_DeleteDelayedGoalCustomer()
        {
            // Arrange
            var custId = 24734887;
            var userId = 8;

            // Act
            var actualResult = _runner.DeleteDelayedGoalCustomer(custId, userId);

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_AddDelayedGoalCustomer()
        {
            // Arrange
            var userName = "sa";
            var userId = 8;

            // Act
            var actualResult = _runner.AddDelayedGoalCustomer(userName, userId);

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_AddDelayGoalAssociatedAccount()
        {
            // Arrange
            var customerAssociationXML = "";

            // Act
            var actualResult = _runner.AddDelayGoalAssociatedAccount(customerAssociationXML);

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }

    }
}