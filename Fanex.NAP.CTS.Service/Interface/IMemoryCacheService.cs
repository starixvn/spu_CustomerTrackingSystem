namespace Fanex.NAP.CTS.Service.Interface
{
    using System.Threading.Tasks;

    public interface IMemoryCacheService
    {
        T GetValue<T>(string key) where T : class;

        bool PushValue<T>(string key, T value, int hours = 1) where T : class;

        Task<bool> PushValueAsync<T>(string key, T value, int hours = 1) where T : class;

        Task<bool> RemoveValueAsync(string key);

        void RemoveKeyContainsValue(string value);
    }
}
