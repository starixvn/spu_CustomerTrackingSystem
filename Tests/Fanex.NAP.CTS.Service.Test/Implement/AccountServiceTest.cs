namespace Fanex.NAP.CTS.Service.Test.Implement
{
    using Fanex.NAP.CTS.Model.Subscriber;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Test.Mocks;
    using Fanex.NAP.CTS.Service.Test.SampleData;
    using Fanex.NAP.CTS.ViewModel.Account.Search;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class AccountServiceTest
    {
        private readonly AccountServiceMock _runner;

        public AccountServiceTest()
        {
            _runner = new AccountServiceMock();
        }

        #region Search account test
        [Fact]
        public void SearchAccounts_Test()
        {
            // Arrange
            _runner.GenerateSearchAccountFilter();

            // Act
            var result = _runner.SearchAccounts();

            // Assert
            Assert.IsType<List<SubscriberAccount>>(result);
        }

        #endregion

        #region Subscribers to user
        [Theory]
        [InlineData(0)]
        [InlineData(8)]
        public void GetSubscribers_Test(long userId)
        {
            // Act
            var result = _runner.GetSubscribers(userId);

            // Assert
            Assert.IsType<List<SubscriberAssignment>>(result);
        }

        [Fact]
        public void CheckIsMasterSubscriber_Test()
        {
            // Arrange
            var expected = ExpectedData.ExpectedSubscriberAssignments.FirstOrDefault(c => c.IsMaster == 1) != null;

            // Act
            var actualResult = _runner.CheckIsMasterSubscriber(ExpectedData.ExpectedSubscriberAssignments);

            // Assert
            Assert.Equal(expected, actualResult);
        }

        [Fact]
        public void CheckIsPermissionCN88_Test()
        {
            // Arrange
            var expected = ExpectedData.ExpectedSubscriberAssignments.FirstOrDefault(c => c.IsMaster == 1
                                        || (c.isAssigned == 1 && c.SubscriberName.ToUpper().Contains(Consts.SubscriberNameCN88))) != null;

            // Act
            var actualResult = _runner.CheckIsPermissionCN88(ExpectedData.ExpectedSubscriberAssignments);

            // Assert
            Assert.Equal(expected, actualResult);
        }
        #endregion

        #region General information
        [Fact]
        public void GetAccountDetailGeneralInformation()
        {
            // Arrange
            var expected = ExpectedData.ExpectedAccountDetail;
            _runner.GenerateGeneralInformationFilter();

            // Act
            var actualResult = _runner.GetAccountDetailGeneralInformation();

            // Assert
            Assert.IsType(expected.GetType(), actualResult);
        }
        #endregion

    }
}