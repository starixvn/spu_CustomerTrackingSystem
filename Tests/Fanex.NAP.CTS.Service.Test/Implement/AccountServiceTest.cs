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
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.Account;

    [Collection(nameof(DbSettingProviderCollection))]
    public class AccountServiceTest
    {
        private readonly IAccountService _runner;

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

        #region Chain To Evidence
        [Theory]
        [InlineData(11113, "1.1")]
        public void Can_GetFlaggedEvidences(int custId, string evidenceCodes)
        {
            // Act
            var actualResult = _runner.GetFlaggedEvidences(custId, evidenceCodes);
          
            // Assert
            Assert.IsType<List<FlaggedEvidenceModel>>(actualResult);
        }

        [Theory]
        [InlineData(11113, 1, 1, "Place by Tuan", 8)]        
        [InlineData(120, 9, 60, "Place by Tuan", 8)]
        public void Can_PlaceEvidence(int ctsCustId, int subscriberId, int evidenceId, string comment, int userId)
        {
            // Act
            var actualResult = _runner.PlaceEvidence(ctsCustId, subscriberId, evidenceId, comment, userId) as DBResult;

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }

        #endregion

    }
}