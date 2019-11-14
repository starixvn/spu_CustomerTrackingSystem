namespace Fanex.NAP.CTS.Controller.Test
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Data.SqlServer;
    using Fanex.NAP.CTS.Core.Utilities;
    using System;

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
        }

        public void Dispose()
        {
        }
    }
}