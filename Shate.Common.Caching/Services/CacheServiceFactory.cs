namespace Shate.Common.Caching
{
    internal class CacheServiceFactory
    {
        public static ICacheService GetDefaultEmptyCacheService()
        {
            return new DefaultCacheService();
        }

        public static ICacheService GetRedisCacheService(RedisConfig cfg)
        {
            return new RedisCacheService(cfg);
        }

        public static ICacheService GetInMemoryCacheService(ICacheProviderConfig cfg)
        {
            return new MemoryCacheService();
        }
    }
}
