using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shate.Common.Caching
{
    /// <summary>
    /// Cache manager interface
    /// </summary>
    public abstract class CacheProviderBase : ICacheProvider
    {
        protected readonly ICacheService Service;

        protected CacheProviderBase(ICacheService service)
        {
            Service = service;
        }
        public virtual void Dispose()
        {

        }

        #region get
        public abstract Task<IDictionary<string, T>> GetListAsync<T>(string[] keys) where T : class;

        public abstract Task<T> GetAsync<T>(string key);
        
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire)
        {
            return await GetAsync<T>(key, 60, acquire);
        }

        public async Task<T> GetAsync<T>(string key, int cacheTime,
            Func<Task<T>> acquire)
        {
            try
            {
                if (await IsSetAsync(key))
                {
                    return await GetAsync<T>(key);
                }

                var result = await acquire().ConfigureAwait(false);

                if (cacheTime > 0)
                {
                    await SetAsync(key, result, cacheTime);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public abstract Task<T> GetDecompressedAsync<T>(string key);

        #endregion

        #region set

        public abstract Task<bool> SetAsync(string key, object data, int cacheTime);

        public abstract Task<bool> SetListAsync<T>(IDictionary<string, T> values, int cacheTime) where T : class;

        public abstract Task<bool> IsSetAsync(string key);

        public abstract Task<bool> SetCompressedAsync(string key, object data, int cacheTime);

        #endregion

        #region remove
        public abstract Task<bool> RemoveAsync(string key);

        public abstract Task<bool> RemoveAllAsync(string [] keys);

        public abstract Task<bool> RemoveByPatternAsync(string pattern);

        #endregion

        protected IDictionary<string, T> GetDefaultDictionaryWithEmptyValues<T>(IEnumerable<string> keys) where T : class
        {
            var dictionary = new Dictionary<string, T>();

            foreach (var key in keys)
            {
                dictionary.Add(key, null);
            }

            return dictionary;
        }

        public abstract Task<string> BitOpAsync(BitOperation operation, string[] keys);
    }
}
