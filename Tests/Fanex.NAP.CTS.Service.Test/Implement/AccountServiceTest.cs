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
        //[Fact]
        //public void Can_SearchAccounts()
        //{
        //    // Act
        //    var result = _runner.SearchAccounts(ExpectedFilter.ExpectedSearchAccountFilter);

        //    // Assert
        //    Assert.IsType<List<SubscriberAccount>>(result);
        //}

        #endregion

        #region Subscribers to user
        //[Theory]
        //[InlineData(0)]
        //[InlineData(8)]
        //public void Can_GetSubscribers(long userId)
        //{
        //    // Act
        //    var result = _runner.GetSubscribers(userId);

        //    // Assert
        //    Assert.IsType<List<SubscriberAssignment>>(result);
        //}

        //[Fact]
        //public void Can_CheckIsMasterSubscriber()
        //{
        //    // Arrange
        //    var expected = ExpectedData.ExpectedSubscriberAssignments.FirstOrDefault(c => c.IsMaster == 1) != null;

        //    // Act
        //    var actualResult = _runner.CheckIsMasterSubscriber(ExpectedData.ExpectedSubscriberAssignments);

        //    // Assert
        //    Assert.Equal(expected, actualResult);
        //}

        //[Fact]
        //public void Can_CheckIsPermissionCN88()
        //{
        //    // Arrange
        //    var expected = ExpectedData.ExpectedSubscriberAssignments.FirstOrDefault(c => c.IsMaster == 1
        //                                || (c.isAssigned == 1 && c.SubscriberName.ToUpper().Contains(Consts.SubscriberNameCN88))) != null;

        //    // Act
        //    var actualResult = _runner.CheckIsPermissionCN88(ExpectedData.ExpectedSubscriberAssignments);

        //    // Assert
        //    Assert.Equal(expected, actualResult);
        //}
        #endregion

        #region General information
        //[Fact]
        //public void Can_GetAccountDetailGeneralInformation()
        //{
        //    // Arrange
        //    var expected = ExpectedData.ExpectedAccountDetail;

        //    // Act
        //    var actualResult = _runner.GetAccountDetailGeneralInformation(ExpectedFilter.ExpectedGeneralInformationFilter);

        //    // Assert
        //    Assert.IsType(expected.GetType(), actualResult);
        //}
        #endregion        

        #region Associated account by account
      
        #endregion

        #region Chain To Evidence
        [Fact]
        public void Can_GetEvidenceList()
        {
            // Act
            var actualResult = _runner.GetEvidenceList();

            // Assert
            Assert.IsType<List<EvidenceModel>>(actualResult);
        }

        [Theory]
        [InlineData(11113, "6.8")]
        public void Can_GetFlaggedEvidences(int ctsCustId, string evidenceCodes)
        {
            // Act
            var actualResult = _runner.GetFlaggedEvidences(ctsCustId, evidenceCodes);
          
            // Assert
            Assert.IsType<List<FlaggedEvidenceModel>>(actualResult);
        }

        [Fact]
        public void Can_RemoveFlaggedEvidence()
        {
            // Act
            var actualResult = _runner.RemoveFlaggedEvidence(ExpectedUpdate.ExpectedRemoveFlaggedEvidenceModel) as DBResult;

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }

        [Theory]
        [InlineData(11113, "6.8")]
        public void Can_GetAffectedRetractEvidences(int ctsCustId, string evidenceCodes)
        {
            // Act
            var actualResult = _runner.GetAffectedRetractEvidences(ctsCustId, evidenceCodes);

            // Assert
            Assert.IsType<AffectedRetractEvidenceModel>(actualResult);
        }

        [Fact]
        public void Can_RetractAffectedEvidence()
        {
            // Act
            var actualResult = _runner.RetractAffectedEvidence(ExpectedUpdate.ExpectedRetractAffectedEvidenceModel);

            // Assert
            Assert.True(actualResult == 0);
        }

        [Fact]
        public void Can_RestoreAffectedEvidence()
        {
            // Act
            var actualResult = _runner.RestoreAffectedEvidence(ExpectedUpdate.ExpectedRestoreAffectedEvidenceModel);

            // Assert
            Assert.True(actualResult == 0);
        }

        [Theory]
        [InlineData(501755, 18491)]
        public void Can_GetAffectedEvidenceListBetweenCusts(int fromCTSCustId, int toCTSCustId)
        {
            // Act
            var actualResult = _runner.GetAffectedEvidenceListBetweenCusts(fromCTSCustId, toCTSCustId);

            // Assert
            Assert.IsType<List<AffectedEvidenceListBetweenCustsModel>>(actualResult);
        }
      
        [Fact]
        public void Can_AddAccountException()
        {
            // Act
            var actualResult = _runner.AddAccountException(ExpectedUpdate.ExpectedUpdateAccountExceptionModel);

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }

        #endregion

    }
}