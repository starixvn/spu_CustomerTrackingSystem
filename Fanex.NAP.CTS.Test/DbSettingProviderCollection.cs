namespace Fanex.NAP.CTS.Test
{
    using Xunit;

    [CollectionDefinition(nameof(DbSettingProviderCollection))]
    public class DbSettingProviderCollection : ICollectionFixture<DbSettingProviderFixture>
    {
    }
}