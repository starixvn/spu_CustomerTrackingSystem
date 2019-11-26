namespace Fanex.NAP.CTS.Service.Test.Implement
{
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Test.DbSetting;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class OfficeIPServiceTest
    {
        private readonly IOfficeIPService _runner;

        public OfficeIPServiceTest()
        {
            _runner = new OfficeIPService();
        }

        [Fact]
        public void Can_AddIpRange()
        {
            // Arrange
            var fromIp = "192.168.254.100";
            var toIp = "192.168.254.110";
            var userId = 8;

            // Act
            var actualResult = _runner.AddIpRange(fromIp, toIp, userId);

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_DeleteIpRange()
        {
            // Arrange
            var fromIp = "192.168.254.100";
            var toIp = "192.168.254.110";
            var userId = 8;

            // Act
            var actualResult = _runner.DeleteIpRange(fromIp, toIp, userId);

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_GetIps()
        {
            // Act
            var actualResult = _runner.GetIps();

            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void Can_GetInternalAccounts()
        {
            // Act
            var actualResult = _runner.GetInternalAccounts();

            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void Can_AddInternalAccount()
        {
            // Arrange
            var userName = "sa";
            var userId = 8;

            // Act
            var actualResult = _runner.AddInternalAccount(userName, userId);

            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void Can_DeleteInternalAccount()
        {
            // Arrange
            var custId = 24734887;
            var userId = 8;

            // Act
            var actualResult = _runner.DeleteInternalAccount(custId, userId);

            // Assert
            Assert.NotNull(actualResult);
        }
    }
}