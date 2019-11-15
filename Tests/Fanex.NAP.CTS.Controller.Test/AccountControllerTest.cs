namespace Fanex.NAP.CTS.Test
{
    using Fanex.NAP.CTS.Controller;
    using Fanex.NAP.CTS.Controller.Test.DbSetting;
    using Fanex.NAP.CTS.Model.Transaction;
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using Fanex.NAP.CTS.ViewModel.Account.TransactionHistory;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class AccountControllerTest
    {
        private readonly AccountController _runner;
        private const string ExpectedTransactionHistoryViewName = "TransactionHistory/TransactionHistory";

        public AccountControllerTest()
        {
            _runner = new AccountController();
        }

        #region Associated account by account

        [Theory]
        [InlineData("Associated/AccountByAccount")]
        public void GetAssociatedAccountByAccount_Test(string partialViewName)
        {
            // Act
            var actualResult = _runner.GetAssociatedAccountByAccount() as PartialViewResult;

            // Assert
            Assert.Equal(partialViewName, actualResult.ViewName);
        }

        [Fact]
        public void GetAssociatedAccountByAccountFilter_Test()
        {
            // Act
            var actualResult = _runner.GetAssociatedAccountByAccountFilter(ExpectedFilter.ExpectedAssociatedAccountByAccountFilter) as JsonResult;

            // Assert
            Assert.IsType<List<AssociatedAccountByAccount>>(actualResult.Data);
        }

        #endregion

        #region Transaction history
        [Theory]
        [InlineData(12345)]
        public void GetTransactionHistory_Test(int accountId)
        {
            // Act
            var actualResult = _runner.GetTransactionHistory(accountId) as PartialViewResult;

            // Assert
            Assert.Equal(ExpectedTransactionHistoryViewName, actualResult.ViewName);
        }

        [Fact]
        public void GetTransactionHistoryFilter_Test()
        {
            // Act
            var actualResult = _runner.GetTransactionHistoryFilter(ExpectedFilter.ExpectedTransactionHistoryFilter) as JsonResult;

            // Assert
            Assert.IsType<List<Transaction>>(actualResult.Data);
        }

        [Theory]
        [InlineData("20.0.255.255")]
        public void GetIPInformation_Test(string ipAddress)
        {
            // Act
            var actualResult = _runner.GetIPInformation(ipAddress) as PartialViewResult;

            // Assert
            Assert.IsType<IPInformation>(actualResult.Model);
        }

        #endregion
    }
}