namespace Fanex.NAP.CTS.Test
{
    using Fanex.NAP.CTS.Controller;
    using Fanex.NAP.CTS.Controller.Test.DbSetting;
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.ViewModel.Account.Associated;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class AccountControllerTest
    {
        private readonly AccountController _runner;

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
    }
}