namespace Fanex.NAP.CTS.Service.Test.Implement
{
    using Fanex.NAP.CTS.SampleData;
    using Fanex.NAP.CTS.Service.Test.DbSetting;
    using Fanex.NAP.CTS.Model.Transaction;
    using Fanex.NAP.CTS.ViewModel.Account.TransactionHistory;
    using System.Collections.Generic;    
    using Xunit;    

    [Collection(nameof(DbSettingProviderCollection))]
    public class TransactionServiceTest
    {
        private readonly ITransactionService _runner;

        public TransactionServiceTest()
        {
            _runner = new TransactionService();
        }

        //[Fact]
        //public void Can_GetTransactionHistory()
        //{
        //    // Act
        //    var actualResult = _runner.GetTransactionHistory(ExpectedFilter.ExpectedTransactionHistoryFilter);

        //    // Assert
        //    Assert.IsType<List<Transaction>>(actualResult);
        //}

        [Theory]
        [InlineData("20.0.255.255")]
        public void Can_GetIPInformation(string ipAddress)
        {
            // Act
            var actualResult = _runner.GetIPInformation(ipAddress);

            // Assert
            Assert.IsType<IPInformation>(actualResult);
        }
    }
}