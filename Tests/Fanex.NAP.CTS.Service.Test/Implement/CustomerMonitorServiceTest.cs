namespace Fanex.NAP.CTS.Service.Test.Implement
{
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Model.CustomerMonitor;
    using Fanex.NAP.CTS.Model.MatchMonitor;
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Test.DbSetting;
    using System.Collections.Generic;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class CustomerMonitorServiceTest
    {
        private readonly ICustomerMonitorService _runner;

        public CustomerMonitorServiceTest()
        {
            _runner = new CustomerMonitorService();
        }

        [Fact]
        public void Can_GetBalanceUpDate()
        {
            // Act
            var actualResult = _runner.GetBalanceUpDate();

            // Assert
            Assert.IsType<BalanceUpDate>(actualResult);
        }

        [Fact]
        public void Can_GetCustomers()
        {
            // Act
            var actualResult = _runner.GetCustomers(ExpectedCriteria.ExpectedCustomerGetCriteria);

            // Assert
            Assert.IsType<List<Customer>>(actualResult);
        }

        [Fact]
        public void Can_UpdateNote()
        {
            // Act
            var actualResult = _runner.UpdateNote(ExpectedCriteria.ExpectedUpdateNoteCriteria) as DBResult;

            // Assert
            Assert.True(actualResult.ErrorCode == 0);
        }

        [Fact]
        public void Can_GetTicketDetails()
        {
            // Act
            var actualResult = _runner.GetTicketDetails(ExpectedCriteria.ExpectedGetCustomerTicketsCriteria);

            // Assert
            Assert.NotNull(actualResult);
        }
    }
}