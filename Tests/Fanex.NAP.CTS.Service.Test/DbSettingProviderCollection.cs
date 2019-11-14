namespace Fanex.NAP.CTS.Service.Test
{
    using Xunit;

    [CollectionDefinition(nameof(DbSettingProviderCollection))]
    public class DbSettingProviderCollection : ICollectionFixture<DbSettingProviderFixture>
    {
    }
}