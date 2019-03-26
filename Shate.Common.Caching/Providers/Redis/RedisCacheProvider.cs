using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
//using Shate.Common.Caching.Logging;
using StackExchange.Redis;

namespace Shate.Common.Caching
{
    /// <summary>
    /// Represents a manager for caching in Redis store
    /// </summary>
    public class RedisCacheProvider : CacheProviderBase
    {
        private readonly RedisConnectionManager _connectionManager;
        private IDatabase _db;
        private bool _isDatabaseAvaliable = true;

        public RedisCacheProvider(RedisConfig config, ICacheService service)
            : base(service)
        {
            _connectionManager = new RedisConnectionManager(config);
        }

        public override void Dispose()
        {
            _connectionManager.Dispose();
        }

        #region get

        public override async Task<T> GetAsync<T>(string key)
        {
            return await GetValueAsync<T>(key, decompressed: false);
        }

        public override async Task<IDictionary<string, T>> GetListAsync<T>(string[] keys)
        {
            try
            {
                ConfirmRedisOperationAvailable();

                keys = keys.Distinct().Select(ClearKey).ToArray();

                var dictionary = new Dictionary<string, T>();

                var listOfRedisKeys = keys.Select(key => (RedisKey)key).ToList();

                var cacheResult = await _db.StringGetAsync(listOfRedisKeys.ToArray());

                for (var i = 0; i < keys.Count(); i++)
                {
                    var redisValue = cacheResult[i];
                    byte[] value = (byte[])redisValue;
                    var typeValue = Deserialize<T>(value);
                    dictionary.Add(keys[i], typeValue);
                }

                return dictionary;
            }
            catch (Exception ex)
            {
                RegisterError(ex);
                throw ex;
            }
        }

        private async Task<T> GetValueAsync<T>(string key, bool decompressed = false)
        {
            try
            {
                ConfirmRedisOperationAvailable();

                var value = await _db.StringGetAsync(ClearKey(key));

                if (!value.HasValue)
                {
                    return default(T);
                }

                return Deserialize<T>(value, decompressed);
            }
            catch (Exception ex)
            {
                RegisterError(ex);
                throw ex;
            }
        }

        public override async Task<T> GetDecompressedAsync<T>(string key)
        {
            return await GetValueAsync<T>(key, decompressed: true);
        }

        public override async Task<string> BitOpAsync(BitOperation operation, string[] keys)
        {
            try
            {
                ConfirmRedisOperationAvailable();

                keys = keys.Select(ClearKey).ToArray();

                var redisKeys = keys.Select(k => (RedisKey)k).ToArray();

                var destKey = (RedisKey)string.Join(",", keys);

                long result = await _db.StringBitOperationAsync((Bitwise)(int)operation, destKey, redisKeys);

                return await GetAsync<string>(destKey);
            }
            catch (Exception ex)
            {
                RegisterError(ex);
                throw ex;
            }
        }

        #endregion

        #region set

        public override async Task<bool> SetAsync(string key, object data, int cacheTime)
        {
            await SetValueAsync(key, data, cacheTime, compressed: false);

            return true;
        }

        public override async Task<bool> SetListAsync<T>(IDictionary<string, T> values, int cacheTime)
        {
            var operationsToExecute = new ConcurrentBag<KeyValuePair<string, Task<bool>>>();

            try
            {
                ConfirmRedisOperationAvailable();

                // Variant 2 (use TimeSpan)                
                var batch = _db.CreateBatch();
                foreach (var pair in values)
                {
                    operationsToExecute.Add(new KeyValuePair<string, Task<bool>>(ClearKey(pair.Key),
                        batch.StringSetAsync(ClearKey(pair.Key),
                            Serialize(pair.Value), new TimeSpan(0, cacheTime, 0),
                            When.Always, CommandFlags.DemandMaster)));
                }

                batch.Execute();

                var batchResult = Task.WhenAll(operationsToExecute.Select(item => item.Value).ToArray());

                await batchResult;

                var negativeResults = operationsToExecute.Where(item => item.Value.Result.Equals(false)).ToList();
                if (batchResult.IsFaulted || negativeResults.Any())
                {
                    foreach (var operation in negativeResults)
                        RegisterError(new Exception($@"Batch recording has error for KEY = {operation.Key}"));

                    return false;
                }
            }
            catch (Exception ex)
            {
                RegisterError(ex);
                throw ex;
            }

            return true;
        }

        private async Task<bool> SetValueAsync(string key, object data, int cacheTime, bool compressed = false)
        {
            try
            {
                ConfirmRedisOperationAvailable();

                if (data == null)
                {
                    return false;
                }

                var entryBytes = Serialize(data, compressed);
                var expiresIn = TimeSpan.FromMinutes(cacheTime);

                return await _db.StringSetAsync(ClearKey(key), entryBytes, expiresIn);

            }
            catch (Exception ex)
            {
                RegisterError(ex);
                throw ex;
            }
        }

        public override async Task<bool> IsSetAsync(string key)
        {
            try
            {
                ConfirmRedisOperationAvailable();

                return await _db.KeyExistsAsync(ClearKey(key));
            }
            catch (Exception ex)
            {
                RegisterError(ex);
                throw;
            }
        }

        public override async Task<bool> SetCompressedAsync(string key, object data, int cacheTime)
        {
            return await SetValueAsync(key, data, cacheTime, compressed: true);
        }

        #endregion

        #region remove

        public override async Task<bool> RemoveAsync(string key)
        {
            try
            {
                ConfirmRedisOperationAvailable();

                return await _db.KeyDeleteAsync(ClearKey(key));
            }
            catch (Exception ex)
            {
                RegisterError(ex);
                throw;
            }
        }

        public override async Task<bool> RemoveAllAsync(string[] keys)
        {
            if (!keys.Any()) return false;

            var operationsToExecute = new ConcurrentBag<KeyValuePair<string, Task<bool>>>();

            try
            {
                ConfirmRedisOperationAvailable();

                var batch = _db.CreateBatch();

                foreach (var key in keys)
                {
                    operationsToExecute.Add(new KeyValuePair<string, Task<bool>>(ClearKey(key), batch.KeyDeleteAsync(ClearKey(key))));
                }

                batch.Execute();

                var batchResult = Task.WhenAll(operationsToExecute.Select(item => item.Value).ToArray());

                await batchResult;

                var negativeResults = operationsToExecute.Where(item => item.Value.Result == false).ToList();

                if (batchResult.IsFaulted || negativeResults.Any())
                {
                    foreach (var operation in negativeResults)
                    {
                        RegisterError(new Exception($@"Batch deleting has error for KEY = {operation}"));
                    }

                    return false;
                }

            }
            catch (Exception ex)
            {
                RegisterError(ex);
                throw ex;
            }

            return true;
        }

        public override async Task<bool> RemoveByPatternAsync(string pattern)
        {
            bool result = false;

            try
            {
                ConfirmRedisOperationAvailable();

                foreach (var ep in _connectionManager.GetEndpoints())
                {
                    var keys = _connectionManager.GetServer(ep).Keys(
                        pattern: "*" + ClearKey(pattern) + "*",
                        pageSize: _connectionManager.Config.ScanPageSize);

                    var keysToRemove = keys.Select(i => i.ToString()).ToArray();
                    result = await RemoveAllAsync(keysToRemove);
                }
            }
            catch (Exception ex)
            {
                RegisterError(ex);
                throw;
            }

            return result;
        }

        #endregion

        #region private

        private void ConfirmRedisOperationAvailable()
        {
            if (!IsDatabaseAvailable())
            {
                throw new CacheProviderNotAvailableException();
            }
        }

        /// <summary>
        /// Проверяем, доступен ли редис
        /// </summary>
        private bool IsDatabaseAvailable()
        {
            if (_db == null)
            {
                _isDatabaseAvaliable = _connectionManager.TryGetDatabase(out _db);
            }

            return _isDatabaseAvaliable && _db.IsConnected(default(RedisKey));
        }

        /// <summary>
        /// Сериализация в Json
        /// </summary>
        /// <param name="item">Объект для сериализации</param>
        /// <param name="compress">Сжимать данные или нет</param>
        /// <returns></returns>
        private byte[] Serialize(object item, bool compress = false)
        {
            var jsonString = JsonConvert.SerializeObject(item);

            if (compress)
            {
                using (var outStream = new MemoryStream())
                {
                    using (var tinyStream = new GZipStream(outStream, CompressionMode.Compress))
                    using (var mStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
                        mStream.CopyTo(tinyStream);

                    var compressed = outStream.ToArray();

                    return compressed;
                }
            }

            return Encoding.UTF8.GetBytes(jsonString);
        }

        /// <summary>
        /// Десериализация из Json
        /// </summary>
        /// <typeparam name="T">Десереализация объекта</typeparam>
        /// <param name="serializedObject">Объект для десериализации</param>
        /// <param name="compressed">Хранится ли ключ в сжатом виде</param>
        /// <returns></returns>
        private T Deserialize<T>(byte[] serializedObject, bool compressed = false)
        {
            if (serializedObject == null)
                return default(T);

            if (compressed)
            {
                using (var inStream = new MemoryStream(serializedObject))
                using (var bigStream = new GZipStream(inStream, CompressionMode.Decompress))
                using (var bigStreamOut = new MemoryStream())
                {
                    bigStream.CopyTo(bigStreamOut);
                    var decompressedJsonString = Encoding.UTF8.GetString(bigStreamOut.ToArray());
                    return JsonConvert.DeserializeObject<T>(decompressedJsonString);
                }
            }

            var jsonString = Encoding.UTF8.GetString(serializedObject);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        private void RegisterError(Exception ex)
        {
            //CachingLogger.Log.RedisFailed(ex.GetBaseException().Message);
        }

        private string ClearKey(string key) => key.ToUpper();

        /// <summary>
        /// Получить шаблон из ключа с параметрами (напр., "BLP.SalesOffice.{0}.Vendors")
        /// </summary>
        /// <param name="key">Шаблон</param>
        /// <returns></returns>
        private string GetTemplateFromKey(string key)
        {
            string template = key;
            string pattern = @"{\d}";
            Regex rgx = new Regex(pattern);
            string sentence = key;

            foreach (Match match in rgx.Matches(sentence))
                template = template.Replace(match.Value, "*");

            return template;
        }

        #endregion
    }
}
