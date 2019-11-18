namespace Fanex.NAP.CTS.Service.Test.DbSetting
{
    using Fanex.BetList.Core;
    using Fanex.BetList.Core.RemotingRef;
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.SqlServer;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.RemotingRef;
    using System;
    using System.Configuration;

    public sealed class DbSettingProviderFixture : IDisposable
    {
        public DbSettingProviderFixture()
        {
            DbSettingProviderManager
                .StartNewSession()
                .WithSqlServer()
                .WithMySql()
                .UseDefaultDbSettingProvider(resourcePath: ConfigSettings.GetConfigStringByName("UAT") ?? "~/App_Data")
                .UseDefaultConnectionStringProvider()
                .Run();

            RefDataTypeContainer
               .ConnectTo(new Uri(ConfigurationManager.AppSettings["Fanex.RemotingRef.Service.Url"]))
               .FromApplication("SPU - Fraud Sensor");

            BetListManager.RegisterRemotingRef(new CacheRef());
        }

        public void Dispose()
        {
        }
    }
}