namespace Fanex.NAP.CTS.Service.Test.Implement
{
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Test.DbSetting;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class NotificationRecipientServiceTest
    {
        private readonly INotificationRecipientService _runner;

        public NotificationRecipientServiceTest()
        {
            _runner = new NotificationRecipientService();
        }

        [Fact]
        public void Can_AddRecipient()
        {
            // Arrange
            var recipients = "1";
            var userId = 8;
            var fraudCase = FraudCaseId.CheckBet;

            // Act
            var actualResult = _runner.AddRecipient(recipients, userId, fraudCase);

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_GetRecipients()
        {
            // Act
            var actualResult = _runner.GetRecipients(FraudCaseId.DelayedGoal);

            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void Can_RemoveRecipient()
        {
            // Arrange
            var recipients = 1;
            var userId = 8;
            var fraudCase = FraudCaseId.CheckBet;

            // Act
            var actualResult = _runner.RemoveRecipient(recipients, userId, fraudCase);

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }
    }
}