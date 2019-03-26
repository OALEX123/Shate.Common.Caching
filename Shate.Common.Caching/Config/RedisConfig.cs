namespace Shate.Common.Caching
{
    public class RedisConfig : ICacheProviderConfig
    {
        /// <summary>
        /// Redis connection string. Used when Redis caching is enabled
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Redis SCAN command PageSize
        /// </summary>
        public int ScanPageSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCacheEnabled { get; set; }
    }
}
