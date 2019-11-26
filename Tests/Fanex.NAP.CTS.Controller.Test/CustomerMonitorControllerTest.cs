namespace Fanex.NAP.CTS.Controller.Test
{
    using Fanex.NAP.CTS.Controller;
    using Fanex.NAP.CTS.Controller.Test.DbSetting;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.Test.Fake;
    using System;
    using System.Configuration;
    using System.Web.Mvc;
    using Xunit;

    [Collection(nameof(DbSettingProviderCollection))]
    public class CustomerMonitorControllerTest
    {
        private readonly CustomerMonitorController _runner;
        private const string IdentityName = "sa";
        private const string ExpectedTemplatePath = @"\App_data\Excel Template\CustomerMonitor\template.xlsx";

        public CustomerMonitorControllerTest()
        {
            ConfigurationManager.AppSettings[Consts.SettingEnvironmentKey] = Consts.SettingEnvironmentLocalValue;
            FakeHttpContext.SetCurrentContext();
            _runner = new CustomerMonitorController(FakeSession.CurrentSession);
        }

        [Fact]
        public void CanView_Index()
        {
            // Act
            var actualResult = _runner.Index();
            
            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void Can_GetCustomers()
        {
            // Act
            var actualResult = _runner.GetCustomers(ExpectedCriteria.ExpectedCustomerGetCriteria) as JsonResult;

            // Assert
            Assert.NotNull(actualResult);
        }

        [Fact]
        public void Can_UpdateNote()
        {
            // Act
            var actualResult = _runner.UpdateNote(ExpectedCriteria.ExpectedUpdateNoteCriteria) as JsonResult;
            var actualDBResult = actualResult.Data as DBResult;

            // Assert
            Assert.True(actualDBResult.ErrorCode == 0);
        }

        [Theory]
        [InlineData("CustomerDetails/Index")]
        public void Can_GetCustomerTicketDetails(string viewName)
        {
            // Act
            var actualResult = _runner.GetCustomerTicketDetails(ExpectedData.ExpectedCustomerDetailsInfo) as ViewResult;

            // Assert
            Assert.Equal(viewName, actualResult.ViewName);
        }

        [Fact]
        public void Can_GetTicketDetails()
        {
            // Act
            var actualResult = _runner.GetTicketDetails(ExpectedCriteria.ExpectedGetCustomerTicketsCriteria) as JsonResult;

            // Assert
            Assert.NotNull(actualResult);
        }
        
        public void Can_ExportExcel()
        {
            // Arrange
            var controllerContextMock = new FakeControllerContext(IdentityName, _runner);
            controllerContextMock.TemplatePath = AppDomain.CurrentDomain.BaseDirectory + ExpectedTemplatePath;
            _runner.TemplatePath = AppDomain.CurrentDomain.BaseDirectory + ExpectedTemplatePath;
            _runner.ControllerContext = controllerContextMock.Instance;

            // Act
            var actualResult = _runner.ExportExcel(ExpectedCriteria.ExpectedCustomerGetCriteria) as FileContentResult;

            // Assert
            Assert.NotNull(actualResult);
        }
    }
}