namespace Shate.Common.Caching
{
    internal class CacheProviderFactory//: ICacheProviderFactory
    {
        public static ICacheProvider GetInMemoryCacheProvider(ICacheService service)
        {
            return new MemoryCacheProvider(service);
        }

        public static ICacheProvider GetRedisCacheProvider(ICacheService service, RedisConfig cfg)
        {
            return new RedisCacheProvider(cfg, service);
        }

        public static ICacheProvider GetDefaultCacheProvider(ICacheService service)
        {
            return new DefaultCacheProvider(service);
        }
    }
}
