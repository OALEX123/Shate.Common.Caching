using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shate.Common.Caching
{
    public interface IDataExchangeable
    {
        /// <summary>
        /// Get async a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Cache key</param>
        /// <returns>Task get cached item</returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Get async a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>Task get cached item</returns>
        Task<T> GetAsync<T>(string key, Func<Task<T>> acquire);

        /// <summary>
        /// Get async a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="cacheTime">Cache time in minutes (0 - do not cache)</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>Task get cached item</returns>
        Task<T> GetAsync<T>(string key, int cacheTime, Func<Task<T>> acquire);

        /// <summary>
        /// Gets list from redis
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys">List of keys</param>
        /// <returns>Dictionary key/value(T)</returns>
        Task<IDictionary<string, T>> GetListAsync<T>(string[] keys) where T : class;

        /// <summary>
        /// Gets or sets the decompressed value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        Task<T> GetDecompressedAsync<T>(string key);

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        Task<bool> SetAsync(string key, object data, int cacheTime);

        /// <summary>
        /// Sends list to redis
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="cacheTime"></param>
        /// <returns></returns>
        Task<bool> SetListAsync<T>(IDictionary<string, T> values, int cacheTime) where T : class;

        /// <summary>
        /// Adds the specified key and compress and save object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        Task<bool> SetCompressedAsync(string key, object data, int cacheTime);

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        Task<bool> IsSetAsync(string key);

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        Task<bool> RemoveAsync(string key);

        /// <summary>
        /// Removes the values with the specified keys from the cache
        /// </summary>
        /// <param name="keys">/keys</param>
        /// <returns>Operation result</returns>
        Task<bool> RemoveAllAsync(string [] keys);

        /// <summary>
        /// Removes the value with the specified pattern
        /// </summary>
        /// <param name="pattern"></param>
        Task<bool> RemoveByPatternAsync(string pattern);

        /// <summary>
        /// Выполняет битовоую операцию
        /// </summary>
        /// <param name="operation">Битовая операция</param>
        /// <param name="keys">Ключи</param>
        /// <returns></returns>
        Task<string> BitOpAsync(BitOperation operation, string[] keys);
    }
}
