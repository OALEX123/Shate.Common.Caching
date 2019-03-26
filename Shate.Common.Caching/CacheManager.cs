using System.Collections.Generic;
//using Shate.Common.Caching.Logging;

namespace Shate.Common.Caching
{
    /// <summary>
    /// Cache manager wrapper
    /// </summary>
    public class CacheManager : ICacheManager
    {
        private readonly IServiceManager _serviceManager;

        /// <summary>
        /// Приватный конструктор
        /// </summary>
        private CacheManager(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;

            //CachingLogger.Log.CacheManagerInstanceCreated(_serviceManager.GetCurrentCacheService().Name);
        }

        public IDataExchangeable GetCurrentCacheService()
        {
            return _serviceManager.GetCurrentCacheService();
        }

        public static CacheManager CreateFromConfig(CachingConfig cfg)
        {
            var services = new List<ICacheService>();

            if (cfg.IsCacheEnabled)
            {
                foreach (var providerCfg in cfg.ProviderConfigs)
                {
                    if (providerCfg is RedisConfig)
                    {
                        if (providerCfg.IsCacheEnabled)
                        {
                            services.Add(CacheServiceFactory.GetRedisCacheService(providerCfg as RedisConfig));
                        }
                    }

                    if (providerCfg is InMemoryConfig)
                    {
                        if (providerCfg.IsCacheEnabled)
                        {
                            services.Add(CacheServiceFactory.GetInMemoryCacheService(providerCfg));
                        }
                    }
                }
            }
            else
            {
                services.Add(CacheServiceFactory.GetDefaultEmptyCacheService());
            }

            var serviceManager = ServiceManager.Register(services);

            return new CacheManager(serviceManager);
        }
    }
}
