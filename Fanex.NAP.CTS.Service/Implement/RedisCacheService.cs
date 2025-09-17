namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Service.Interface;
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class RedisCacheService : IMemoryCacheService
    {
        private static readonly Lazy<ConnectionMultiplexer> LazyConnection =
            new Lazy<ConnectionMultiplexer>(() =>
            {
                var conn = ConfigSettings.RedisConnection;
                return ConnectionMultiplexer.Connect(conn);
            });

        private static ConnectionMultiplexer Connection => LazyConnection.Value;
        private static IDatabase Database => Connection.GetDatabase();

        public T GetValue<T>(string key) where T : class
        {
            try
            {
                var value = Database.StringGet(key);
                if (value.IsNullOrEmpty) return null;

                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                LogExtension.LogError(ex, $"Redis Cache Error {nameof(GetValue)}");
            }

            return null;
        }

        public bool PushValueBySlidingTime<T>(string key, T value, int hours = 1) where T : class
        {
            try
            {
                if (value == null) return false;
                var json = JsonConvert.SerializeObject(value);
                return Database.StringSet(key, json, TimeSpan.FromHours(hours));
            }
            catch (Exception ex)
            {
                LogExtension.LogError(ex, $"Redis Cache Error {nameof(GetValue)}");
            }

            return false;
        }

        public object GetValueByKey(string key)
        {
            try
            {
                var value = Database.StringGet(key);
                return value.IsNullOrEmpty ? null : value.ToString();
            }
            catch (Exception ex)
            {
                LogExtension.LogError(ex, $"Redis Cache Error {nameof(GetValue)}");
            }

            return null;
        }

        public bool PushValue<T>(string key, T value, int hours = 1) where T : class
        {
            try
            {
                if (value == null) return false;
                var json = JsonConvert.SerializeObject(value);
                return Database.StringSet(key, json, TimeSpan.FromHours(hours));
            }
            catch (Exception ex)
            {
                LogExtension.LogError(ex, $"Redis Cache Error {nameof(GetValue)}");
            }

            return false;
        }

        public async Task<bool> PushValueAsync<T>(string key, T value, int hours = 1) where T : class
        {
            try
            {
                if (value == null) return false;
                var json = JsonConvert.SerializeObject(value);
                return await Database.StringSetAsync(key, json, TimeSpan.FromHours(hours));
            }
            catch (Exception ex)
            {
                LogExtension.LogError(ex, $"Redis Cache Error {nameof(GetValue)}");
            }

            return false;
        }

        public async Task<bool> RemoveValueAsync(string key)
        {
            try
            {
                return await Database.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                LogExtension.LogError(ex, $"Redis Cache Error {nameof(GetValue)}");
            }

            return false;
        }

        public void RemoveKeyContainsValue(string value)
        {
            try
            {
                var endpoints = Connection.GetEndPoints();
                var server = Connection.GetServer(endpoints.First());

                foreach (var key in server.Keys(pattern: $"*{value}*"))
                {
                    Database.KeyDelete(key);
                }
            }
            catch (Exception ex)
            {
                LogExtension.LogError(ex, $"Redis Cache Error {nameof(GetValue)}");
            }
        }

        public IEnumerable<string> GetKeyContainsValue(string value)
        {
            try
            {
                var endpoints = Connection.GetEndPoints();
                var server = Connection.GetServer(endpoints.First());

                return server.Keys(pattern: $"*{value}*").Select(k => (string)k).ToList();
            }
            catch (Exception ex)
            {
                // log error
                return Enumerable.Empty<string>();
            }
        }
    }
}
