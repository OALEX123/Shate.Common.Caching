using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Shate.Common.Caching
{
    /// <summary>
    /// Represents a manager for caching between HTTP requests (long term caching)
    /// </summary>
    public class MemoryCacheProvider : CacheProviderBase
    {
        public MemoryCacheProvider(ICacheService service) : base(service)
        {
        }

        /// <summary>
        /// Cache object
        /// </summary>
        protected ObjectCache Cache => MemoryCache.Default;

        #region get
        public override async Task<T> GetAsync<T>(string key) => (T)Cache[key];
        public override async Task<IDictionary<string, T>> GetListAsync<T>(string[] keys)
        {
            var dict = new Dictionary<string, T>();

            foreach (var key in keys)
            {
                T value = await GetAsync<T>(key);
                dict.Add(key, value);
            }

            return dict;
        }
        public override async Task<T> GetDecompressedAsync<T>(string key)
        {
            return await GetAsync<T>(key);
        }

        #endregion

        #region set
        public override async Task<bool> SetAsync(string key, object data, int cacheTime)
        {
            if (data == null)
                return false;

            var policy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime) };
            Cache.Set(new CacheItem(key, data), policy);

            return true;
        }
        public override async Task<bool> SetListAsync<T>(IDictionary<string, T> values, int cacheTime) => false;
        public override async Task<bool> SetCompressedAsync(string key, object data, int cacheTime)
        {
            return await SetAsync(key, data, cacheTime);
        }
        public override async Task<bool> IsSetAsync(string key) => Cache.Contains(key);

        #endregion

        #region remove

        public  override async Task<bool> RemoveAsync(string key)
        {
            Cache.Remove(key);

            return true;
        }
        public  override async Task<bool> RemoveAllAsync(string[] keys)
        {
            foreach (var key in keys)
            {
                await RemoveAsync(key);
            }

            return true;
        }
        public  override async Task<bool> RemoveByPatternAsync(string pattern)
        {
            return true;
        }

        public override Task<string> BitOpAsync(BitOperation operation, string[] keys)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
