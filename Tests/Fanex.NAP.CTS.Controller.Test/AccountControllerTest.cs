namespace Fanex.NAP.CTS.Controller.Test
{
    using Fanex.NAP.CTS.Controller;
    using Fanex.NAP.CTS.Controller.Test.DbSetting;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.ViewModel.Evidence;
    using Fanex.NAP.CTS.Model.Transaction;
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.Test.Fake;
    using Fanex.NAP.CTS.ViewModel.Account;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using Fanex.NAP.CTS.ViewModel.Account.Evidence;
    using Fanex.NAP.CTS.ViewModel.Account.TransactionHistory;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class AccountControllerTest
    {
        private readonly AccountController _runner;
        private const string ExpectedTransactionHistoryViewName = "TransactionHistory/TransactionHistory";
        private const string IdentityName = "sa";

        public AccountControllerTest()
        {
            _runner = new AccountController();
        }

        //#region General information
        //[Fact]
        //public void Can_GetGeneralInformation()
        //{
        //    // Arrange
        //    FakeHttpContext.SetCurrentContext();
        //    FakeHttpContext.SetFakeSession();
        //    _runner.CurrentSession = FakeSession.CurrentSession;

        //    // Act
        //    var actualResult = _runner.GetGeneralInformation(ExpectedFilter.ExpectedGeneralInformationFilter) as JsonResult;

        //    // Assert
        //    Assert.IsType<AccountDetail>(actualResult.Data);
        //}

        //[Fact]
        //public void Can_GetAllEvidences()
        //{
        //    // Act
        //    var actualResult = _runner.GetAllEvidences() as JsonResult;

        //    // Assert
        //    Assert.IsType<List<Evidence>>(actualResult.Data);
        //}

        //#endregion

        //#region Chain to evidence
        //[Theory]
        //[InlineData("DetailInfoAccount/ChainsToEvidence")]
        //public void Can_GetChainsToEvidence(string partialViewName)
        //{
        //    // Arrange
        //    FakeHttpContext.SetCurrentContext();
        //    FakeHttpContext.SetFakeSession();
        //    _runner.CurrentSession = FakeSession.CurrentSession;

        //    // Act
        //    var actualResult = _runner.GetChainsToEvidence(ExpectedFilter.ExpectedChainsToEvidenceFilter) as PartialViewResult;

        //    // Assert
        //    Assert.Equal(partialViewName, actualResult.ViewName);
        //    Assert.IsType<ChainsToEvidence>(actualResult.Model);
        //}

        //[Fact]
        //public void Can_AddExceptionEvidence()
        //{
        //    // Arrange
        //    _runner.CurrentSession = FakeSession.CurrentSession;

        //    // Act
        //    var actualResult = _runner.AddExceptionEvidence(ExpectedUpdate.ExpectedExceptionEvidenceAdd) as JsonResult;
        //    var actualDBResult = actualResult.Data as DBResult;

        //    // Assert
        //    Assert.True(actualDBResult.ErrorCode == 0);
        //}
        //#endregion

        //#region Retract and exception
        //[Theory]
        //[InlineData("DetailInfoAccount/RetractAndException")]
        //public void Can_GetRetractAndException(string partialViewName)
        //{
        //    // Arrange
        //    FakeHttpContext.SetCurrentContext();
        //    FakeHttpContext.SetFakeSession();
        //    _runner.CurrentSession = FakeSession.CurrentSession;

        //    // Act
        //    var actualResult = _runner.GetRetractAndException(ExpectedFilter.ExpectedRetractedAndExceptionEvidenceFilter) as PartialViewResult;

        //    // Assert
        //    Assert.Equal(partialViewName, actualResult.ViewName);
        //    Assert.IsType<RetractedAndExceptionEvidence>(actualResult.Model);
        //}
        //#endregion

        //#region Associated account by account

        //[Theory]
        //[InlineData("Associated/AccountByAccount")]
        //public void Can_GetAssociatedAccountByAccount(string partialViewName)
        //{
        //    // Act
        //    var actualResult = _runner.GetAssociatedAccountByAccount() as PartialViewResult;

        //    // Assert
        //    Assert.Equal(partialViewName, actualResult.ViewName);
        //}

        //[Fact]
        //public void Can_GetAssociatedAccountByAccountFilter()
        //{
        //    // Act
        //    var actualResult = _runner.GetAssociatedAccountByAccountFilter(ExpectedFilter.ExpectedAssociatedAccountByAccountFilter) as JsonResult;

        //    // Assert
        //    Assert.IsType<List<AssociatedAccountByAccount>>(actualResult.Data);
        //}

        //#endregion

        //#region Transaction history
        //[Theory]
        //[InlineData(12345)]
        //public void Can_GetTransactionHistory(int accountId)
        //{
        //    // Act
        //    var actualResult = _runner.GetTransactionHistory(accountId) as PartialViewResult;

        //    // Assert
        //    Assert.Equal(ExpectedTransactionHistoryViewName, actualResult.ViewName);
        //}

        //[Fact]
        //public void Can_GetTransactionHistoryFilter()
        //{
        //    // Act
        //    var actualResult = _runner.GetTransactionHistoryFilter(ExpectedFilter.ExpectedTransactionHistoryFilter) as JsonResult;

        //    // Assert
        //    Assert.IsType<List<Transaction>>(actualResult.Data);
        //}

        //[Theory]
        //[InlineData("20.0.255.255")]
        //public void Can_GetIPInformation(string ipAddress)
        //{
        //    // Act
        //    var actualResult = _runner.GetIPInformation(ipAddress) as PartialViewResult;

        //    // Assert
        //    Assert.IsType<IPInformation>(actualResult.Model);
        //}

        //#endregion
    }
}