namespace Shate.Common.Caching
{
    internal class DefaultCacheService : CacheServiceBase
    {
        public DefaultCacheService()
        {
            Provider = CacheProviderFactory.GetDefaultCacheProvider(this);
        }

        public override string Name => CacheServiceName.Default;

        public override ICacheProvider Provider { get; }
    }
}
