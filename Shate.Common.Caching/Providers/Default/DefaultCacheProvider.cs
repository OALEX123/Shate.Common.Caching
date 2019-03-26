using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shate.Common.Caching
{
    public class DefaultCacheProvider : CacheProviderBase
    {
        public DefaultCacheProvider(ICacheService service) : base(service)
        {
        }

        #region get

        public override async Task<T> GetAsync<T>(string key) => default(T);
        public override async Task<IDictionary<string, T>> GetListAsync<T>(string[] keys) => GetDefaultDictionaryWithEmptyValues<T>(keys);
        public override async Task<T> GetDecompressedAsync<T>(string key)
        {
            return await GetAsync<T>(key);
        }

        #endregion

        #region remove

        public override async Task<bool> RemoveAllAsync(string[] keys) => false;

        public override async Task<bool> RemoveAsync(string key) => false;

        public override async Task<bool> RemoveByPatternAsync(string pattern) => false;

        #endregion

        #region set

        public override async Task<bool> SetAsync(string key, object data, int cacheTime) => false;
        public override async Task<bool> SetListAsync<T>(IDictionary<string, T> values, int cacheTime) => false;
        public override async Task<bool> IsSetAsync(string key) => false;
        public override async Task<bool> SetCompressedAsync(string key, object data, int cacheTime)
        {
            return await SetAsync(key, data, cacheTime);
        }

        public override Task<string> BitOpAsync(BitOperation operation, string[] keys)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
