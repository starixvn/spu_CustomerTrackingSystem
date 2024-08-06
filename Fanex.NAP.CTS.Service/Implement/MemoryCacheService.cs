namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Service.Interface;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Threading.Tasks;

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
                LogService.Info("Memory Cache Error: " + ex.Message + ex.StackTrace + "\n"
                     + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                     + ex.InnerException.StackTrace);
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
                LogService.Info("Memory Cache Error: " + ex.Message + ex.StackTrace + "\n"
                    + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                    + ex.InnerException.StackTrace);
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
                    LogService.Info("Memory Cache Error: " + ex.Message + ex.StackTrace + "\n"
                        + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                        + ex.InnerException.StackTrace);
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
                    LogService.Info("Memory Cache Error: " + ex.Message + ex.StackTrace + "\n"
                        + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                        + ex.InnerException.StackTrace);
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
                LogService.Info("Memory Cache Error: " + ex.Message + ex.StackTrace + "\n"
                    + ex.InnerException.Message + ex.InnerException.Source + ex.InnerException
                    + ex.InnerException.StackTrace);
            }
        }

        public IEnumerable<string> GetKeyContainsValue(string value)
        {
            return _memoryCache.Where(x => x.Key.Contains(value)).Select(x => x.Key);
        }
    }
}
