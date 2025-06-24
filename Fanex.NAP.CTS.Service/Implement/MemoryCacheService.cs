namespace Fanex.NAP.CTS.Service.Implement
{
    using System;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Service.Interface;


    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly static MemoryCache _memoryCache;

        static MemoryCacheService()
        {
            if (_memoryCache == null)
            {
                _memoryCache = new MemoryCache(nameof(MemoryCacheService));
            }
        }

        public T GetValue<T>(string key) where T : class
        {
            try
            {
                var jsonObject = key.IsNotNullOrEmpty() ? _memoryCache.Get(key) : null;

                return jsonObject != null ? JsonConvert.DeserializeObject<T>((string)jsonObject) : null;
            }
            catch (Exception ex)
            {
                LogExtension.LogError(ex, $"Memory Cache Error {nameof(GetValue)}");
            }

            return null;
        }

        public bool PushValue<T>(string key, T value, int hours = 1) where T : class
        {
            var result = false;

            try
            {
                if (value != null)
                {
                    result = _memoryCache.Add(key, JsonConvert.SerializeObject(value), DateTimeOffset.UtcNow.AddHours(hours));
                }
            }
            catch (Exception ex)
            {
                LogExtension.LogError(ex, $"Memory Cache Error {nameof(PushValue)}");
            }

            return result;
        }

        public async Task<bool> PushValueAsync<T>(string key, T value, int hours = 1) where T : class
        {
            return await Task.Run(() =>
            {
                var result = false;
                try
                {
                    if (value != null)
                    {
                        result = _memoryCache.Add(key, JsonConvert.SerializeObject(value), DateTimeOffset.UtcNow.AddHours(hours));
                    }
                }
                catch (Exception ex)
                {
                    LogExtension.LogError(ex, $"Memory Cache Error {nameof(PushValueAsync)}");
                }

                return result;
            });
        }

        public async Task<bool> RemoveValueAsync(string key)
        {
            return await Task.Run(() =>
            {
                var result = false;
                try
                {
                    if (_memoryCache.Contains(key))
                    {
                        _memoryCache.Remove(key);
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    LogExtension.LogError(ex, $"Memory Cache Error {nameof(RemoveValueAsync)}");
                }

                return result;
            });
        }

        public void RemoveKeyContainsValue(string value)
        {
            try
            {
                var keys = _memoryCache.Where(x => x.Key.Contains(value)).Select(x => x.Key);
                foreach (var key in keys)
                {
                    if (_memoryCache.Contains(key))
                    {
                        _memoryCache.Remove(key);
                    }
                }
            }
            catch (Exception ex)
            {
                LogExtension.LogError(ex, $"Memory Cache Error {nameof(RemoveKeyContainsValue)}");
            }
        }

        public IEnumerable<string> GetKeyContainsValue(string value)
        {
            return _memoryCache.Where(x => x.Key.Contains(value)).Select(x => x.Key);
        }

        public object GetValueByKey(string key)
        {
            return key.IsNotNullOrEmpty() ? _memoryCache.Get(key) : null;
        }
    }
}
