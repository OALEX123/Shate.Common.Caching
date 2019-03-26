namespace Shate.Common.Caching
{
    internal class RedisCacheService : CacheServiceBase
    {
        public RedisCacheService(RedisConfig cfg)
        {
            Provider = CacheProviderFactory.GetRedisCacheProvider(this, cfg);
        }

        public override string Name => CacheServiceName.Redis;

        public override ICacheProvider Provider { get; }
    }
}
