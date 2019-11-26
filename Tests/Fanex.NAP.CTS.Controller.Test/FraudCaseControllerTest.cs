namespace Fanex.NAP.CTS.Controller.Test
{
    using Fanex.NAP.CTS.Controller.Test.DbSetting;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model;
    using System.Web.Mvc;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class FraudCaseControllerTest
    {
        private readonly FraudCaseController _runner;

        public FraudCaseControllerTest()
        {
            _runner = new FraudCaseController();
        }

        [Theory]
        [InlineData(3)]
        public void CanView_Index(int fraudCaseId)
        {
            // Act
            var actualResult1 = _runner.Index() as ViewResult;
            var actualResult2 = _runner.Index(fraudCaseId) as ViewResult;

            // Assert
            Assert.NotNull(actualResult1.Model);
            Assert.NotNull(actualResult2.Model);
        }

        #region delay goal customer
        [Fact]
        public void Can_GetDelayedGoalCustomerList()
        {
            // Act
            var actualResult = _runner.GetDelayedGoalCustomerList() as JsonResult;

            // Assert
            Assert.NotNull(actualResult.Data);
        }

        [Fact]
        public void Can_DeleteDelayedGoalCustomer()
        {
            // Arrange
            var custId = 24734887;

            // Act
            var actualResult = _runner.DeleteDelayedGoalCustomer(custId) as JsonResult;
            var actualDBResult = actualResult.Data as DBResult;

            // Assert
            Assert.True(actualDBResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_AddDelayedGoalCustomer()
        {
            // Arrange
            var userName = "sa";

            // Act
            var actualResult = _runner.AddDelayedGoalCustomer(userName) as JsonResult;
            var actualDBResult = actualResult.Data as DBResult;

            // Assert
            Assert.True(actualDBResult.ErrorCode == 0);
        }
        #endregion

        #region scanned office ip
        [Fact]
        public void Can_GetIpList()
        {
            // Act
            var actualResult = _runner.GetIpList() as JsonResult;

            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void Can_DeleteIpRange()
        {
            // Arrange
            var fromIp = "192.168.254.100";
            var toIp = "192.168.254.110";

            // Act
            var actualResult = _runner.DeleteIpRange(fromIp, toIp) as JsonResult;
            var actualDBResult = actualResult.Data as DBResult;

            // Assert
            Assert.True(actualDBResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_AddIpRange()
        {
            // Arrange
            var fromIp = "192.168.254.100";
            var toIp = "192.168.254.110";

            // Act
            var actualResult = _runner.AddIpRange(fromIp, toIp) as JsonResult;
            var actualDBResult = actualResult.Data as DBResult;

            // Assert
            Assert.True(actualDBResult.ErrorCode == 0);
        }

        #endregion

        #region scanned office internal account
        [Fact]
        public void Can_GetAllInternalAccount()
        {
            // Act
            var actualResult = _runner.GetAllInternalAccount() as JsonResult;

            // Assert
            Assert.NotNull(actualResult.Data);
        }

        [Fact]
        public void Can_AddInternalAccount()
        {
            // Arrange
            var userName = "sa";

            // Act
            var actualResult = _runner.AddInternalAccount(userName) as JsonResult;
            var actualDBResult = actualResult.Data as DBResult;

            // Assert
            Assert.True(actualDBResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_DeleteInternalAccount()
        {
            // Arrange
            var custId = 24734887;

            // Act
            var actualResult = _runner.DeleteInternalAccount(custId) as JsonResult;
            var actualDBResult = actualResult.Data as DBResult;

            // Assert
            Assert.True(actualDBResult.ErrorCode == 0);
        }

        #endregion

        #region notification recipient
        [Fact]
        public void Can_GetRecipients()
        {
            // Arrange
            var custId = 24734887;

            // Act
            var actualResult = _runner.DeleteInternalAccount(custId) as JsonResult;

            // Assert
            Assert.NotNull(actualResult.Data);
        }

        [Fact]
        public void Can_AddRecipients()
        {
            // Arrange
            var recipients = "1";
            var fraudCase = FraudCaseId.CheckBet;

            // Act
            var actualResult = _runner.AddRecipients(recipients, fraudCase) as JsonResult;
            var actualDBResult = actualResult.Data as DBResult;

            // Assert
            Assert.True(actualDBResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_RemoveRecipient()
        {
            // Arrange
            var id = 1;
            var fraudCase = FraudCaseId.CheckBet;

            // Act
            var actualResult = _runner.RemoveRecipient(id, fraudCase) as JsonResult;
            var actualDBResult = actualResult.Data as DBResult;

            // Assert
            Assert.True(actualDBResult.ErrorCode == 0);
        }

        #endregion
    }
}