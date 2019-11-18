namespace Fanex.NAP.CTS.Service.Test.Implement
{
    using Fanex.NAP.CTS.Model.Subscriber;
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.Service.Test.DbSetting;
    using System.Collections.Generic;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class SubscriberAssignmentServiceTest
    {
        private readonly SubscriberAssignmentService _runner;

        public SubscriberAssignmentServiceTest()
        {
            _runner = new SubscriberAssignmentService();
        }

        [Theory]
        [InlineData(8)]
        public void Can_GetSubscribers(long userId)
        {
            // Act
            var actualResult = _runner.GetSubscribers(userId);

            // Assert
            Assert.IsType<List<SubscriberAssignment>>(actualResult);
        }

        [Fact]
        public void UpdateSuccess_UpdateSubscribers()
        {
            // Act
            var actualResult = _runner.UpdateSubscribers(ExpectedUpdate.ExpectedSubscriberAssignmentUpdate);

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }
    }
}