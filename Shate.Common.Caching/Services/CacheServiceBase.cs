using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shate.Common.Caching
{
    internal abstract class CacheServiceBase : ICacheService
    {
        protected CacheServiceBase(CacheServicePriority priority = CacheServicePriority.Secondary)
        {
            Priority = priority;
            State = CacheServiceState.Active;
        }

        public abstract string Name { get; }

        public abstract ICacheProvider Provider { get; }

        public CacheServicePriority Priority { get; set; }

        public CacheServiceState State { get; private set; }

        public bool IsActive { get; set; }
        
        #region IDataExchangeable

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                return await Provider.GetAsync<T>(key);
            }
            catch (Exception ex)
            {
                HandleProviderFailed(ex);
            }

            return default(T);
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire)
        {
            try
            {
                return await Provider.GetAsync<T>(key, acquire);
            }
            catch (Exception ex)
            {
                HandleProviderFailed(ex);
            }

            return default(T);
        }

        public async Task<T> GetDecompressedAsync<T>(string key)
        {
            try
            {
                return await Provider.GetDecompressedAsync<T>(key);
            }
            catch (Exception ex)
            {
                HandleProviderFailed(ex);
            }

            return default(T);
        }

        public async Task<bool> SetAsync(string key, object data, int cacheTime)
        {
            try
            {
                return await Provider.SetAsync(key, data, cacheTime);
            }
            catch (Exception ex)
            {
                HandleProviderFailed(ex);

                return false;
            }
        }

        public async Task<bool> IsSetAsync(string key)
        {
            try
            {
                return await Provider.IsSetAsync(key);
            }
            catch (Exception ex)
            {
                HandleProviderFailed(ex);
            }

            return false;
        }

        public async Task<bool> RemoveAsync(string key)
        {
            bool result = false;
            try
            {
                result = await Provider.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                HandleProviderFailed(ex);
            }

            return result;
        }

        public async Task<bool> RemoveAllAsync(string [] keys)
        {
            bool result = false;

            try
            {
                result = await Provider.RemoveAllAsync(keys);
            }
            catch (Exception ex)
            {
                HandleProviderFailed(ex);
            }

            return result;
        }

        public async Task<bool> RemoveByPatternAsync(string pattern)
        {
            bool result = false;
            try
            {
                result = await Provider.RemoveByPatternAsync(pattern);
            }
            catch (Exception ex)
            {
                HandleProviderFailed(ex);
            }

            return result;
        }

        public async Task<T> GetAsync<T>(string key, int cacheTime, Func<Task<T>> acquire)
        {
            try
            {
                return await Provider.GetAsync<T>(key, cacheTime, acquire);
            }
            catch (Exception ex)
            {
                HandleProviderFailed(ex);

                var result = await acquire().ConfigureAwait(false);

                return result;
            }
        }

        public async Task<IDictionary<string, T>> GetListAsync<T>(string[] keys) where T : class
        {
            try
            {
                return await Provider.GetListAsync<T>(keys);
            }
            catch (Exception ex)
            {
                HandleProviderFailed(ex);

                var dictionary = new Dictionary<string, T>();

                foreach (var key in keys)
                {
                    dictionary.Add(key, null);
                }

                return dictionary;
            }
        }

        public async Task<bool> SetListAsync<T>(IDictionary<string, T> values, int cacheTime) where T : class
        {
            try
            {
                return await Provider.SetListAsync(values, cacheTime);
            }
            catch (Exception ex)
            {
                HandleProviderFailed(ex);

                return false;
            }
        }

        public async Task<bool> SetCompressedAsync(string key, object data, int cacheTime)
        {
            try
            {
                return await Provider.SetCompressedAsync(key, data, cacheTime);
            }
            catch (Exception ex)
            {
                HandleProviderFailed(ex);

                return false;
            }
        }

        public async Task<string> BitOpAsync(BitOperation operation, string[] keys)
        {
            return await Provider.BitOpAsync(operation, keys);
        }

        #endregion

        /// <summary>
        /// Обрабатывает ошибку провайдера
        /// </summary>
        /// <param name="ex"></param>
        private void HandleProviderFailed(Exception ex)
        {
            //CachingLogger.Log.ServiceProviderFailed(Name, ex.GetBaseException().Message);

            //OnException(this, ex);
        }

        private void RestoreDefaultState()
        {
            State = CacheServiceState.Active;
            //_disabledUntilDate = null;
        }
    }
}
