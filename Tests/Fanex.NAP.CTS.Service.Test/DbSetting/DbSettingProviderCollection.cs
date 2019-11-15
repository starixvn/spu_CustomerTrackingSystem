namespace Fanex.NAP.CTS.Service.Test.DbSetting
{
    using Xunit;

    [CollectionDefinition(nameof(DbSettingProviderCollection))]
    public class DbSettingProviderCollection : ICollectionFixture<DbSettingProviderFixture>
    {
    }
}