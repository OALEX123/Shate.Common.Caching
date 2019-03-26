namespace Shate.Common.Caching
{
    internal class MemoryCacheService : CacheServiceBase
    {
        public MemoryCacheService()
        {
            Provider = CacheProviderFactory.GetInMemoryCacheProvider(this);
        }

        public override string Name => CacheServiceName.InMemory;

        public override ICacheProvider Provider { get; }
    }
}
