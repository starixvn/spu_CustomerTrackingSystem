namespace Fanex.NAP.CTS.Test
{
    using Fanex.NAP.CTS.Controller;
    using Fanex.NAP.CTS.Controller.Test.DbSetting;
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

        [Fact]
        public void GetAssociatedAccountByAccounts_Test()
        {
            // Arrange

            // Act
            var actualResult = _runner.GetAssociatedAccountByAccount();
            // Assert

        }

        #endregion
    }
}