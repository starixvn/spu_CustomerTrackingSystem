namespace Fanex.NAP.CTS.Controller.Test
{
    using Xunit;

    [CollectionDefinition(nameof(DbSettingProviderCollection))]
    public class DbSettingProviderCollection : ICollectionFixture<DbSettingProviderFixture>
    {
    }
}