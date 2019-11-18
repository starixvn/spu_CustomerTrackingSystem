namespace Fanex.NAP.CTS.Service.Test.Implement
{
    using Fanex.NAP.CTS.Model.Subscriber;
    using Fanex.NAP.CTS.Resource.Consts;    
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.ViewModel.Account.Search;
    using Fanex.NAP.CTS.Service.Test.DbSetting;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;

    [Collection(nameof(DbSettingProviderCollection))]
    public class AccountServiceTest
    {
        private readonly AccountService _runner;

        public AccountServiceTest()
        {
            _runner = new AccountService();
        }

        #region Search account test
        [Fact]
        public void Can_SearchAccounts()
        {
            // Act
            var result = _runner.SearchAccounts(ExpectedFilter.ExpectedSearchAccountFilter);

            // Assert
            Assert.IsType<List<SubscriberAccount>>(result);
        }

        #endregion

        #region Subscribers to user
        [Theory]
        [InlineData(0)]
        [InlineData(8)]
        public void Can_GetSubscribers(long userId)
        {
            // Act
            var result = _runner.GetSubscribers(userId);

            // Assert
            Assert.IsType<List<SubscriberAssignment>>(result);
        }

        [Fact]
        public void Can_CheckIsMasterSubscriber()
        {
            // Arrange
            var expected = ExpectedData.ExpectedSubscriberAssignments.FirstOrDefault(c => c.IsMaster == 1) != null;

            // Act
            var actualResult = _runner.CheckIsMasterSubscriber(ExpectedData.ExpectedSubscriberAssignments);

            // Assert
            Assert.Equal(expected, actualResult);
        }

        [Fact]
        public void Can_CheckIsPermissionCN88()
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
        public void Can_GetAccountDetailGeneralInformation()
        {
            // Arrange
            var expected = ExpectedData.ExpectedAccountDetail;

            // Act
            var actualResult = _runner.GetAccountDetailGeneralInformation(ExpectedFilter.ExpectedGeneralInformationFilter);

            // Assert
            Assert.IsType(expected.GetType(), actualResult);
        }
        #endregion        

        #region Associated account by account
        [Fact]
        public void Can_GetAssociatedAccountByAccounts()
        {
            // Arrange
            string msgError = string.Empty;

            // Act
            var actualResult = _runner.GetAssociatedAccountByAccounts(ExpectedFilter.ExpectedAssociatedAccountByAccountFilter, out msgError);

            // Assert
            Assert.IsType<List<AssociatedAccountByAccount>>(actualResult);
        }

        #endregion

    }
}